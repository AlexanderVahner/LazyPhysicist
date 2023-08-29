﻿using LazyContouring.Models;
using LazyContouring.Operations;
using LazyContouring.UI.ViewModels;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MainVM == null)
            {
                return;
            }

            if (e.Delta > 0)
            {
                MainVM.CurrentSlice++;
                MainVM.SliceCanvas.CurrentSlice++;
            }
            else if (e.Delta < 0)
            {
                MainVM.CurrentSlice--;
                MainVM.SliceCanvas.CurrentSlice--;
            }
        }

        private MainVM mainVM;
        public MainVM MainVM
        {
            get
            {
                if (mainVM == null)
                {
                    if (DataContext is MainVM vm)
                    {
                        mainVM = vm;
                        SliceGrid.Children.Add(vm.SliceControl);
                        //OperationsGrid.Children.Add(vm.OperationPage);
                    }
                }
                return mainVM;
            }
            set
            {
                mainVM = value;
                SliceGrid.Children.Add(value.SliceControl);
                //OperationsGrid.Children.Add(value.OperationPage);
            }
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fwElement && fwElement.Tag is StructureVariable sVar)
            {
                DragDrop.DoDragDrop(fwElement, sVar, DragDropEffects.Move);
            }
        }

        private void OperationBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fwElement && fwElement.Tag is string operation)
            {
                DragDrop.DoDragDrop(fwElement, operation, DragDropEffects.Move);
            }
        }
    }
}
