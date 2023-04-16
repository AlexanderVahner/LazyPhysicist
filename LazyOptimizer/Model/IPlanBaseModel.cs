using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
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
        IEnumerable<IObjectiveInfo> GetObjectiveInfos();
        ObservableCollection<IStructureModel> Structures { get; }
        ObservableCollection<IStructureSuggestionModel> StructureSuggestions { get; }
        INtoInfo NtoInfo { get; }
        string PlanTitle { get; }
    }
}
