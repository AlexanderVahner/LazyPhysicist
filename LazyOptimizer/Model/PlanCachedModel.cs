using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public PlanCachedModel(CachedPlan cachedPlan, PlanInteractions planInteractions, App.AppContext context) : base(context.CurrentPlan, planInteractions)
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
                MatchStructures();
            }
            return structures;
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
                    structureModel = new StructureModel(cachedObjective.StructureId, StructuresBroker);
                    tempStructures.Add(structureModel);
                }
                structureModel.AddObjective(cachedObjective);
                structureId = cachedObjective.StructureId;
            }

            foreach (IStructureModel s in tempStructures.OrderByDescending(s => s.OrderByDoseDescProperty))
            {
                structures.Add(s);
            }
        }

        private void MatchStructures()
        {
            if (structures.Count == 0 || UndefinedStructures.Count == 0)
            {
                return;
            }

            List<StructuresComparsion> comparsion = GetComparsionTable(UndefinedStructures);

            foreach (StructuresComparsion sc in comparsion.OrderBy(c => c.Distance))
            {
                if (sc.StructureModel?.CurrentPlanStructure?.StructureInfo == null
                    && sc.CurrentPlanStructure?.StructureInfo != null
                    && UndefinedStructures.Contains(sc.CurrentPlanStructure)
                    && (sc.Distance < (sc.CurrentPlanStructure.Id.Length * ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF)))
                {
                    sc.StructureModel.CurrentPlanStructure = sc.CurrentPlanStructure; // StructureModel.CurrentPlanStructure removes assigned value from StructureSuggestions in setter
                    if (UndefinedStructures.Count == 0 || structures.Count(s => s.CurrentPlanStructure?.StructureInfo == null) == 0)
                    {
                        break;
                    }
                }
            }
            comparsion.Clear();
        }

        private List<StructuresComparsion> GetComparsionTable(IEnumerable<IStructureSuggestionModel> suggestions)
        {
            List<StructuresComparsion> comparsion = new List<StructuresComparsion>(structures.Count * suggestions.Count());
            foreach (IStructureModel s in structures)
            {
                foreach (IStructureSuggestionModel s_api in suggestions)
                {
                    string s1, s2;
                    s1 = s.CachedStructureId.Replace(" ", "").Replace("_", "").ToUpper();
                    s2 = s_api.Id.Replace(" ", "").Replace("_", "").ToUpper();
                    comparsion.Add(new StructuresComparsion(s, s_api, Levenshtein.ComputeDistance(s1, s2)));
                }
            }
            return comparsion;
        }

        private struct StructuresComparsion
        {
            public StructuresComparsion(IStructureModel structureModel, IStructureSuggestionModel currentPlanStructure, int distance)
            {
                StructureModel = structureModel;
                CurrentPlanStructure = currentPlanStructure;
                Distance = distance;
            }
            public IStructureModel StructureModel;
            public IStructureSuggestionModel CurrentPlanStructure;
            public int Distance;
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
        protected override string GetDescription() => cachedPlan.Description;
        protected override void SetDescription(string text)
        {
            cachedPlan.Description = text;
            context.PlansContext.UpdatePlan(cachedPlan);
            base.SetDescription(text);
        }
        protected override long GetSelectionFrequency() => cachedPlan?.SelectionFrequency ?? 0;
        protected override void SetSelectionFrequency(long value)
        {
            cachedPlan.SelectionFrequency = value;
            context.PlansContext.UpdatePlan(cachedPlan);
            base.SetSelectionFrequency(value);
        }

        public override string PlanTitle => $"({cachedPlan.PatientId}).[{cachedPlan.CourseId}].{cachedPlan.PlanId}";
        public DateTime CreationDate => cachedPlan.CreationDate;
        public List<CachedObjective> CachedObjectives => GetCachedObjectives();
        public bool IsStarred
        {
            get => cachedPlan.Starred == 1;
            set
            {
                cachedPlan.Starred = value == true ? 1 : 0;
                context.PlansContext.UpdatePlan(cachedPlan);
            }
        }

    }
}
