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

        public UserSettings Settings { get; set; }
    }
}
