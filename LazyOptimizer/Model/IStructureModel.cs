using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public interface IStructureModel : INotifyPropertyChanged
    {
        void AddObjective(CachedObjective objective);
        void AddObjective(IObjectiveModel objective);
        double GetObjectivesMaxDose();
        string CachedStructureId { get; }
        List<IObjectiveModel> Objectives { get; }
        bool IsTarget { get; }
        IStructureSuggestionModel CurrentPlanStructure { get; set; }
        double OrderByDoseDescProperty { get; }
    }
}
