using LazyContouring.UI.ViewModels.Operations.ContextConditions;
using System.Windows;
using System.Windows.Controls;

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
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ConditionNodeVM vm)
            {
            }

        }
    }
}
