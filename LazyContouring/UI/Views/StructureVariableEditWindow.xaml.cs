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
    /// Interaction logic for StructureVariableEditWindow.xaml
    /// </summary>
    public partial class StructureVariableEditWindow : Window
    {
        public StructureVariableEditWindow()
        {
            InitializeComponent();
        }

        private void PickColorButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sorry. Not implemented yet.", "Oups...");
        }
    }
}
