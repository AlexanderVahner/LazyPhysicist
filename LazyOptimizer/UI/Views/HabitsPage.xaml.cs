using LazyOptimizer.Model;
using LazyOptimizer.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LazyOptimizer.UI.Views
{
    /// <summary>
    /// Interaction logic for HabitsPage.xaml
    /// </summary>
    public partial class HabitsPage : Page
    {
        private HabitsVM vm;
        public HabitsPage()
        {
            InitializeComponent();
        }

        private HabitsVM DefineVM()
        {
            if (vm == null &&  DataContext is HabitsVM hvm)
            {
                vm = hvm;
            }
            return vm;
        }

        public void UndefinedStrucutres_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is ListBoxItem item && item.Content is IStructureSuggestionModel ssm)
            {
                DefineVM()?.FindStructureInOtherPlans(ssm);
            }
        }
    }
}
