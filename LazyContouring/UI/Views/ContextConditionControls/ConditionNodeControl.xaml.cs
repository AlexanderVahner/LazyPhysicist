using LazyContouring.UI.ViewModels.Operations.ContextConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

namespace LazyContouring.UI.Views.ContextConditionControls
{
    /// <summary>
    /// Interaction logic for ConditionNodeControl.xaml
    /// </summary>
    public partial class ConditionNodeControl : UserControl
    {
        private ConditionNodeVM viewModel;
        public static readonly DependencyProperty ViewModelProperty;

        static ConditionNodeControl()
        {
            ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ConditionNodeVM), typeof(ConditionNodeControl));
        }

        public ConditionNodeControl()
        {
            InitializeComponent();
        }

        public ConditionNodeVM ViewModel 
        { 
            get => (ConditionNodeVM)GetValue(ViewModelProperty); 
            set
            {
                SetValue(ViewModelProperty, value);
                viewModel = value;
                DataContext = value;
                MainBorder.Child = value.UIElement;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainBorder.Child = (e.NewValue as ConditionNodeVM).UIElement; 
        }
    }
}
