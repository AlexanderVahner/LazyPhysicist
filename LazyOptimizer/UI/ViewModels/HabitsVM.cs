using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.App;
using LazyOptimizer.DB;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LazyOptimizer.UI.ViewModels
{
    public class HabitsVM : ViewModel
    {
        private App.AppContext context;
        private PlanVM selectedDBPlan;
        //public HabitsVM() { }
        public HabitsVM(App.AppContext context)
        {
            Context = context;
        }
        private void InitializeModel(App.AppContext context)
        {
            if (context != null)
            {
                context.DataService.PlansSelected += (s, plans) =>
                {
                    UpdatePlans(plans);
                };
                NotifyPropertyChanged(nameof(LoadNto));
                NotifyPropertyChanged(nameof(PrioritySetter));
            }
        }
        public void UpdatePlans(IEnumerable<PlanDBRecord> dbPlans)
        {
            Plans.Clear();
            if ((dbPlans?.Count() ?? 0) > 0)
            {
                foreach (PlanDBRecord plan in dbPlans)
                {
                    PlanVM planVM = new PlanVM(Context, plan);
                    Plans.Add(planVM);
                }
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
                    InitializeModel(value);
                }
            }
        }
        public PlanInfo CurrentPlan  => Context?.Plan;
        public List<PlanDBRecord> DBPlans => Context?.DataService?.DBPlans;
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
                    Structures.Clear();
                    UnusedStructures.Clear();

                    SetProperty(ref selectedDBPlan, value);

                    if (selectedDBPlan != null)
                    {
                        if (selectedDBPlan.Structures.Count > 0)
                        {
                            foreach(var structure in selectedDBPlan.Structures)
                            {
                                Structures.Add(structure);
                            }
                            foreach (StructureInfo unusedStructure in selectedDBPlan.StructureSuggestions)
                            {
                                if (unusedStructure.Structure != null)
                                {
                                    UnusedStructures.Add(unusedStructure);
                                }
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
        public ObservableCollection<StructureVM> Structures { get; } = new ObservableCollection<StructureVM>();
        public ObservableCollection<StructureInfo> UnusedStructures { get; } = new ObservableCollection<StructureInfo>();
        public bool LoadNto
        {
            get => Context?.Settings?.LoadNto ?? false;
            set
            {
                SetProperty(v => { if (Context?.Settings?.LoadNto != null) Context.Settings.LoadNto = v; }, value);
                NotifyPropertyChanged(nameof(LoadNto));
            }
        }

        public string PrioritySetter
        {
            get => Context?.Settings?.DefaultPrioritySetValue ?? "";
            set
            {
                if (value == "" || (double.TryParse(value, out double dv) && dv < 1000))
                {
                    SetProperty(v => { if (Context?.Settings?.DefaultPrioritySetValue != null) Context.Settings.DefaultPrioritySetValue = v; }, value);
                    NotifyPropertyChanged(nameof(PrioritySetter));
                }
            }
        }

        public MetaCommand LoadIntoPlan => new MetaCommand(
                o =>
                {
                    if (SelectedDBPlan != null)
                    {
                        bool fillOnlyEmptyStructures = false;
                        MessageBoxResult answer = MessageBoxResult.Yes;
                        if (Context.Plan.ObjectivesCount > 0)
                        {
                            answer = MessageBox.Show("The plan already has Optimization Objectives.\nDo you want to add all of it?\nClick No if you want to fill only empty structures", "Do you?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                            if (answer == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                            fillOnlyEmptyStructures = answer == MessageBoxResult.No;
                        }

                        /*List<ObjectiveInfo> objectives = new List<ObjectiveInfo>();

                        SelectedDBPlan.Structures
                            .Where(s => s.APIStructure != null && s.Objectives.Count() > 0)
                            .ToList()
                            .ForEach(
                                s => s.Objectives
                                    .Where(obj => obj.ObjectiveDB != null)
                                    .ToList()
                                    .ForEach(obj => objectives.Add(obj.GetObjectiveInfo(s.APIStructure.Structure))));*/
                        List<ObjectiveInfo> objectives = GetObjectivesForPlan().ToList();

                        if (objectives.Count > 0)
                        {
                            Context.Plan.LoadObjectives(objectives, fillOnlyEmptyStructures);
                            if (Context.Settings.LoadNto)
                            {
                                Context.Plan.LoadNtoIntoPlan(SelectedDBPlan.Nto.APINto);
                            }
                            Context.DataService.IncreasePlanSelectionFrequency((long)SelectedDBPlan.DBPlan.rowid);
                        }
                        
                    }
                    
                },
                o => SelectedDBPlan != null && SelectedDBPlan.Structures.FirstOrDefault(s => s.APIStructure.Structure != null) != null
            );

        private IEnumerable<ObjectiveInfo> GetObjectivesForPlan()
        {
            if ((SelectedDBPlan?.Structures.Count ?? 0) > 0)
            {
                foreach (var s in SelectedDBPlan.Structures)
                {
                    if (s.APIStructure != null && s.Objectives.Count() > 0)
                    {
                        
                        foreach (var obj in s.Objectives)
                        {
                            if (obj.ObjectiveDB != null)
                            {
                                yield return obj.GetObjectiveInfo(s.APIStructure.Structure);
                            }
                        }
                    }
                }
            }
            else
            {
                yield break;
            }
        }
        public MetaCommand SetOarsPriority => new MetaCommand(
                priorityString =>
                {
                    if (SelectedDBPlan != null)
                    {
                        if (double.TryParse(priorityString as string, out double priority))
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
                        else
                        {
                            Logger.Write(this, "Enter priority.", LogMessageType.Warning);
                        }
                    }

                },
                o => SelectedDBPlan != null && SelectedDBPlan.Structures.Count > 0
            );
    }
}
