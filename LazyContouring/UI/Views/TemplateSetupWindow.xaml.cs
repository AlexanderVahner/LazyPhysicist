using LazyContouring.UI.ViewModels;
using System.Windows;

namespace LazyContouring.UI.Views
{
    /// <summary>
    /// Interaction logic for TemplateSetupWindow.xaml
    /// </summary>
    public partial class TemplateSetupWindow : Window
    {
        private OperationTemplateVM vM;

        public TemplateSetupWindow()
        {
            InitializeComponent();
        }

        public OperationTemplateVM ViewModel
        {
            get => vM;
            set
            {
                vM = value;
                DataContext = vM;
            }
        }
    }
}
