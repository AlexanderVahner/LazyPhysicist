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
        private StructuresBroker structuresBroker;
        public PlanBaseModel(IPlanInfo currentPlan)
        {
            CurrentPlan = currentPlan;
        }
        private IEnumerable<IStructureSuggestionModel> GetStructureSuggestionModels(IPlanInfo currentPlan)
        {
            if (currentPlan == null)
            {
                yield break;
            }
            foreach (var structure in currentPlan.Structures.Where(s => s.CanOptimize).OrderBy(s => s.Id))
            {
                yield return new StructureSuggestionModel(structure);
            }
        }
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
        public IPlanInfo CurrentPlan { get; }
        public abstract string PlanTitle { get; }
        public ObservableCollection<IStructureModel> Structures => GetStructures();
        public ObservableCollection<IStructureSuggestionModel> UndefinedStructures => StructuresBroker.UndefinedStructures;
        public StructuresBroker StructuresBroker => structuresBroker ?? (structuresBroker = new StructuresBroker(GetStructureSuggestionModels(CurrentPlan)));
        public INtoInfo NtoInfo => GetNto();
        
        protected abstract ObservableCollection<IStructureModel> GetStructures();
        protected abstract INtoInfo GetNto();
    }
}
