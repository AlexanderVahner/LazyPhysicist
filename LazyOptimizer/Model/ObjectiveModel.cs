using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
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
            ResetPriority();
        }
        public ObjectiveInfo GetObjectiveInfo(IStructureInfo structure)
        {
            ObjectiveInfo result = null;
            if (cachedObjective != null && structure?.Structure != null)
            {
                result = new ObjectiveInfo()
                {
                    Type = ObjType,
                    Structure = structure.Structure,
                    StructureId = structure.Id,
                    Priority = Priority,
                    Operator = Operator,
                    Dose = Dose ?? .0,
                    Volume = Volume ?? .0,
                    ParameterA = ParameterA ?? .0
                };
            }
            return result;
        }
        public void ResetPriority()
        {
            Priority = cachedObjective?.Priority ?? 0;
        }
        public CachedObjective CachedObjective => cachedObjective;
        public double? Dose => CachedObjective?.Dose;
        public double? Volume => CachedObjective?.Volume;
        public double? ParameterA => CachedObjective?.ParameterA;
        public ObjectiveType ObjType => (ObjectiveType)(cachedObjective?.ObjType ?? 99);
        public Operator Operator => (Operator)(cachedObjective?.Operator ?? 99);
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
