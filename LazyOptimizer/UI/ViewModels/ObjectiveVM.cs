using ESAPIInfo.Plan;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            if (CachedObjective == null)
            {
                Logger.Write(this, "Objective View Model doesn't have an ObjectiveDBRecord.", LogMessageType.Error);
            }
            else
            {
                result = new ObjectiveInfo()
                {
                    Type = (ObjectiveType)(CachedObjective.ObjType ?? 99),
                    Structure = structure,
                    StructureId = CachedObjective.StructureId,
                    Priority = Priority,
                    Operator = (Operator)CachedObjective.Operator,
                    Dose = CachedObjective.Dose ?? .0,
                    Volume = CachedObjective.Volume ?? .0,
                    ParameterA = CachedObjective.ParameterA ?? .0
                };
            }

            return result;
        }

        public void ResetPriority()
        {
            Priority = cachedObjective?.Priority ?? 0;
        }

        private CachedObjective cachedObjective;
        public CachedObjective CachedObjective
        {
            get => cachedObjective;
            set
            {
                SetProperty(ref cachedObjective, value);
                ResetPriority();
            }
        }
        
        private double priority;
        public double Priority
        {
            get => priority;
            set => SetProperty(ref priority, value);
        }

        public string Info => $"{CachedObjective.Dose} {CachedObjective.Volume} {CachedObjective.ParameterA}";
        public double? Dose => CachedObjective?.Dose;
        public double? Volume => CachedObjective?.Volume;
        public double? ParameterA => CachedObjective?.ParameterA;
        public ObjectiveType ObjectiveDBType => (ObjectiveType)(cachedObjective?.ObjType ?? 99);
        public Operator ObjectiveDBOperator => (Operator)(cachedObjective?.Operator ?? 99);
        public string ArrowImageSource
        {
            get
            {
                string result = "/LazyOptimizer.esapi;component/UI/Views/Unknown.png";
                if (cachedObjective != null)
                {
                    if (ObjectiveDBType == ObjectiveType.Mean)
                    {
                        result = "/LazyOptimizer.esapi;component/UI/Views/Mean.png";

                    }
                    else if (ObjectiveDBType == ObjectiveType.EUD)
                    {
                        switch (ObjectiveDBOperator)
                        {
                            case Operator.Upper:
                                result = "/LazyOptimizer.esapi;component/UI/Views/UpperEUD.png";
                                break;
                            case Operator.Lower:
                                result = "/LazyOptimizer.esapi;component/UI/Views/LowerEUD.png";
                                break;
                            case Operator.Exact:
                                result = "/LazyOptimizer.esapi;component/UI/Views/TargetEUD.png";
                                break;
                        }
                    }
                    else if (ObjectiveDBType == ObjectiveType.Point)
                    {
                        switch (ObjectiveDBOperator)
                        {
                            case Operator.Upper:
                                result = "/LazyOptimizer.esapi;component/UI/Views/Upper.png";
                                break;
                            case Operator.Lower:
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
