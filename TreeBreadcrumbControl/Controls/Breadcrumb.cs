using Enums;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TreeBreadcrumbControl.Commands;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
    public class Breadcrumb : ContentControl
    {
        private const string PopupName = "PART_Popup";

        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(
            "Object", typeof(object), typeof(Breadcrumb), new PropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
  
        }

        public static readonly DependencyProperty SetObjectProperty = DependencyProperty.Register(
            "SetObject", typeof(ICommand), typeof(Breadcrumb), new PropertyMetadata(null));



        public Flow Flow
        {
            get { return (Flow)GetValue(FlowProperty); }
            set { SetValue(FlowProperty, value); }
        }


        public static readonly DependencyProperty FlowProperty =
            DependencyProperty.Register("Flow", typeof(Flow), typeof(Breadcrumb), new PropertyMetadata(Flow.Direct));



        static Breadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Breadcrumb), new FrameworkPropertyMetadata(typeof(Breadcrumb)));
        }

        public object Object
        {
            get => (object)GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public ICommand SetObject
        {
            get => (ICommand)GetValue(SetObjectProperty);
            set => SetValue(SetObjectProperty, value);
        }

        // -------------------------------------------------------------------------------------------------------------------------------

        private Popup _popup;

        public ICommand InternalSetCurrentNodeCommand { get; }

        public Breadcrumb()
        {
            InternalSetCurrentNodeCommand = new RelayCommand<object>(parameter =>
            {
                SetObject?.Execute(parameter);
                _popup.SetCurrentValue(Popup.IsOpenProperty, false);
            });
        }

        public override void OnApplyTemplate()
        {
            _popup = (Popup)Template.FindName(PopupName, this);
            base.OnApplyTemplate();
        }
    }

    public class RootBreadcrumb : Breadcrumb
    {
        public static readonly DependencyProperty OverflowProperty = DependencyProperty.Register(
            "Overflow", typeof(IEnumerable), typeof(RootBreadcrumb), new PropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty IsTextModeProperty = DependencyProperty.Register(
            "IsTextMode", typeof(bool), typeof(RootBreadcrumb), new PropertyMetadata(default(bool)));

        public IEnumerable Overflow
        {
            get => (IEnumerable)GetValue(OverflowProperty);
            set => SetValue(OverflowProperty, value);
        }

        public bool IsTextMode
        {
            get => (bool)GetValue(IsTextModeProperty);
            set => SetValue(IsTextModeProperty, value);
        }

        static RootBreadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RootBreadcrumb), new FrameworkPropertyMetadata(typeof(RootBreadcrumb)));
        }
    }
}
