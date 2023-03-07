﻿using System.Windows;
using System.Windows.Input;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            this.Content = DataContext;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var uiElement = (UIElement)sender;
            uiElement.Focus();
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {

        }
    }
}
