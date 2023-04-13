using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public sealed class StructureModel : Notifier, IStructureModel
    {
        private List<IObjectiveModel> objectives;
        private IStructureSuggestionModel currentPlanStructure;
        public StructureModel(string cachedStructureId, ObservableCollection<IStructureSuggestionModel> structureSuggestions)
        {
            CachedStructureId = cachedStructureId;
            StructureSuggestions = structureSuggestions;

        }
        public void AddObjective(CachedObjective objective)
        {
            Objectives.Add(new ObjectiveModel(objective));
        }
        public void AddObjective(IObjectiveModel objective)
        {
            Objectives.Add(objective);
        }
        public ObservableCollection<IStructureSuggestionModel> StructureSuggestions { get; }
        public double GetObjectivesMaxDose() => objectives?.Max(o => o.Dose ?? .0) ?? .0;
        public string CachedStructureId { get; }
        public List<IObjectiveModel> Objectives => objectives ?? (objectives = new List<IObjectiveModel>());
        public bool IsTarget => StructureInfo.IsTarget(CachedStructureId);
        public double OrderByDoseDescProperty => GetObjectivesMaxDose() + (IsTarget ? 1000 : 0);
        public IStructureSuggestionModel CurrentPlanStructure
        {
            get => currentPlanStructure;
            set
            {
                if (value != null && !Equals(currentPlanStructure, value))
                {
                    if (currentPlanStructure?.StructureInfo != null)
                    {
                        StructureSuggestions.Insert(1, currentPlanStructure); // Insertion into postion 1 because it must be under <none> element
                    }

                    if (value?.StructureInfo != null)
                    {
                        StructureSuggestions.Remove(value);
                    }
                    SetProperty(ref currentPlanStructure, value);
                }
            }
        }
    }
}
