using PluginTesterNameSpace;
using System;
using System.Windows;

namespace PluginTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PluginTesterInitializer tester;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                tester = new PluginTesterInitializer(this);
                tester.Execute();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tester?.Dispose();
        }
    }
}
