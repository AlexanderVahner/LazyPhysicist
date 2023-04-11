using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;

namespace LazyOptimizer.UI.ViewModels
{
    public class NtoVM : ViewModel
    {
        private INtoInfo nto;
        public INtoInfo Nto
        {
            get
            {
                if (nto == null)
                {
                    nto = new NtoInfo();
                }
                return nto;
            }
            set => SetProperty(ref nto, value);
        }
        public string NtoString => Nto == null ? "Not set" :
            "NTO: " + (Nto.IsAutomatic ? $"Automatic, Priority: {Nto.Priority}" : $"Manual, Priority: {Nto.Priority}, {Nto.DistanceFromTargetBorderInMM}mm, {Nto.StartDosePercentage}%=>{Nto.EndDosePercentage}%, f={Nto.FallOff}");
        public override string ToString()
        {
            return NtoString;
        }
    }
}
