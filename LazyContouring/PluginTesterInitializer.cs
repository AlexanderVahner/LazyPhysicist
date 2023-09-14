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
            Patient patient = App.OpenPatientById("0220005213");
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(ss => ss.Id == "CT_1");
            Course course = patient?.Courses.FirstOrDefault(c => c.Id == "C1");
            ExternalPlanSetup plan = course?.ExternalPlanSetups.FirstOrDefault(p => p.Id == "CV1");

            if (patient == null)
            {
                MessageBox.Show("Patient insn't open");
                return;
            }

            Script script = new Script();
            script.Run(new ScriptArgs()
            {
                CurrentUser = App.CurrentUser,
                Patient = patient,
                StructureSet = structureSet,
                Course = course,
                Plan = plan,
                Window = Window
            });
        }

        public void Dispose()
        {
            App?.Dispose();
        }

        public V.Application App { get; }
        public Window Window { get; }

    }
}
