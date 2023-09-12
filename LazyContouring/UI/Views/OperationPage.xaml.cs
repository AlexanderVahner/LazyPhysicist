using LazyContouring.UI.ViewModels;
using System.Windows.Controls;

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
                DataContext = vm;
                MainGrid.Children.Add(vm.UIElement);
            }
        }
    }
}
