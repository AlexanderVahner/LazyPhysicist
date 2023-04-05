using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;

namespace LazyOptimizer.UI.ViewModels
{
    public class NtoVM : ViewModel
    {
        private CachedNto cachedNto;
        public CachedNto CachedNto
        {
            get => cachedNto;
            set
            {
                SetProperty(ref cachedNto, value);
                NotifyPropertyChanged("NtoInfo");
            }
        }
        private NtoInfo currentNto;
        public NtoInfo CurrentNto
        {
            get
            {
                if (currentNto == null && cachedNto != null)
                {
                    currentNto = new NtoInfo()
                    {
                        IsAutomatic = cachedNto.IsAutomatic,
                        DistanceFromTargetBorderInMM = cachedNto.DistanceFromTargetBorderInMM ?? 0,
                        StartDosePercentage = cachedNto.StartDosePercentage ?? 0,
                        EndDosePercentage = cachedNto.EndDosePercentage ?? 0,
                        FallOff = cachedNto.FallOff ?? 0,
                        Priority = cachedNto.Priority ?? 0
                    };
                }
                return currentNto;
            }
        }
        public string NtoString => CurrentNto == null ? "Not set" :
            "NTO: " + (CurrentNto.IsAutomatic ? $"Automatic, Priority: {CurrentNto.Priority}" : $"Manual, Priority: {CurrentNto.Priority}, {CurrentNto.DistanceFromTargetBorderInMM}mm, {CurrentNto.StartDosePercentage}%=>{CurrentNto.EndDosePercentage}%, f={CurrentNto.FallOff}");
        public override string ToString()
        {
            return NtoString;
        }
    }
}
