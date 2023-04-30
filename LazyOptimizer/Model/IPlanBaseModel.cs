using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public interface IPlanBaseModel
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
