using LazyContouring.Operations;
using LazyContouring.UI.ViewModels;
using System.Windows.Controls;

namespace LazyContouring.UI.Views
{
    /// <summary>
    /// Interaction logic for TemplatesListControl.xaml
    /// </summary>
    public partial class TemplatesListControl : UserControl
    {
        public TemplatesListControl()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem != null)
            {
                (DataContext as MainVM)?.OpenTemplateSetupWindow((OperationTemplate)lb.SelectedItem);
            }

        }
    }
}
