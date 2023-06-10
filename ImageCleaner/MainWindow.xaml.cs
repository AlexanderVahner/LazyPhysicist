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
using System.Windows.Threading;

namespace ImageCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitLogger();

            
        }

        public void InitLogger()
        {
            LogBox.Document = new FlowDocument();

            Logger.Logged += (s, message, type) =>
            {
                Paragraph logMessage = new Paragraph(new Run(message));
                LogBox.Document.Blocks.Add(logMessage);
                LogBox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                          new Action(delegate { }));
            };
        }

        
    }

    public class Runner
    {
        public MetaCommand Run => new MetaCommand(
            o => {
                var cleaner = new Cleaner();
                cleaner.Clean();
                cleaner.Dispose();
            }
        );
    }

}
