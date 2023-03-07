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
using WPF.Commands;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = TextBoxName, Type = typeof(TextBox))]
    public class BreadCrumbsBox : Control, INotifyPropertyChanged
    {
        public const string TextBoxName = "PART_TextBox";


        public static readonly DependencyProperty SetObjectProperty = DependencyProperty.Register(
            "SetObject", typeof(ICommand), typeof(BreadCrumbsBox), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty PathSeparatorProperty = DependencyProperty.Register(
            "PathSeparator", typeof(string), typeof(BreadCrumbsBox), new PropertyMetadata("/"));

        //public static readonly DependencyProperty RootProperty =
        //    DependencyProperty.Register("Root", typeof(object), typeof(BreadCrumbsBox), new PropertyMetadata());

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(IEnumerable), typeof(BreadCrumbsBox), new PropertyMetadata());
        
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

                _textBox.ExecuteAfterLoaded(@this =>
                {
                    @this.Text = string.Join(PathSeparator, Children.Cast<object>().Select(item => item.ToString()));
                    if (!@this.Focus())
                    {
                        throw new InvalidOperationException(
                            "The focus of the TextBox setting operation should not fail, please check the custom template.");
                    }
                    @this.LostKeyboardFocus += TextBoxOnLostKeyboardFocus;
                });
            });
        }


        #region properties


        public IEnumerable Children
        {
            get { return (IEnumerable)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public IEnumerable Overflow
        {
            get { return (IEnumerable)GetValue(OverflowProperty); }
            set { SetValue(OverflowProperty, value); }
        }

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
