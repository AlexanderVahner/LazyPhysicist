using ESAPIInfo.Plan;
using LazyOptimizer.Model;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class ObjectiveVM : ViewModel<IObjectiveModel>
    {
        public ObjectiveVM(IObjectiveModel objectiveModel) : base(objectiveModel)
        {
            objectiveModel.PropertyChanged += (s, e) => NotifyPropertyChanged(e.PropertyName);
        }
        public void ResetPriority() => SourceModel.ResetPriority();
        public double Priority
        {
            get => SourceModel.Priority;
            set => SourceModel.Priority = value;
        }
        public double? Dose => SourceModel.Dose;
        public double? Volume => SourceModel.Volume;
        public double? ParameterA => SourceModel.ParameterA;
        public ObjectiveType ObjectiveType => SourceModel.ObjType;
        public Operator ObjectiveOperator => SourceModel.Operator;
        public string ArrowImageSource
        {
            get
            {
                string result = "/LazyOptimizer.esapi;component/UI/Views/Unknown.png";

                if (ObjectiveType == ObjectiveType.Mean)
                {
                    result = "/LazyOptimizer.esapi;component/UI/Views/Mean.png";

                }
                else if (ObjectiveType == ObjectiveType.EUD)
                {
                    switch (ObjectiveOperator)
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
                else if (ObjectiveType == ObjectiveType.Point)
                {
                    switch (ObjectiveOperator)
                    {
                        case Operator.Upper:
                            result = "/LazyOptimizer.esapi;component/UI/Views/Upper.png";
                            break;
                        case Operator.Lower:
                            result = "/LazyOptimizer.esapi;component/UI/Views/Lower.png";
                            break;
                    }
                }
                return result;
            }
        }
    }
}
