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
        private const double ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF = 0.7;

        private readonly App.AppContext context;
        private readonly CachedPlan cachedPlan;
        private List<CachedObjective> cachedObjectives;
        private List<IStructureModel> structures;
        private INtoModel ntoModel;
        public PlanBaseModel(App.AppContext context)
        {
            if (context?.CurrentPlan == null)
            {
                Logger.Write(this, "Can't create a PlanModel - Cached plan or Context is NULL", LogMessageType.Error);
                return;
            }
            Context = context;
        }
        public abstract string PlanTitle { get; }
        protected App.AppContext Context { get; }
        protected List<CachedObjective> CachedObjectives => GetCachedObjectives();
        protected List<IStructureModel> Structures => GetStructureModels();
        protected INtoModel NtoModel => GetNtoModel();
        private List<CachedObjective> GetCachedObjectives()
        {
            if (cachedObjectives == null && cachedPlan != null)
            {
                cachedObjectives = context.PlansContext.GetObjectives(cachedPlan.RowId);
            }
            return cachedObjectives;
        }
        private List<IStructureModel> GetStructureModels()
        {
            if (structures == null)
            {
                structures = new List<IStructureModel>();
                LoadStructures();
            }
            return structures;
        }
        private INtoModel GetNtoModel()
        {
            if (ntoModel == null && cachedPlan != null)
            {
                ntoModel = new NtoModel(context.PlansContext.GetNto(cachedPlan.RowId));
            }
            return ntoModel;
        }
        

        private void LoadStructures()
        {
            if ((structures?.Count ?? 0) == 0 && context.CurrentPlan != null && CachedObjectives != null)
            {
                LoadCachedObjectivesIntoStructures();
                MatchStructures(context.CurrentPlan.Structures.OrderBy(s => s.Id));
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

            if (Structures.Count == 0 || suggestions.Count == 0)
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
                    if (suggestions.Count == 0 || Structures.Count(s => s.CurrentPlanStructure?.Structure == null) == 0)
                    {
                        break;
                    }
                }
            }
            comparsion.Clear();
        }
        private List<StructuresComparsion> GetComparsionTable(List<IStructureInfo> suggestions)
        {
            List<StructuresComparsion> comparsion = new List<StructuresComparsion>(Structures.Count * suggestions.Count);
            foreach (IStructureModel s in Structures)
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
