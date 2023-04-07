using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public sealed class NtoModel : Notifier, INtoModel
    {
        private CachedNto cachedNto;
        public NtoModel(CachedNto cachedNto)
        {
            this.cachedNto = cachedNto;
        }
        public CachedNto CachedNto
        {
            get => cachedNto;
            set
            {
                SetProperty(ref cachedNto, value);
                NotifyPropertyChanged("NtoInfo");
            }
        }
        private INtoInfo ntoInfo;
        public INtoInfo NtoInfo
        {
            get
            {
                if (ntoInfo == null && cachedNto != null)
                {
                    ntoInfo = new NtoInfo()
                    {
                        IsAutomatic = cachedNto.IsAutomatic,
                        DistanceFromTargetBorderInMM = cachedNto.DistanceFromTargetBorderInMM ?? 0,
                        StartDosePercentage = cachedNto.StartDosePercentage ?? 0,
                        EndDosePercentage = cachedNto.EndDosePercentage ?? 0,
                        FallOff = cachedNto.FallOff ?? 0,
                        Priority = cachedNto.Priority ?? 0
                    };
                }
                return ntoInfo;
            }
        }
        public string NtoString => NtoInfo == null ? "Not set" :
            "NTO: " + (NtoInfo.IsAutomatic ? 
            $"Automatic, Priority: {NtoInfo.Priority}" : 
            $"Manual, Priority: {NtoInfo.Priority}, {NtoInfo.DistanceFromTargetBorderInMM}mm, {NtoInfo.StartDosePercentage}%=>{NtoInfo.EndDosePercentage}%, f={NtoInfo.FallOff}");
        public override string ToString()
        {
            return NtoString;
        }
    }
}
