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
                Paragraph logMessage = new Paragraph(new Run($"{type} from {s?.GetType().Name ?? "UNKNOWN"}: {message}"))
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                LogBox.Document.Blocks.Add(logMessage);
                LogBox.ScrollToEnd();
            };
        }
    }
}
