using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.UI.ViewModels
{
    public class PlanVM : ViewModel
    {
        private const double ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF = 0.7;
        private App.AppContext context;
        private CachedPlan cachedPlan;
        private List<CachedObjective> objectivesCache;
        private ObservableCollection<StructureVM> structures;
        private ObservableCollection<StructureInfo> structureSuggestions;
        private NtoVM ntoVM;

        public PlanVM(App.AppContext context, CachedPlan cachedPlan)
        {
            this.context = context;
            this.cachedPlan = cachedPlan;
        }
        public CachedPlan CachedPlan
        {
            get => cachedPlan;
            set
            {
                SetProperty(ref cachedPlan, value);
            }
        }
        public App.AppContext Context
        {
            get => context;
            set
            {
                if (context != value)
                {
                    context = value;
                }
            }
        }
        private void LoadStructures()
        {
            if ((structures?.Count ?? 0) == 0 && context.CurrentPlan != null && ObjectivesCache != null)
            {

                foreach (StructureInfo structure in context.CurrentPlan.Structures.OrderBy(s => s.Id))
                {
                    if (structure.CanOptimize)
                    {
                        StructureSuggestions.Add(structure);
                    }
                }

                string structureId = "";
                StructureVM structureVM = null;
                List<StructureVM> tempStructures = new List<StructureVM>();
                foreach (var cachedObjective in ObjectivesCache.OrderBy(o => o.StructureId))
                {
                    if (!Equals(cachedObjective.StructureId, structureId) && (cachedObjective.StructureId ?? "") != "")
                    {
                        structureVM = new StructureVM() { DBStructureId = cachedObjective.StructureId, StructureSuggestions = StructureSuggestions };
                        tempStructures.Add(structureVM);
                    }
                    structureVM?.Objectives.Add(new ObjectiveVM() { CachedObjective = cachedObjective });
                    structureId = cachedObjective.StructureId;
                }
                tempStructures = new List<StructureVM>(tempStructures.OrderByDescending(s => s.OrderByDoseDescProperty));
                tempStructures.ForEach(s => Structures.Add(s));

                // Comparse Structures in template and Current Plan's StructureSet
                if (Structures.Count > 0 && StructureSuggestions.Count > 0)
                {
                    List<StructuresComparsion> comparsion = new List<StructuresComparsion>();
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
                    comparsion = new List<StructuresComparsion>(comparsion.OrderBy(c => c.Distance));
                    foreach (StructuresComparsion sc in comparsion)
                    {
                        if (sc.StructureVM?.APIStructure?.Structure == null
                            && sc.APIStructure?.Structure != null
                            && StructureSuggestions.Contains(sc.APIStructure)
                            && (sc.Distance < (sc.APIStructure.Id.Length * ACCEPTABLE_LEVENSTEIN_PER_STRUCUTREID_COEFF)))
                        {
                            sc.StructureVM.APIStructure = sc.APIStructure; // StructureVM removes structure from StructureSuggestions in APIStructure property, when it assigned
                            if (StructureSuggestions.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                }
                // Adding <none> Item as a suggestion
                StructureSuggestions.Insert(0, new StructureInfo());
            }
        }
        private struct StructuresComparsion
        {
            public StructuresComparsion(StructureVM s, StructureInfo s_api, int distance)
            {
                StructureVM = s;
                APIStructure = s_api;
                Distance = distance;
            }
            public StructureVM StructureVM;
            public StructureInfo APIStructure;
            public int Distance;
        }
        public string PlanName => $"({CachedPlan?.PatientId}).[{CachedPlan?.CourseId}].{CachedPlan?.PlanId}";
        public string CreationDate => CachedPlan?.CreationDate.ToString("g") ?? "";
        public string StructuresString => CachedPlan?.StructuresString;
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
                context.PlansContext.UpdatePlan(cachedPlan);
            }
        }
        public string SelectionFrequencyBackground
        {
            get
            {
                string color = "#FF4646FF";
                if (SelectionFrequency > 5)
                    color = "#FFE83C03";
                else if (SelectionFrequency > 2)
                    color = "#FFD0B13E";
                else if (SelectionFrequency > 1)
                    color = "#FFCED672";
                else if (SelectionFrequency > 0)
                    color = "#FF3E8337";
                return color;
            }
        }
        public List<CachedObjective> ObjectivesCache
        {
            get 
            {   
                if (objectivesCache == null && cachedPlan != null)
                {
                    objectivesCache = Context.PlansContext.GetObjectives(CachedPlan.RowId);
                }
                return objectivesCache;
            }
        }
        public ObservableCollection<StructureVM> Structures
        {
            get
            {
                if (structures == null)
                {
                    structures = new ObservableCollection<StructureVM>();
                    LoadStructures();
                }
                return structures;
            }
        }
        public ObservableCollection<StructureInfo> StructureSuggestions => structureSuggestions ?? (structureSuggestions = new ObservableCollection<StructureInfo>());
        public NtoVM NtoVM
        {
            get 
            {   
                if (ntoVM == null)
                {
                    ntoVM = new NtoVM();
                    if (cachedPlan != null)
                    {
                        ntoVM.CachedNto = Context.PlansContext.GetNto(CachedPlan.RowId);
                    }
}
                return ntoVM;
            }
        }
    }
}
