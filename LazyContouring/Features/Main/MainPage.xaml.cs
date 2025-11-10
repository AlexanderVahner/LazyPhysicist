using LazyContouring.Models;
using LazyContouring.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        }

        private MainVM vm;
        public MainVM ViewModel
        {
            get => vm;
            set
            {
                vm = value;
                DataContext = vm;
                SliceGrid.Children.Clear();
                SliceGrid.Children.Add(vm.SliceControl);
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

        private void AddStringNodeDrop(object sender, DragEventArgs e)
        {
            ViewModel.AddNodeStringFromDrop(e.Data);
        }
    }
}
