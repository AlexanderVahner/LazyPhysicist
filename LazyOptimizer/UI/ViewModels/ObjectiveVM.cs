using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Runtime.Remoting.Contexts;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.UI.ViewModels
{
    public class ObjectiveVM : ViewModel
    {
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
            set
            {
                if (value <= 1000) 
                    SetProperty(ref priority, value);
            }
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
