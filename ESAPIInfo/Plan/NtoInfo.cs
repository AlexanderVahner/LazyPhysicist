using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Plan
{
    public class NtoInfo
    {
        public NtoInfo() { }
        public NtoInfo(OptimizationNormalTissueParameter nto)
        {
            if (nto != null)
            {
                IsAutomatic = nto.IsAutomatic;
                DistanceFromTargetBorderInMM = nto.DistanceFromTargetBorderInMM;
                StartDosePercentage = nto.StartDosePercentage;
                EndDosePercentage = nto.EndDosePercentage;
                FallOff = nto.FallOff;
                Priority = nto.Priority;
            }
        }
        public bool IsAutomatic { get; set; }
        public long IsAutomaticLong
        {
            get => IsAutomatic ? 1 : 0;
            set => IsAutomatic = value != 0;
        }
        public double DistanceFromTargetBorderInMM { get; set; }
        public double StartDosePercentage { get; set; }
        public double EndDosePercentage { get; set; }
        public double FallOff { get; set; }
        public double Priority { get; set; }
    }
}
