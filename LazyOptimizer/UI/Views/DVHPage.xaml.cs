using LazyOptimizer.UI.ViewModels;
using System.Windows.Controls;

namespace LazyOptimizer.UI.Views
{
    /// <summary>
    /// Interaction logic for DVHPage.xaml
    /// </summary>
    public partial class DVHPage : Page
    {
        public DVHPage()
        {
            InitializeComponent();
        }

        private DvhVM viewModel;
        public DvhVM ViewModel
        {
            get => viewModel;
            set
            {
                Content = value?.Canvas;
                viewModel = value;
            }
        }
    }
}
