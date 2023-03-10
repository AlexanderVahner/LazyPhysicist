using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.DB;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.UI.ViewModels
{
    public class PlanVM : ViewModel
    {
        private PlanDBRecord dbPlan;
        private List<ObjectiveDBRecord> dbObjectives;
        private ObservableCollection<StructureVM> structures;
        private ObservableCollection<StructureInfo> structureSuggestions;
        private NtoVM nto;
        public PlanVM() { }
        public PlanDBRecord DBPlan
        {
            get => dbPlan;
            set
            {
                SetProperty(ref dbPlan, value);
            }
        }
        public void LoadStructures(PlanInfo apiPlan)
        {
            if (Structures.Count == 0 && apiPlan != null && dbObjectives != null)
            {
                APIPlan = apiPlan;

                foreach (Structure structure in apiPlan.Structures.OrderBy(s => s.Id))
                {
                    StructureSuggestions.Add(new StructureInfo() { Structure = structure });
                }

                string structureId = "";
                StructureVM structureVM = null;
                List<StructureVM> tempStructures = new List<StructureVM>();
                foreach (ObjectiveDBRecord objective in dbObjectives.OrderBy(o => o.StructureId))
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
                        if (sc.StructureVM?.APIStructure?.Structure == null && sc.APIStructure?.Structure != null && StructureSuggestions.Contains(sc.APIStructure))
                        {
                            sc.StructureVM.APIStructure = sc.APIStructure;
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
        public PlanInfo APIPlan { get; set; }
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
        public List<ObjectiveDBRecord> DBObjectives => dbObjectives ?? (dbObjectives = new List<ObjectiveDBRecord>());
        public ObservableCollection<StructureVM> Structures => structures ?? (structures = new ObservableCollection<StructureVM>());
        public ObservableCollection<StructureInfo> StructureSuggestions => structureSuggestions ?? (structureSuggestions = new ObservableCollection<StructureInfo>());
        public NtoVM Nto => nto ?? (nto = new NtoVM());
    }
}
