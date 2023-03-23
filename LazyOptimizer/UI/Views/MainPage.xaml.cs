using LazyOptimizer.UI.ViewModels;
using LazyPhysicist.Common;
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

namespace LazyOptimizer.UI.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            LogBox.Document = new FlowDocument();

            Logger.Logged += (s, message, type) =>
            {
                if (mainVM == null)
                {
                    if (DataContext is MainVM vm && vm?.Context?.Settings != null)
                    {
                        mainVM = vm;
                    }
                }

                isDebugMode = mainVM?.Context?.Settings?.DebugMode ?? false;

                if (isDebugMode && type == LogMessageType.Debug)
                {
                    return;
                }

                Brush brush = Brushes.Black;
                double fontSize = 12;
                switch (type)
                {
                    case LogMessageType.Debug:
                        brush = Brushes.Gray;
                        fontSize = 10;
                        break;
                    case LogMessageType.Error:
                        brush = Brushes.Red;
                        break;
                    case LogMessageType.Warning:
                        brush = Brushes.Orange;
                        break;
                    case LogMessageType.Info:
                        brush = Brushes.Green;
                        break;
                }
                string text = "";
                if (type == LogMessageType.Debug)
                {
                    text += $"Debug from {s?.GetType().Name ?? "???"}:\n";
                }
                text += message;

                Paragraph logMessage = new Paragraph(new Run(text))
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Foreground = brush,
                    FontSize = fontSize
                };
                LogBox.Document.Blocks.Add(logMessage);
                LogBox.ScrollToEnd();
            };
        }

        private bool isDebugMode = false;
        private MainVM mainVM = null;
    }
}
