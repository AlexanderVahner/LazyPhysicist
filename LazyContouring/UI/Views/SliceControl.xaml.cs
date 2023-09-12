using LazyContouring.UI.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazyContouring.UI.Views
{
    /// <summary>
    /// Interaction logic for SliceControl.xaml
    /// </summary>
    public partial class SliceControl : UserControl
    {
        public SliceControl()
        {
            InitializeComponent();
            PlaneViewBox.MouseWheel += PlaneViewBox_MouseWheel;
        }

        private void PlaneViewBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (e.Delta > 0)
            {
                ViewModel.CurrentPlaneIndex++;
            }
            else if (e.Delta < 0)
            {
                ViewModel.CurrentPlaneIndex--;
            }
        }

        private ViewPlaneVM viewModel;
        public ViewPlaneVM ViewModel
        {
            get => viewModel;
            set
            {
                PlaneViewBox.Child = value?.PlaneCanvas;
                viewModel = value;
            }
        }
    }
}
