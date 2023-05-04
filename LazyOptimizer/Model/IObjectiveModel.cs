using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using System.ComponentModel;

namespace LazyOptimizer.Model
{
    public interface IObjectiveModel : INotifyPropertyChanged
    {
        ObjectiveInfo GetObjectiveInfo(IStructureInfo structure);
        void ResetPriority();
        double Priority { get; set; }
        double? Dose { get; }
        double? Volume { get; }
        double? ParameterA { get; }
        ObjectiveType ObjType { get; }
        Operator Operator { get; }
    }
}
