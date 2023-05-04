using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LazyOptimizer.Model
{
    public interface IStructureModel : INotifyPropertyChanged
    {
        void AddObjective(CachedObjective objective);
        void AddObjective(IObjectiveModel objective);
        IEnumerable<IObjectiveInfo> GetObjectiveInfos();
        double GetObjectivesMaxDose();
        string CachedStructureId { get; }
        ObservableCollection<IObjectiveModel> Objectives { get; }
        bool IsTarget { get; }
        IStructureSuggestionModel CurrentPlanStructure { get; set; }
        ObservableCollection<IStructureSuggestionModel> StructureSuggestions { get; }
        double OrderByDoseDescProperty { get; }
    }
}
