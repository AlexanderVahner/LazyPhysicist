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
    public sealed class PlanCachedModel : PlanBaseModel, IPlanCachedModel
    {
        private const double ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF = 0.7;

        private readonly App.AppContext context;
        private readonly CachedPlan cachedPlan;
        private ObservableCollection<IStructureModel> structures;
        private List<CachedObjective> cachedObjectives;
        private CachedNto cachedNto;
        private NtoInfo ntoInfo;
        public PlanCachedModel(CachedPlan cachedPlan, App.AppContext context)
        {
            if (cachedPlan == null || context?.CurrentPlan == null)
            {
                Logger.Write(this, "Can't create a PlanModel - Cached plan or Context is NULL", LogMessageType.Error);
                return;
            }
            this.cachedPlan = cachedPlan;
            this.context = context;
        }
        protected override ObservableCollection<IStructureModel> GetStructures()
        {
            if (structures == null)
            {
                structures = new ObservableCollection<IStructureModel>();
                LoadCachedObjectivesIntoStructures();
                MatchStructures(context.CurrentPlan.Structures.OrderBy(s => s.Id));
            }
            return structures;
        }
        protected override ObservableCollection<IStructureInfo> GetStructureSuggestions()
        {
            throw new NotImplementedException();
        }
        protected override INtoInfo GetNto()
        {
            if (ntoInfo == null)
            {
                cachedNto = context.PlansContext.GetNto(cachedPlan.RowId);
                ntoInfo = new NtoInfo();
                if (cachedPlan != null)
                {
                    ntoInfo.IsAutomatic = cachedNto.IsAutomatic;
                    ntoInfo.DistanceFromTargetBorderInMM = cachedNto.DistanceFromTargetBorderInMM ?? 0;
                    ntoInfo.StartDosePercentage = cachedNto.StartDosePercentage ?? 0;
                    ntoInfo.EndDosePercentage = cachedNto.EndDosePercentage ?? 0;
                    ntoInfo.FallOff = cachedNto.FallOff ?? 0;
                    ntoInfo.Priority = cachedNto.Priority ?? 0;
                };
            }
            return ntoInfo;
        }
        public override string PlanTitle => $"({cachedPlan.PatientId}).[{cachedPlan.CourseId}].{cachedPlan.PlanId}";
        public DateTime CreationDate => cachedPlan.CreationDate;
        public List<CachedObjective> CachedObjectives => GetCachedObjectives();
        public string Description
        {
            get => cachedPlan.Description;
            set
            {
                cachedPlan.Description = value;
                context.PlansContext.UpdatePlan(cachedPlan);
            }
        }
        public long? SelectionFrequency
        {
            get => cachedPlan?.SelectionFrequency;
            set
            {
                cachedPlan.SelectionFrequency = value;
                context.PlansContext.UpdatePlan(cachedPlan);
            }
        }
        private List<CachedObjective> GetCachedObjectives()
        {
            if (cachedObjectives == null && cachedPlan != null)
            {
                cachedObjectives = context.PlansContext.GetObjectives(cachedPlan.RowId);
            }
            return cachedObjectives;
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
                structures.Add(s);
            }
        }
        private void MatchStructures(IEnumerable<IStructureInfo> structureSuggestions)
        {
            List<IStructureInfo> suggestions = new List<IStructureInfo>();
            foreach (IStructureInfo structure in structureSuggestions)
            {
                if (structure.CanOptimize)
                {
                    suggestions.Add(structure);
                }
            }

            if (structures.Count == 0 || suggestions.Count == 0)
            {
                return;
            }

            List<StructuresComparsion> comparsion = GetComparsionTable(suggestions);

            foreach (StructuresComparsion sc in comparsion.OrderBy(c => c.Distance))
            {
                if (sc.StructureModel?.CurrentPlanStructure?.Structure == null
                    && sc.CurrentPlanStructure?.Structure != null
                    && suggestions.Contains(sc.CurrentPlanStructure)
                    && (sc.Distance < (sc.CurrentPlanStructure.Id.Length * ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF)))
                {
                    sc.StructureModel.CurrentPlanStructure = sc.CurrentPlanStructure;
                    suggestions.Remove(sc.CurrentPlanStructure);
                    if (suggestions.Count == 0 || structures.Count(s => s.CurrentPlanStructure?.Structure == null) == 0)
                    {
                        break;
                    }
                }
            }
            comparsion.Clear();
        }
        private List<StructuresComparsion> GetComparsionTable(List<IStructureInfo> suggestions)
        {
            List<StructuresComparsion> comparsion = new List<StructuresComparsion>(structures.Count * suggestions.Count);
            foreach (IStructureModel s in structures)
            {
                foreach (IStructureInfo s_api in suggestions)
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
