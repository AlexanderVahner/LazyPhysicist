using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Plan
{
    public class NtoInfo : INtoInfo
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
        public bool IsAutomatic { get; set; } = true;
        public double DistanceFromTargetBorderInMM { get; set; } = 0.5;
        public double StartDosePercentage { get; set; } = 105.0;
        public double EndDosePercentage { get; set; } = 60.0;
        public double FallOff { get; set; } = 0.05;
        public double Priority { get; set; } = 100.0;
    }
}
