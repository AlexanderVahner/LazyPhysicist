using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public abstract class PlanBaseModel : IPlanBaseModel
    {
        private StructuresBroker structuresBroker;
        private string description;
        private long selectionFrequency;
        private readonly List<IStructureSuggestionModel> currentPlanStructures = new List<IStructureSuggestionModel>();
        private readonly PlanInteractions planInteractions;

        public PlanBaseModel(IPlanInfo currentPlan, PlanInteractions planInteractions)
        {
            CurrentPlan = currentPlan;
            this.planInteractions = planInteractions;
            LoadCurrentPlanStructures(currentPlan);
        }

        private void LoadCurrentPlanStructures(IPlanInfo currentPlan)
        {
            if (currentPlan == null)
            {
                return;
            }
            foreach (var structure in currentPlan.Structures.Where(s => s.CanOptimize).OrderBy(s => s.Id))
            {
                currentPlanStructures.Add(new StructureSuggestionModel(structure));
            }
        }

        public IEnumerable<IObjectiveInfo> GetObjectiveInfos()
        {
            if ((Structures?.Count ?? 0) == 0)
            {
                yield break;
            }
            foreach (var structure in Structures)
            {
                foreach (var objective in structure.GetObjectiveInfos())
                {
                    yield return objective;
                }
            }
        }

        public void AddToMerged()
        {
            planInteractions.AddToMerged(this);
        }

        public IStructureModel AddStructure(string id, IStructureSuggestionModel currentPlanStructure = null)
        {
            IStructureModel structure = Structures.FirstOrDefault(s => s.CachedStructureId == id);
            if (structure == null)
            {
                structure = new StructureModel(id, StructuresBroker);
                Structures.Add(structure);
            }
            structure.CurrentPlanStructure = currentPlanStructure;
            return structure;
        }

        protected abstract ObservableCollection<IStructureModel> GetStructures();
        protected abstract INtoInfo GetNto();
        protected virtual string GetDescription() => description;
        protected virtual void SetDescription(string text) { description = text; }
        protected virtual long GetSelectionFrequency() => selectionFrequency;
        protected virtual void SetSelectionFrequency(long value) { selectionFrequency = value; }

        public IPlanInfo CurrentPlan { get; }
        public abstract string PlanTitle { get; }
        public ObservableCollection<IStructureModel> Structures => GetStructures();
        public ObservableCollection<IStructureSuggestionModel> UndefinedStructures => StructuresBroker.UndefinedStructures;
        public StructuresBroker StructuresBroker => structuresBroker ?? (structuresBroker = new StructuresBroker(currentPlanStructures));
        public INtoInfo NtoInfo => GetNto();
        public string Description
        {
            get => GetDescription();
            set => SetDescription(value);
        }
        public long SelectionFrequency
        {
            get => GetSelectionFrequency();
            set => SetSelectionFrequency(value);
        }
    }
}
