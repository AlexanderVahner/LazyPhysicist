using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.Model
{
    public sealed class ObjectiveModel : Notifier, IObjectiveModel
    {
        private readonly CachedObjective cachedObjective;
        private double priority;
        public ObjectiveModel(CachedObjective cachedObjective)
        {
            this.cachedObjective = cachedObjective;
        }
        public void ResetPriority()
        {
            Priority = cachedObjective?.Priority ?? 0;
        }
        public CachedObjective CachedObjective => cachedObjective;
        public string Info => $"{CachedObjective.Dose} {CachedObjective.Volume} {CachedObjective.ParameterA}";
        public double? Dose => CachedObjective?.Dose;
        public double? Volume => CachedObjective?.Volume;
        public double? ParameterA => CachedObjective?.ParameterA;
        public ObjectiveType ObjectiveDBType => (ObjectiveType)(cachedObjective?.ObjType ?? 99);
        public Operator ObjectiveDBOperator => (Operator)(cachedObjective?.Operator ?? 99);
        public double Priority
        {
            get => priority;
            set
            {
                if (value <= 1000)
                    SetProperty(ref priority, value);
            }
        }
    }
}
