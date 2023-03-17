using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public class PlansFilterArgs : Notifier
    {
        private string structuresString;
        private double singleDose = .0;
        private int fractionsCount = 0;
        private string technique = "";
        private string machineId = "";
        private int limit = 0;
        public string StructuresString { get => structuresString; set => SetProperty(ref structuresString, value); }
        public double SingleDose { get => singleDose; set => SetProperty(ref singleDose, value); }
        public int FractionsCount { get => fractionsCount; set => SetProperty(ref fractionsCount, value); }
        public string Technique { get => technique; set => SetProperty(ref technique, value); }
        public string MachineId { get => machineId; set => SetProperty(ref machineId, value); }
        public int Limit { get => limit; set => SetProperty(ref limit, value); }
    }
}
