using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;

namespace LazyOptimizer.Model
{
    public sealed class ObjectiveModel : Notifier, IObjectiveModel
    {
        private double priority;
        private double initPriority;
        public ObjectiveModel()
        {
        
        }
        public ObjectiveModel(CachedObjective cachedObjective)
        {
            CopyFrom(cachedObjective);
        }
        public ObjectiveModel(ObjectiveModel objective)
        {
            CopyFrom(objective);
        }
        public void CopyFrom(CachedObjective cachedObjective)
        {
            if (cachedObjective == null)
            {
                return;
            }

            Dose = cachedObjective.Dose;
            Volume = cachedObjective.Volume;
            ParameterA = cachedObjective.ParameterA;
            ObjType = (ObjectiveType)(cachedObjective.ObjType ?? 99);
            Operator = (Operator)(cachedObjective.Operator ?? 99);
            Priority = cachedObjective.Priority ?? 0;

            initPriority = Priority;
        }
        public void CopyFrom(ObjectiveModel objective)
        {
            if (objective == null)
            {
                return;
            }

            Dose = objective.Dose;
            Volume = objective.Volume;
            ParameterA = objective.ParameterA;
            ObjType = objective.ObjType;
            Operator = objective.Operator;
            Priority = objective.Priority;

            initPriority = Priority;
        }
        public ObjectiveInfo GetObjectiveInfo(IStructureInfo structure)
        {
            ObjectiveInfo result = null;
            if (structure?.Structure != null)
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
            Priority = initPriority;
        }
        public double? Dose { get; set; }
        public double? Volume { get; set; }
        public double? ParameterA { get; set; }
        public ObjectiveType ObjType { get; set; }
        public Operator Operator { get; set; }
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
