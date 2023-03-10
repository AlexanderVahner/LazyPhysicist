using FillTheOptimizer.API;
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
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using ESAPI = VMS.TPS.Common.Model.API;

namespace PluginTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ESAPI.Application app = ESAPI.Application.CreateApplication();
        public MainWindow()
        {
            InitializeComponent();
            Patient patient = app.OpenPatientById("STT 4656468");
            Course course = patient?.Courses.FirstOrDefault(c => c.Id == "C1");
            ExternalPlanSetup plan = course?.ExternalPlanSetups.FirstOrDefault(p => p.Id == "LazyDest");

            if (plan == null)
            {
                MessageBox.Show("Can't find plan");
            }
            else
            {
                Script script = new Script();
                script.Run(new ScriptArgs()
                {
                    CurrentUser = app.CurrentUser,
                    Patient = patient,
                    Plan = plan,
                    Window = this
                });
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            app?.Dispose();
        }
    }
}
