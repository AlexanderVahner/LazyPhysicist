using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.DB;
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
        private PlanDBRecord dbPlan;
        private List<ObjectiveDBRecord> dbObjectives;
        private ObservableCollection<StructureVM> structures;
        private ObservableCollection<StructureInfo> structureSuggestions;
        private NtoVM nto;
        //public PlanVM() { }
        public PlanVM(App.AppContext context, PlanDBRecord DBPlan)
        {
            this.context = context;
            this.dbPlan = DBPlan;
            CurrentPlan = context.Plan;
        }
        public PlanDBRecord DBPlan
        {
            get => dbPlan;
            set
            {
                SetProperty(ref dbPlan, value);
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
            if (structures.Count == 0 && CurrentPlan != null && DBObjectives != null)
            {

                foreach (StructureInfo structure in CurrentPlan.Structures.OrderBy(s => s.Id))
                {
                    StructureSuggestions.Add(structure);
                }

                string structureId = "";
                StructureVM structureVM = null;
                List<StructureVM> tempStructures = new List<StructureVM>();
                foreach (ObjectiveDBRecord objective in DBObjectives.OrderBy(o => o.StructureId))
                {
                    if (!Equals(objective.StructureId, structureId) && (objective.StructureId ?? "") != "")
                    {
                        structureVM = new StructureVM() { DBStructureId = objective.StructureId, StructureSuggestions = StructureSuggestions };
                        tempStructures.Add(structureVM);
                    }
                    structureVM?.Objectives.Add(new ObjectiveVM() { ObjectiveDB = objective });
                    structureId = objective.StructureId;
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
                            comparsion.Add(new StructuresComparsion(s, s_api, Levenshtein.ComputeDistance(s.DBStructureId, s_api.Id)));
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
        public PlanInfo CurrentPlan { get; set; }
        public string PlanName => $"{DBPlan?.PatientId}/{DBPlan?.CourseId}/{DBPlan?.PlanId}";
        public string StructuresString => DBPlan?.StructuresString;
        public string Description
        {
            get => DBPlan?.DescriptionProperty;
            set => SetProperty((v) => { if (DBPlan != null) DBPlan.DescriptionProperty = v; }, value);
        }
        public long? SelectionFrequency
        {
            get => DBPlan?.SelectionFrequencyProperty;
            set => SetProperty((v) => { if (DBPlan != null) DBPlan.SelectionFrequencyProperty = v; }, value);
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
        public List<ObjectiveDBRecord> DBObjectives
        {
            get 
            {   
                if (dbObjectives == null)
                {
                    dbObjectives = new List<ObjectiveDBRecord>();
                    if (dbPlan != null)
                    {
                        Context.DataService.GetObjectives(dbObjectives, (long)DBPlan.rowid);
                    }
                }
                return dbObjectives;
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
        public NtoVM Nto
        {
            get 
            {   
                if (nto == null)
                {
                    nto = new NtoVM();
                    if (dbPlan != null)
                    {
                        nto.NtoDB = Context.DataService.GetNto((long)DBPlan.rowid);
                    }
}
                return nto;
            }
        }
    }
}
