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
    public sealed class PlanModel : Notifier, IPlanModel
    {
        private readonly App.AppContext context;
        private readonly CachedPlan cachedPlan;
        private List<CachedObjective> cachedObjectives;
        private ObservableCollection<IStructureModel> structures;
        private ObservableCollection<IStructureInfo> structureSuggestions;
        private INtoModel ntoModel;
        public PlanModel(CachedPlan cachedPlan, App.AppContext context)
        {
            this.cachedPlan = cachedPlan;
            this.context = context;
        }
        public PlanModelType Type => PlanModelType.CachedPlan;
        public CachedPlan CachedPlan => cachedPlan;
        public string PlanName => $"({CachedPlan?.PatientId}).[{CachedPlan?.CourseId}].{CachedPlan?.PlanId}";
        public string CreationDate => CachedPlan?.CreationDate.ToString("g") ?? "";
        public List<CachedObjective> CachedObjectives => GetCachedObjectives();
        public ObservableCollection<IStructureModel> Structures => GetStructureModels();
        public ObservableCollection<IStructureInfo> StructureSuggestions => structureSuggestions ?? (structureSuggestions = new ObservableCollection<IStructureInfo>());
        public INtoModel NtoModel => GetNtoModel();
        public string Description
        {
            get => CachedPlan?.Description;
            set
            {
                SetProperty((v) => { if (CachedPlan != null) CachedPlan.Description = v; }, value);
                context.PlansContext.UpdatePlan(cachedPlan);
            }
        }
        public long? SelectionFrequency
        {
            get => CachedPlan?.SelectionFrequency;
            set
            {
                SetProperty((v) => { if (CachedPlan != null) CachedPlan.SelectionFrequency = v; }, value);
                context.PlansContext.UpdatePlan(CachedPlan);
            }
        }
        private List<CachedObjective> GetCachedObjectives()
        {
            if (cachedObjectives == null && cachedPlan != null)
            {
                cachedObjectives = context.PlansContext.GetObjectives(CachedPlan.RowId);
            }
            return cachedObjectives;
        }
        private ObservableCollection<IStructureModel> GetStructureModels()
        {
            if (structures == null)
            {
                structures = new ObservableCollection<IStructureModel>();
                LoadStructures();
            }
            return structures;
        }
        private INtoModel GetNtoModel()
        {
            if (ntoModel == null && cachedPlan != null)
            {
                ntoModel = new NtoModel(context.PlansContext.GetNto(CachedPlan.RowId));
            }
            return ntoModel;
        }
        private const double ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF = 0.7;

        private void LoadStructures()
        {
            if ((structures?.Count ?? 0) == 0 && context.CurrentPlan != null && CachedObjectives != null)
            {
                LoadCurrentPlanStructuresToSuggestions(context.CurrentPlan.Structures.OrderBy(s => s.Id));
                LoadCachedObjectivesIntoStructures();
                MatchStructures();
            }
        }
        private void LoadCurrentPlanStructuresToSuggestions(IEnumerable<StructureInfo> structuresList)
        {
            foreach (StructureInfo structure in structuresList)
            {
                if (structure.CanOptimize)
                {
                    StructureSuggestions.Add(structure);
                }
            }
        }
        private void LoadCachedObjectivesIntoStructures()
        {
            string structureId = "";
            IStructureModel structureModel = null;
            List<IStructureModel> tempStructures = new List<IStructureModel>();
            foreach (CachedObjective cachedObjective in CachedObjectives.OrderBy(o => o.StructureId))
            {
                if (!Equals(cachedObjective.StructureId, structureId) && (cachedObjective.StructureId ?? "") != "")
                {
                    structureModel = new StructureModel(cachedObjective.StructureId);
                    tempStructures.Add(structureModel);
                }
                structureModel?.Objectives.Add(new ObjectiveModel(cachedObjective));
                structureId = cachedObjective.StructureId;
            }

            foreach (IStructureModel s in tempStructures.OrderByDescending(s => s.OrderByDoseDescProperty))
            {
                Structures.Add(s);
            }
        }
        private void MatchStructures()
        {
            if (Structures.Count == 0 || StructureSuggestions.Count == 0)
            {
                return;
            }

            List<StructuresComparsion> comparsion = GetComparsionTable();

            foreach (StructuresComparsion sc in comparsion.OrderBy(c => c.Distance))
            {
                if (sc.StructureModel?.CurrentPlanStructure?.Structure == null
                    && sc.CurrentPlanStructure?.Structure != null
                    && StructureSuggestions.Contains(sc.CurrentPlanStructure)
                    && (sc.Distance < (sc.CurrentPlanStructure.Id.Length * ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF)))
                {
                    sc.StructureModel.CurrentPlanStructure = sc.CurrentPlanStructure; // StructureModel removes structure from StructureSuggestions in APIStructure property, when it assigned
                    if (StructureSuggestions.Count == 0 || Structures.Count(s => s.CurrentPlanStructure?.Structure == null) == 0)
                    {
                        break;
                    }
                }
            }
            comparsion.Clear();
        }
        private List<StructuresComparsion> GetComparsionTable()
        {
            List<StructuresComparsion> comparsion = new List<StructuresComparsion>(Structures.Count * StructureSuggestions.Count);
            foreach (IStructureModel s in Structures)
            {
                foreach (IStructureInfo s_api in StructureSuggestions)
                {
                    string s1, s2;
                    s1 = s.CachedStructureId.Replace(" ", "").Replace("_", "");
                    s2 = s_api.Id.Replace(" ", "").Replace("_", "");
                    comparsion.Add(new StructuresComparsion(s, s_api, Levenshtein.ComputeDistance(s1, s2)));
                }
            }
            return comparsion;
        }
        private struct StructuresComparsion
        {
            public StructuresComparsion(IStructureModel structureModel, IStructureInfo currentPlanStructure, int distance)
            {
                StructureModel = structureModel;
                CurrentPlanStructure = currentPlanStructure;
                Distance = distance;
            }
            public IStructureModel StructureModel;
            public IStructureInfo CurrentPlanStructure;
            public int Distance;
        }

        
    }
}
