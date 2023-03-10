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
using System.Windows.Data;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.UI.ViewModels
{
    public class HabitsVM : ViewModel
    {
        private PlanInfo currentPlan;
        private PlanVM selectedDBPlan;

        public void UpdatePlans(IEnumerable<PlanDBRecord> dbPlans)
        {
            Plans.Clear();
            if ((dbPlans?.Count() ?? 0) > 0)
            {
                PlanVM planVM;
                foreach (PlanDBRecord plan in dbPlans)
                {
                    planVM = new PlanVM() { DBPlan = plan };
                    Plans.Add(planVM);
                }
            }
        }

        public PlanInfo CurrentPlan
        {
            get => currentPlan;
            set => SetProperty(ref currentPlan, value);
        }
        public List<PlanDBRecord> DBPlans { get; set; }
        public List<ObjectiveDBRecord> DBObjectives { get; set; }
        public PlanVM SelectedDBPlan
        {
            get => selectedDBPlan;
            set
            {
                if (!Equals(selectedDBPlan, value))
                {
                    if (selectedDBPlan != null)
                    {
                        selectedDBPlan.StructureSuggestions.CollectionChanged -= StructureSuggestions_CollectionChanged;
                    }
                    UnusedStructures.Clear();

                    SetProperty(ref selectedDBPlan, value);
                    
                    // This event updates DBPlan objectives from DB, if necessary
                    SelectedDBPlanChanged?.Invoke(this, selectedDBPlan);
                    // And then they'll be ready to be loaded
                    if (selectedDBPlan != null)
                    {
                        selectedDBPlan.LoadStructures(currentPlan);
                        foreach (StructureInfo unusedStructure in selectedDBPlan.StructureSuggestions)
                        {
                            if (unusedStructure.Structure != null)
                            {
                                UnusedStructures.Add(unusedStructure);
                            }
                            
                        }
                        selectedDBPlan.StructureSuggestions.CollectionChanged += StructureSuggestions_CollectionChanged;
                    }
                }
                
            }
        }

        private void StructureSuggestions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<StructureInfo>)
            {
                if (e.NewItems != null)
                {
                    foreach (StructureInfo newStructure in e.NewItems)
                    {
                        if (newStructure.Structure != null)
                        {
                            UnusedStructures.Add(newStructure);
                        }
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (StructureInfo oldStructure in e.OldItems)
                    {
                        UnusedStructures.Remove(oldStructure);
                    }
                }
            }
        }

        public ObservableCollection<PlanVM> Plans { get; set; } = new ObservableCollection<PlanVM>();
        public ObservableCollection<StructureInfo> UnusedStructures { get; } = new ObservableCollection<StructureInfo>();

        private string prioritySetter;
        public string PrioritySetter
        {
            get => prioritySetter;
            set => SetProperty(ref prioritySetter, value);
        }

        private bool addNto = true;
        public bool AddNto
        {
            get => addNto;
            set => SetProperty(ref addNto, value);
        }

        public delegate void SelectedPlanChangedEventHandler(object sender, PlanVM dbPlan);
        public event SelectedPlanChangedEventHandler SelectedDBPlanChanged;

        public MetaCommand LoadIntoPlan => new MetaCommand(
                o =>
                {
                    if (SelectedDBPlan != null)
                    {
                        List<ObjectiveInfo> objectives = new List<ObjectiveInfo>();

                        SelectedDBPlan.Structures
                            .Where(s => s.APIStructure != null && s.Objectives.Count() > 0)
                            .ToList()
                            .ForEach(
                                s => s.Objectives
                                    .Where(obj => obj.ObjectiveDB != null)
                                    .ToList()
                                    .ForEach(obj => objectives.Add(obj.GetObjectiveInfo(s.APIStructure.Structure))));

                        if (objectives.Count > 0)
                        {
                            LoadIntoPlanClick?.Invoke(this, objectives);
                            if (AddNto)
                            {
                                LoadNtoIntoPlanClick?.Invoke(this, SelectedDBPlan.Nto.APINto);
                            }
                        }
                        
                    }
                    
                },
                o => SelectedDBPlan != null && SelectedDBPlan.Structures.FirstOrDefault(s => s.APIStructure.Structure != null) != null
            );

        public MetaCommand SetOarsPriority => new MetaCommand(
                priorityString =>
                {
                    double priority;
                    if (SelectedDBPlan != null)
                    {
                        if (double.TryParse(priorityString as string, out priority))
                        {
                            SelectedDBPlan.Structures
                                .Where(s => !s.IsTarget)
                                .ToList()
                                .ForEach(
                                    s => s.Objectives.ToList().ForEach(o =>
                                        {
                                            if (priority == -1)
                                            {
                                                o.ResetPriority();
                                            }
                                            else
                                            {
                                                o.Priority = priority;
                                            }
                                        }));
                        }
                    }

                },
                o => SelectedDBPlan != null && SelectedDBPlan.Structures.Count > 0
            );

        public delegate void LoadIntoPlanClickEventHandler(object sender, List<ObjectiveInfo> objectives);
        public event LoadIntoPlanClickEventHandler LoadIntoPlanClick;

        public delegate void LoadNtoIntoPlanClickEventHandler(object sender, NtoInfo nto);
        public event LoadNtoIntoPlanClickEventHandler LoadNtoIntoPlanClick;
    }
}
