using ESAPIInfo.Plan;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LazyOptimizer.Model
{
    public interface IPlanBaseModel : INotifyPropertyChanged
    {
        IPlanInfo CurrentPlan { get; }
        string PlanTitle { get; }
        IEnumerable<IObjectiveInfo> GetObjectiveInfos();
        void AddToMerged();
        IStructureModel AddStructure(string id, IStructureSuggestionModel currentPlanStructure = null);
        ObservableCollection<IStructureModel> Structures { get; }
        ObservableCollection<IStructureSuggestionModel> UndefinedStructures { get; }
        INtoInfo NtoInfo { get; }
        string Description { get; set; }
        long SelectionFrequency { get; set; }
    }
}
