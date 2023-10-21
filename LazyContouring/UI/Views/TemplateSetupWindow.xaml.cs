using LazyContouring.Operations;
using LazyContouring.UI.ViewModels;
using LazyContouring.UI.ViewModels.Operations.ContextConditions;
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
using System.Windows.Shapes;

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

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedNodeVM = e.NewValue as ConditionNodeVM;
        }
    }
}
