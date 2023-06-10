using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.3")]
[assembly: AssemblyFileVersion("1.0.0.3")]
[assembly: AssemblyInformationalVersion("1.1")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        public Script() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context/*, System.Windows.Window window, ScriptEnvironment environment*/)
        {
            Run(new ScriptArgs()
            {
                Patient = context.Patient,
                Plan = context.ExternalPlanSetup
            });
        }

        public void Run(ScriptArgs args)
        {
            if (args.Plan == null)
            {
                MessageBox.Show("Open plan first!");
                return;
            }

            var beamsToChange = GetBeamsToChange(args.Plan);

            if (beamsToChange.Count == 0)
            {
                MessageBox.Show("The Plan doesn't have treatment beams.");
                return;
            }

            if (MessageBox.Show(ChangingBeamsToText(beamsToChange), 
                "Shall we change Field IDs?", 
                MessageBoxButton.OKCancel, 
                MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    args.Patient.BeginModifications();
                    beamsToChange.ForEach(b => b.Beam.Id = b.NewId);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private List<BeamToChange> GetBeamsToChange(ExternalPlanSetup plan)
        {
            List<BeamToChange> beamsToChange = new List<BeamToChange>();
            var beams = plan.Beams.Where(b => !b.IsSetupField).ToList();

            if (beams.Count > 0)
            {
                foreach (var beam in beams)
                {
                    beamsToChange.Add(new BeamToChange()
                    {
                        Beam = beam,
                        NewId = GenerateUniqueId(GetBaseId(beam), beamsToChange)
                    });
                }
            }

            return beamsToChange;
        }

        private string GenerateUniqueId(string baseId, List<BeamToChange> beamsToChange, int iteration = 0)
        {
            string newId = iteration == 0 ? baseId : $"{baseId}.{iteration}";
            return beamsToChange.Count(item => item.NewId == newId) == 0 ? newId : GenerateUniqueId(baseId, beamsToChange, ++iteration);
        }

        private string GetBaseId(Beam beam)
        {
            if (beam.GantryDirection == Common.Model.Types.GantryDirection.None)
            {
                return beam.ControlPoints.First().GantryAngle.ToString("F0");
            }
            return beam.GantryDirection == Common.Model.Types.GantryDirection.Clockwise ? "CW" : "CCW";
        }

        private string ChangingBeamsToText(List<BeamToChange> beamsToChange)
        {
            StringBuilder text = new StringBuilder();
            beamsToChange.ForEach(b => text.Append(b.Beam.Id).Append(" => ").Append(b.NewId).Append("\n"));
            return text.ToString();
        }

        private sealed class BeamToChange
        {
            public Beam Beam { get; set; }
            public string NewId { get; set; }
        }
    }
}

