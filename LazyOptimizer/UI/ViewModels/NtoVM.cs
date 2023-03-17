using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyOptimizer.UI.ViewModels
{
    public class NtoVM : ViewModel
    {
        private NtoDBRecord ntoDB;
        public NtoDBRecord NtoDB
        {
            get => ntoDB;
            set
            {
                SetProperty(ref ntoDB, value);
                NotifyPropertyChanged("NtoInfo");
            }
        }
        private NtoInfo apiNto;
        public NtoInfo APINto
        {
            get
            {
                if (apiNto == null && ntoDB != null)
                {
                    apiNto = new NtoInfo()
                    {
                        IsAutomaticLong = ntoDB.IsAutomatic,
                        DistanceFromTargetBorderInMM = ntoDB.DistanceFromTargetBorderInMM ?? 0,
                        StartDosePercentage = ntoDB.StartDosePercentage ?? 0,
                        EndDosePercentage = ntoDB.EndDosePercentage ?? 0,
                        FallOff = ntoDB.FallOff ?? 0,
                        Priority = ntoDB.Priority ?? 0
                    };
                }
                return apiNto;
            }
        }
        public string NtoInfo => APINto == null ? "Not set" :
            "NTO: " + (APINto.IsAutomatic ? $"Automatic, Priority: {APINto.Priority}" : $"Manual, Priority: {APINto.Priority}, {APINto.DistanceFromTargetBorderInMM}mm, {APINto.StartDosePercentage}%=>{APINto.EndDosePercentage}%, f={APINto.FallOff}");
    }
}
