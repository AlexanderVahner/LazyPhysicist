using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyOptimizer.UI.ViewModels
{
    public class ObjectiveVM : ViewModel
    {
        public ObjectiveInfo GetObjectiveInfo(Structure structure)
        {
            ObjectiveInfo result = null;
            if (ObjectiveDB == null)
            {
                Logger.Write(this, "Objective View Model doesn't have an ObjectiveDBRecord.", LogMessageType.Error);
            }
            else
            {
                result = new ObjectiveInfo()
                {
                    Type = (ObjectiveType)(ObjectiveDB.ObjType ?? 99),
                    Structure = structure,
                    StructureId = ObjectiveDB.StructureId,
                    Priority = Priority,
                    Operator = (OptimizationObjectiveOperator)ObjectiveDB.Operator,
                    Dose = ObjectiveDB.Dose ?? .0,
                    Volume = ObjectiveDB.Volume ?? .0,
                    ParameterA = ObjectiveDB.ParameterA ?? .0
                };
            }

            return result;
        }

        public void ResetPriority()
        {
            Priority = objectiveDB?.Priority ?? 0;
        }

        private ObjectiveDBRecord objectiveDB;
        public ObjectiveDBRecord ObjectiveDB
        {
            get => objectiveDB;
            set
            {
                SetProperty(ref objectiveDB, value);
                ResetPriority();
            }
        }
        
        private double priority;
        public double Priority
        {
            get => priority;
            set => SetProperty(ref priority, value);
        }

        public string Info => $"{ObjectiveDB.Dose} {ObjectiveDB.Volume} {ObjectiveDB.ParameterA}";
    }
}
