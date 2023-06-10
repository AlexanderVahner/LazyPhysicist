using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

[assembly: AssemblyVersion("1.0.0.3")]
[assembly: AssemblyFileVersion("1.0.0.3")]
[assembly: AssemblyInformationalVersion("1.0")]

[assembly: ESAPIScript(IsWriteable = true)]

namespace ImageCleaner
{
    internal class Cleaner : IDisposable
    {
        Application app;
        int iCan = 0, iCant = 0;

        public Cleaner()
        {
            app = Application.CreateApplication();            
        }

        public void Clean()//(CleanSettings settings)
        {
            int i = 0;
            foreach (var patientSummary in app.PatientSummaries)
            {
                try
                {
                    CleanPatient(app.OpenPatient(patientSummary));
                    app.ClosePatient();
                }
                catch (Exception e)
                {
                    Logger.Write(this, e.Message);
                    break;
                }

                ++i;
                if (i > 100)
                {
                    //break;
                }
            }
            Logger.Write(this, $"Can: {iCan} / Can't: {iCant}");
        }

        private void CleanPatient(Patient patient)
        {
            

            patient.BeginModifications();
            Logger.Write(this, String.Format("Patient: {0}", patient.Id));
            foreach (var ss in patient.StructureSets)
            {
                Logger.Write(this, String.Format("\tStructureSet: {0}/{1} Image {2}", ss.Id, ss.Name, ss?.Image.Id));
                if (patient.CanRemoveEmptyPhantom(ss, out string errorMessage))
                {
                    ++iCan;
                    Logger.Write(this, "Can");
                }
                else
                {
                    ++iCant;
                    Logger.Write(this, "\t\t" + errorMessage);
                }
            }
            
        }

        public void Dispose()
        {
            app?.Dispose();
        }
    }
}
