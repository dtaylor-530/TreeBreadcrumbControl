using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TreeBreadcrumbControl.Commands;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = TextBoxName, Type = typeof(TextBox))]
    public class BreadCrumbsBox : Control, INotifyPropertyChanged
    {
        public const string TextBoxName = "PART_TextBox";

        //public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(
        //    "Object", typeof(object), typeof(BreadCrumbs), new PropertyMetadata(null,
        //        (o, args) =>
        //        {
        //            if (o is BreadCrumbs @this && args.NewValue is object node)
        //            { 
        //                var ancestors = @this.Converter.Convert(node, default, default, default) as IEnumerable<object>;
        //                @this.Root = ancestors.First();
        //                @this.Root = ancestors.Skip(1);
        //                //(@this.Root, @this.Crumbs) = GetAncestors(node);
        //            }
        //        }));
        public static readonly DependencyProperty SetObjectProperty = DependencyProperty.Register(
            "SetCurrentNodeCommand", typeof(ICommand), typeof(BreadCrumbsBox), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty PathSeparatorProperty = DependencyProperty.Register(
            "PathSeparator", typeof(string), typeof(BreadCrumbsBox), new PropertyMetadata("/"));

        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register("Root", typeof(object), typeof(BreadCrumbsBox), new PropertyMetadata());

        public static readonly DependencyProperty DescendantsProperty =
            DependencyProperty.Register("Descendants", typeof(IEnumerable), typeof(BreadCrumbsBox), new PropertyMetadata());
        
        public static readonly DependencyProperty OverflowProperty =
            DependencyProperty.Register("Overflow", typeof(IEnumerable), typeof(BreadCrumbsBox), new PropertyMetadata());






        static BreadCrumbsBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbsBox), new FrameworkPropertyMetadata(typeof(BreadCrumbsBox)));
        }



        //private object _root;
        private bool _isTextMode;
        private TextBox _textBox;


        public BreadCrumbsBox()
        {
            SetTextModeCommand = new RelayCommand(() =>
            {
                IsTextMode = true;

                //_textBox.ExecuteAfterLoaded(@this =>
                //{
                //    @this.Text = string.Join(PathSeparator, Ancestors.Cast<object>().Select(item => item.ToString()));
                //    if (!@this.Focus())
                //    {
                //        throw new InvalidOperationException(
                //            "The focus of the TextBox setting operation should not fail, please check the custom template.");
                //    }
                //    @this.LostKeyboardFocus += TextBoxOnLostKeyboardFocus;
                //});
            });
        }


        #region properties



        public object Root
        {
            get { return (object)GetValue(RootProperty); }
            set { SetValue(RootProperty, value); }
        }

        //public IEnumerable Ancestors
        //{
        //    get { return (IEnumerable)GetValue(AncestorsProperty); }
        //    set { SetValue(AncestorsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Ancestors.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty AncestorsProperty =
        //    DependencyProperty.Register("Ancestors", typeof(IEnumerable), typeof(BreadCrumbs), new PropertyMetadata());


        public IEnumerable Descendants
        {
            get { return (IEnumerable)GetValue(DescendantsProperty); }
            set { SetValue(DescendantsProperty, value); }
        }

        public IEnumerable Overflow
        {
            get { return (IEnumerable)GetValue(OverflowProperty); }
            set { SetValue(OverflowProperty, value); }
        }


        //public IValueConverter Converter
        //{
        //    get { return (IValueConverter)GetValue(ConverterProperty); }
        //    set { SetValue(ConverterProperty, value); }
        //}

        //public object Object
        //{
        //    get => GetValue(ObjectProperty);
        //    set => SetValue(ObjectProperty, value);
        //}

        public ICommand SetObject
        {
            get => (ICommand)GetValue(SetObjectProperty);
            set => SetValue(SetObjectProperty, value);
        }

        public string PathSeparator
        {
            get => (string)GetValue(PathSeparatorProperty);
            set => SetValue(PathSeparatorProperty, value);
        }

        //public IEnumerable Collection
        //{
        //    get => collection;
        //    private set => SetProperty(ref collection, value);
        //}

        //public object Root
        //{
        //    get => _root;
        //    private set => SetProperty(ref _root, value);
        //}

        public bool IsTextMode
        {
            get => _isTextMode;
            private set => SetProperty(ref _isTextMode, value);
        }

        public ICommand SetTextModeCommand { get; }

        #endregion properties


        public override void OnApplyTemplate()
        {
            _textBox = (TextBox)Template.FindName(TextBoxName, this);

            base.OnApplyTemplate();
        }

        private void TextBoxOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _textBox.LostKeyboardFocus -= TextBoxOnLostKeyboardFocus;
            IsTextMode = false;
        }

        //private static  IEnumerable<object> Crumbs GetAncestors(object node)
        //{

        //    var parent = node;
        //    while (parent != null)
        //    {
        //        yield return parent;
        //        parent = parent.Parent;
        //    }
        //}

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
