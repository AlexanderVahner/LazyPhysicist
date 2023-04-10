using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyOptimizer.UI.ViewModels
{
    // This class have a part in "PlanVM" file
    public partial class PlanVM : ViewModel
    {

        private void LoadStructures()
        {
            if ((structures?.Count ?? 0) == 0 && context.CurrentPlan != null && ObjectivesCache != null)
            {
                LoadCurrentPlanStructuresToSuggestions(context.CurrentPlan.Structures.OrderBy(s => s.Id));
                LoadCachedObjectivesIntoStructures();
                MatchStructures();
                StructureSuggestions.Insert(0, new StructureInfo()); // Adding <none> Item as a suggestion
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
            StructureVM structureVM = null;
            List<StructureVM> tempStructures = new List<StructureVM>();
            foreach (CachedObjective cachedObjective in ObjectivesCache.OrderBy(o => o.StructureId))
            {
                if (!Equals(cachedObjective.StructureId, structureId) && (cachedObjective.StructureId ?? "") != "")
                {
                    structureVM = new StructureVM() { DBStructureId = cachedObjective.StructureId, StructureSuggestions = StructureSuggestions };
                    tempStructures.Add(structureVM);
                }
                structureVM?.Objectives.Add(new ObjectiveVM() { CachedObjective = cachedObjective });
                structureId = cachedObjective.StructureId;
            }

            foreach (StructureVM s in tempStructures.OrderByDescending(s => s.OrderByDoseDescProperty))
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
                if (sc.StructureVM?.APIStructure?.Structure == null
                    && sc.APIStructure?.Structure != null
                    && StructureSuggestions.Contains(sc.APIStructure)
                    && (sc.Distance < (sc.APIStructure.Id.Length * ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF)))
                {
                    sc.StructureVM.APIStructure = sc.APIStructure; // StructureVM removes structure from StructureSuggestions in APIStructure property, when it assigned
                    if (StructureSuggestions.Count == 0 || Structures.Count(s => s.APIStructure?.Structure == null) == 0)
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
            foreach (StructureVM s in Structures)
            {
                foreach (StructureInfo s_api in StructureSuggestions)
                {
                    string s1, s2;
                    s1 = s.DBStructureId.Replace(" ", "").Replace("_", "");
                    s2 = s_api.Id.Replace(" ", "").Replace("_", "");
                    comparsion.Add(new StructuresComparsion(s, s_api, Levenshtein.ComputeDistance(s1, s2)));
                }
            }
            return comparsion;
        }
        private struct StructuresComparsion
        {
            public StructuresComparsion(StructureVM structureVM, StructureInfo apiStructure, int distance)
            {
                StructureVM = structureVM;
                APIStructure = apiStructure;
                Distance = distance;
            }
            public StructureVM StructureVM;
            public StructureInfo APIStructure;
            public int Distance;
        }

        private List<CachedObjective> GetCachedObjectives()
        {
            if (objectivesCache == null && cachedPlan != null)
            {
                objectivesCache = context.PlansContext.GetObjectives(CachedPlan.RowId);
            }
            return objectivesCache;
        }
        private ObservableCollection<StructureVM> GetStructureVMs()
        {
            if (structures == null)
            {
                structures = new ObservableCollection<StructureVM>();
                LoadStructures();
            }
            return structures;
        }
        private NtoVM GetNtoVM()
        {
            if (ntoVM == null)
            {
                ntoVM = new NtoVM();
                if (cachedPlan != null)
                {
                    ntoVM.CachedNto = context.PlansContext.GetNto(CachedPlan.RowId);
                }
            }
            return ntoVM;
        }
    }
}
