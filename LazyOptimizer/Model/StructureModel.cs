using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyOptimizer.Model
{
    public sealed class StructureModel : Notifier, IStructureModel
    {
        private ObservableCollection<IObjectiveModel> objectives;
        private IStructureSuggestionModel currentPlanStructure;
        private readonly StructuresBroker structuresBroker;

        public StructureModel(string cachedStructureId, StructuresBroker structuresBroker)
        {
            CachedStructureId = cachedStructureId;
            this.structuresBroker = structuresBroker;
        }

        public void AddObjective(CachedObjective objective)
        {
            Objectives.Add(new ObjectiveModel(objective));
        }

        public void AddObjective(IObjectiveModel objective)
        {
            Objectives.Add(objective);
        }

        public IEnumerable<IObjectiveInfo> GetObjectiveInfos()
        {
            if (CurrentPlanStructure?.StructureInfo == null || Objectives.Count == 0)
            {
                yield break;
            }
            foreach (var objective in Objectives)
            {
                var info = objective.GetObjectiveInfo(CurrentPlanStructure.StructureInfo);
                if (info == null)
                {
                    continue;
                }
                yield return info;
            }
        }

        public override string ToString()
        {
            return $"{CachedStructureId}->{CurrentPlanStructure}";
        }

        public ObservableCollection<IStructureSuggestionModel> StructureSuggestions => structuresBroker.StructureSuggestions;
        public double GetObjectivesMaxDose() => (objectives?.Count() ?? 0) > 0 ? objectives?.Max(o => o.Dose ?? .0) ?? .0 : 0;
        public string CachedStructureId { get; }
        public ObservableCollection<IObjectiveModel> Objectives => objectives ?? (objectives = new ObservableCollection<IObjectiveModel>());
        public bool IsTarget => StructureInfo.IsTarget(CachedStructureId);
        public double OrderByDoseDescProperty => GetObjectivesMaxDose() + (IsTarget ? 1000 : 0);
        public IStructureSuggestionModel CurrentPlanStructure
        {
            get => currentPlanStructure;
            set
            {
                if (value == null || Equals(currentPlanStructure, value))
                {
                    return;
                }
                structuresBroker.Give(currentPlanStructure);
                structuresBroker.Take(value);
                SetProperty(ref currentPlanStructure, value);
            }
        }
    }
}
