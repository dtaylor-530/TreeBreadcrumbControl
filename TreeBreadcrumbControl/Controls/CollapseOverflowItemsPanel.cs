﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Enums;
using Size = System.Windows.Size;
using Orientation = System.Windows.Controls.Orientation;

namespace TreeBreadcrumbControl
{


    public class CollapseOverflowItemsPanel : VirtualizingPanel
    {
        public static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register(
            "OverflowItems", typeof(IEnumerable), typeof(CollapseOverflowItemsPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(CollapseOverflowItemsPanel), new FrameworkPropertyMetadata(
                Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty ReserveProperty = DependencyProperty.Register(
            "Reserve", typeof(bool), typeof(CollapseOverflowItemsPanel), new FrameworkPropertyMetadata(
                false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty FlowProperty = DependencyProperty.RegisterAttached(
            "Flow", typeof(Flow), typeof(CollapseOverflowItemsPanel), new PropertyMetadata(Flow.Direct));

        public static string GetFlow(DependencyObject d)
        {
            return (string)d.GetValue(FlowProperty);
        }
        public static void SetFlow(DependencyObject d, Flow value)
        {
            d.SetValue(FlowProperty, value);
        }


        public IEnumerable OverflowItems
        {
            get => (IEnumerable)GetValue(OverflowItemsProperty);
            private set => SetValue(OverflowItemsProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public bool Reserve
        {
            get => (bool)GetValue(ReserveProperty);
            set => SetValue(ReserveProperty, value);
        }

        public CollapseOverflowItemsPanel()
        {
            OverflowItems = Enumerable.Empty<object>().ToList();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var generatedChildren = GetGeneratedChildren();
            var visibleCount = GetVisibleCount(generatedChildren, availableSize, out var measuredSize);
            var (visibleChildren, overflowItems) = SeparateItems(generatedChildren, visibleCount);

            var children = InternalChildren;
            if (!EqualsList(visibleChildren, children))
            {
                RemoveInternalChildRange(0, children.Count);
                foreach (var child in visibleChildren)
                {
                    AddInternalChild(child);
                }
            }

            if (!EqualsList((IList)OverflowItems, overflowItems))
            {
                OverflowItems = overflowItems;
            }

            foreach(var item in OverflowItems)
            {
                if (item is DependencyObject dependencyObject)
                    SetFlow(dependencyObject, Flow.Over);
                else
                    throw new Exception("fr ");

            }

            return measuredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var isHorizontal = Orientation == Orientation.Horizontal;

            var mainAxisOffset = 0D;
            UIElementCollection children = InternalChildren;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.Arrange(
                    new Rect(isHorizontal ? new Point(mainAxisOffset, 0) : new Point(0, mainAxisOffset),
                    child.DesiredSize));
                mainAxisOffset += isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
            }

            return finalSize;
        }

        private int GetVisibleCount(IEnumerable<UIElement> generatedChildren, Size availableSize, out Size measuredSize)
        {
            var isHorizontal = Orientation == Orientation.Horizontal;

            var mainAxis = 0d;
            var crossAxis = 0d;
            var count = 0;
            var availableMainAxis = isHorizontal ? availableSize.Width : availableSize.Height;
            var source = Reserve ? generatedChildren.Reverse() : generatedChildren;
            foreach (var generatedChild in source)
            {
                generatedChild.Measure(availableSize);
                var (childMainAxis, childCrossAxis) = isHorizontal
                    ? (generatedChild.DesiredSize.Width, generatedChild.DesiredSize.Height)
                    : (generatedChild.DesiredSize.Height, generatedChild.DesiredSize.Width);

                var preMainAxis = mainAxis + childMainAxis;

                if (preMainAxis >= availableMainAxis) break;

                mainAxis = preMainAxis;
                crossAxis = Math.Max(crossAxis, childCrossAxis);
                count++;
            }

            measuredSize = isHorizontal ? new Size(mainAxis, crossAxis) : new Size(crossAxis, mainAxis);
            return count;
        }

        private (UIElement[] visibleChildren, object[] overflowItems) SeparateItems(IReadOnlyCollection<UIElement> generatedChildren, int visibleCount)
        {
            var generator = (ItemContainerGenerator)ItemContainerGenerator;
            var overflowCount = generatedChildren.Count - visibleCount;

            if (Reserve)
            {
                var visibleChildren = generatedChildren.Skip(overflowCount).ToArray();
                var overflowItems = generator.Items.Take(overflowCount).ToArray();
                return (visibleChildren, overflowItems);
            }
            else
            {
                var visibleChildren = generatedChildren.Take(visibleCount).ToArray();
                var overflowItems = generator.Items.Skip(visibleCount).ToArray();
                return (visibleChildren, overflowItems);
            }
        }

        private IReadOnlyCollection<UIElement> GetGeneratedChildren()
        {
            // HACK: Read the InternalChildren property before reading the ItemContainerGenerator property,
            // otherwise, the ItemContainerGenerator property will be null.
            // ReSharper disable once UnusedVariable
            var children = InternalChildren;
            var containerGenerator = ItemContainerGenerator;
            var result = new List<UIElement>();
            using (containerGenerator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
            {
                while (containerGenerator.GenerateNext(out var newlyRealized) is UIElement next)
                {
                    if (newlyRealized)
                    {
                        containerGenerator.PrepareItemContainer(next);
                    }

                    result.Add(next);
                }
            }

            return result;
        }

        private static bool EqualsList(IList list1, IList list2)
        {
            if (Equals(list1, list2)) return true;
            if (list1.Count != list2.Count) return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!Equals(list1[i], list2[i])) return false;
            }

            return true;
        }
    }
}
