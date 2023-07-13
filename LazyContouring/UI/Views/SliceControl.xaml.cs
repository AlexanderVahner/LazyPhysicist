using LazyContouring.UI.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private SliceVM viewModel;
        public SliceVM ViewModel
        {
            get => viewModel;
            set
            {
                StructuresViewBox.Child = value?.SliceCanvas;
                viewModel = value;
            }
        }
    }
}
