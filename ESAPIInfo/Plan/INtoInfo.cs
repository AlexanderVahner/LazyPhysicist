using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Plan
{
    public interface INtoInfo
    {
        bool IsAutomatic { get; set; }
        double DistanceFromTargetBorderInMM { get; set; }
        double StartDosePercentage { get; set; }
        double EndDosePercentage { get; set; }
        double FallOff { get; set; }
        double Priority { get; set; }
    }
}
