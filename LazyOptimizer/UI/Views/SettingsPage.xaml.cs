using LazyOptimizer.App;
using LazyPhysicist.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazyOptimizer.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    FileName = "PlansCache.exe",
                    Filter = "PlansCache.exe|PlansCache.exe"
                };

                bool? result = dialog.ShowDialog();

                if (result == true && Settings != null)
                {
                    // Open document
                    Settings.PlansCacheAppPath = dialog.FileName;
                }
            }

            catch (Exception ex)
            {
                Logger.Write(this, $"File Dialog is broken:\n{ex.Message}", LogMessageType.Error);
            }

        }

        public Settings Settings { get; set; }
    }
}
