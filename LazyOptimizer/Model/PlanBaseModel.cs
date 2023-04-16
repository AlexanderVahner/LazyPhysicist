using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public abstract class PlanBaseModel : IPlanBaseModel
    {
        public IEnumerable<IObjectiveInfo> GetObjectiveInfos()
        {
            foreach (var structure in Structures)
            {
                foreach (var objective in structure.GetObjectiveInfos())
                {
                    yield return objective;
                }
            }
        }
        public ObservableCollection<IStructureModel> Structures => GetStructures();
        public ObservableCollection<IStructureSuggestionModel> StructureSuggestions => GetStructureSuggestions();
        public INtoInfo NtoInfo => GetNto();
        public abstract string PlanTitle { get; }
        protected abstract ObservableCollection<IStructureModel> GetStructures();
        protected abstract ObservableCollection<IStructureSuggestionModel> GetStructureSuggestions();
        protected abstract INtoInfo GetNto();
    }
}
