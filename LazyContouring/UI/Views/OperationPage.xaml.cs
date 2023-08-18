﻿using LazyContouring.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazyContouring.UI.Views
{
    /// <summary>
    /// Interaction logic for OperationPage.xaml
    /// </summary>
    public partial class OperationPage : Page
    {
        public OperationPage()
        {
            InitializeComponent();
        }

        private OperationsVM vm;
        public OperationsVM ViewModel
        {
            get => vm;
            set
            {
                vm = value;
                MainGrid.Children.Add(vm.UIElement);
            }
        } 
    }
}