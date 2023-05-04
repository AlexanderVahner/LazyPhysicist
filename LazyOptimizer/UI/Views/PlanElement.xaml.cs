using LazyOptimizer.UI.ViewModels;
using System.Windows.Controls;

namespace LazyOptimizer.UI.Views
{
    /// <summary>
    /// Interaction logic for PlanElement.xaml
    /// </summary>
    public partial class PlanElement : UserControl
    {
        private PlanVM viewModel;

        public PlanElement()
        {
            InitializeComponent();
        }
        public PlanVM ViewModel
        {
            get
            {
                if (DataContext is PlanVM vm)
                {
                    viewModel = vm;
                }
                return viewModel;
            }
        }
    }
}
