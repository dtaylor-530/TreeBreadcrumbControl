using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Enums;
using Utility.Observables;
using WPF.Commands;
using static Evan.Wpf.DependencyHelper;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
    public class Breadcrumb : ContentControl, IObserver
    {
        private const string PopupName = "PART_Popup";

        public static readonly DependencyProperty ObjectProperty = Register(new PropertyMetadata(null, Changed)),
            SetObjectProperty = Register(),
            HasItemsProperty = Register(),
            ChildrenProperty = Register(),
            FlowProperty = Register(new PropertyMetadata(Flow.Direct));
        //    DependencyProperty.Register("Object", typeof(object), typeof(Breadcrumb), new PropertyMetadata(null, Changed));

        //public static readonly DependencyProperty SetObjectProperty = 
        //    DependencyProperty.Register("SetObject", typeof(ICommand), typeof(Breadcrumb), new PropertyMetadata(null));

        ////public static readonly DependencyProperty HasItemsProperty =
        ////    DependencyProperty.Register("HasItems", typeof(bool), typeof(Breadcrumb), new PropertyMetadata(false));

        //public static readonly DependencyProperty FlowProperty =
        //    DependencyProperty.Register("Flow", typeof(Flow), typeof(Breadcrumb), new PropertyMetadata(Flow.Direct));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IObserver observer && e.NewValue is INode { Children: var children } node)
            {
                children
                    .Subscribe(observer);
            }
        }

        static Breadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Breadcrumb), new FrameworkPropertyMetadata(typeof(Breadcrumb)));
        }


        private Popup _popup;


        public Breadcrumb()
        {
            InternalSetCurrentNodeCommand = new RelayCommand<object>(parameter =>
            {
                SetObject?.Execute(parameter);
                _popup.SetCurrentValue(Popup.IsOpenProperty, false);
            });
        }
        public ICommand InternalSetCurrentNodeCommand { get; }
        #region 

        public Flow Flow
        {
            get { return (Flow)GetValue(FlowProperty); }
            set { SetValue(FlowProperty, value); }
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

        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            set { SetValue(HasItemsProperty, value); }
        }

        public Binding Children
        {
            get { return (Binding)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        

        #endregion

        public override void OnApplyTemplate()
        {
            _popup = (Popup)Template.FindName(PopupName, this);
            base.OnApplyTemplate();
        }

        public void OnNext(object value)
        {
            if (value is NotifyCollectionChangedEventArgs { NewItems: { Count: > 0 } newItems } args)
            {
                HasItems = true;
            }
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
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
