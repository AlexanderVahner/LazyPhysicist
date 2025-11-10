using LazyContouring.UI.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LazyContouring.UI.Views
{
    /// <summary>
    /// Interaction logic for SliceControl.xaml
    /// </summary>
    public partial class SliceControl : UserControl
    {
        private ScaleTransform scale = new ScaleTransform();

        public SliceControl()
        {
            InitializeComponent();
            PlaneViewBox.RenderTransform = scale;
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

            //if (e.Delta > 0) scale.ScaleX = scale.ScaleX *= 1.1;
            //if (e.Delta < 0) scale.ScaleX = scale.ScaleX /= 1.1;
            //scale.ScaleY = scale.ScaleX;

            // TODO
            // https://www.cyberforum.ru/wpf-silverlight/thread348162.html
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
