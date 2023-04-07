using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
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
        private ObservableCollection<IObjectiveModel> objectives;
        private IStructureInfo currentPlanStructure;
        public StructureModel(string cachedStructureId)
        {
            CachedStructureId = cachedStructureId;
        }
        public double GetObjectivesMaxDose() => objectives?.Max(o => o?.CachedObjective?.Dose ?? .0) ?? .0;
        public string CachedStructureId { get; }
        public ObservableCollection<IObjectiveModel> Objectives => objectives ?? (objectives = new ObservableCollection<IObjectiveModel>());
        public bool IsTarget => StructureInfo.IsTarget(CachedStructureId);
        public double OrderByDoseDescProperty => GetObjectivesMaxDose() + (IsTarget ? 1000 : 0);
        public IStructureInfo CurrentPlanStructure
        {
            get => currentPlanStructure;
            set => SetProperty(ref currentPlanStructure, value);
        }
    }
}
