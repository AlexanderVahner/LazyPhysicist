using ScriptArgsNameSpace;
using System;
using System.Linq;
using System.Windows;
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using V = VMS.TPS.Common.Model.API;

namespace PluginTesterNameSpace
{
    public sealed class PluginTesterInitializer : IDisposable
    {
        public PluginTesterInitializer(Window window = null)
        {
            App = V.Application.CreateApplication();
            Window = window;
        }

        public void Execute()
        {
            Patient patient = App.OpenPatientById("HN2022_asv1");
            Course course = patient?.Courses.FirstOrDefault(c => c.Id == "CV");
            ExternalPlanSetup plan = course?.ExternalPlanSetups.FirstOrDefault(p => p.Id == "Fields");

            if (plan == null)
            {
                MessageBox.Show("Can't find plan");
            }
            else
            {
                Script script = new Script();
                script.Run(new ScriptArgs()
                {
                    Patient = patient,
                    Plan = plan
                });
            }
        }

        public void Dispose()
        {
            App?.Dispose();
        }

        public V.Application App { get; }
        public Window Window { get; }

    }
}
