using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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
        public double? Dose => ObjectiveDB?.Dose;
        public double? Volume => ObjectiveDB?.Volume;
        public double? ParameterA => ObjectiveDB?.ParameterA;
        public ObjectiveType ObjectiveDBType => (ObjectiveType)(objectiveDB?.ObjType ?? 99);
        public OptimizationObjectiveOperator ObjectiveDBOperator => (OptimizationObjectiveOperator)(objectiveDB?.Operator ?? 99);
        public string ArrowImageSource
        {
            get
            {
                string result = "/LazyOptimizer.esapi;component/UI/Views/Unknown.png";
                if (objectiveDB != null)
                {
                    if (ObjectiveDBType == ObjectiveType.Mean)
                    {
                        result = "/LazyOptimizer.esapi;component/UI/Views/Mean.png";

                    }
                    else if (ObjectiveDBType == ObjectiveType.EUD)
                    {
                        switch (ObjectiveDBOperator)
                        {
                            case OptimizationObjectiveOperator.Upper:
                                result = "/LazyOptimizer.esapi;component/UI/Views/UpperEUD.png";
                                break;
                            case OptimizationObjectiveOperator.Lower:
                                result = "/LazyOptimizer.esapi;component/UI/Views/LowerEUD.png";
                                break;
                            case OptimizationObjectiveOperator.Exact:
                                result = "/LazyOptimizer.esapi;component/UI/Views/TargetEUD.png";
                                break;
                        }
                    }
                    else if (ObjectiveDBType == ObjectiveType.Point)
                    {
                        switch (ObjectiveDBOperator)
                        {
                            case OptimizationObjectiveOperator.Upper:
                                result = "/LazyOptimizer.esapi;component/UI/Views/Upper.png";
                                break;
                            case OptimizationObjectiveOperator.Lower:
                                result = "/LazyOptimizer.esapi;component/UI/Views/Lower.png";
                                break;
                        }
                    }
                }
                return result;
            }
        }
    }
}
