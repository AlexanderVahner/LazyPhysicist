using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.ESAPI;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
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
        private PlanVM selectedPlanVM;
        public HabitsVM(App.AppContext context)
        {
            Context = context;
        }
        private void InitializeModel(App.AppContext context)
        {
            if (context != null)
            {
                UpdatePlans(Context.PlansFilterArgs);
                context.PlansFilterArgs.UpdateRequest += (s, args) =>
                {
                    UpdatePlans(args);
                };
                NotifyPropertyChanged(nameof(LoadNto));
                NotifyPropertyChanged(nameof(PrioritySetter));
            }
        }
        public void UpdatePlans(PlansFilterArgs args)
        {
            Plans.Clear();
            var plans = Context.PlansContext.GetPlans(args);
            if ((plans?.Count() ?? 0) > 0)
            {
                foreach (var plan in plans)
                {
                    PlanVM planVM = new PlanVM(Context, plan);
                    Plans.Add(planVM);
                }

                Logger.Write(this, $"You have {plans.Count()} matched plan" + (plans.Count() == 1 ? "." : "s."));
            }
            else
            {
                Logger.Write(this, "Seems like you don't have matched plans. Maybe you need to recheck them?", LogMessageType.Warning);
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
        public PlanVM SelectedPlan
        {
            get => selectedPlanVM;
            set
            {
                if (!Equals(selectedPlanVM, value))
                {
                    if (selectedPlanVM != null)
                    {
                        selectedPlanVM.StructureSuggestions.CollectionChanged -= StructureSuggestions_CollectionChanged;
                    }
                    Structures.Clear();
                    UnusedStructures.Clear();

                    SetProperty(ref selectedPlanVM, value);

                    if (selectedPlanVM != null)
                    {
                        if (selectedPlanVM.Structures.Count > 0)
                        {
                            foreach(StructureVM structure in selectedPlanVM.Structures)
                            {
                                Structures.Add(structure);
                            }
                            foreach (StructureInfo unusedStructure in selectedPlanVM.StructureSuggestions)
                            {
                                if (unusedStructure.Structure != null)
                                {
                                    UnusedStructures.Add(unusedStructure);
                                }
                            }
                            
                        }
                        selectedPlanVM.StructureSuggestions.CollectionChanged += StructureSuggestions_CollectionChanged;
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
                //NotifyPropertyChanged(nameof(LoadNto));
            }
        }

        public string PrioritySetter
        {
            get => Context?.Settings?.DefaultPrioritySetValue ?? "";
            set
            {
                if (value == "" || (double.TryParse(value, out double dv) && dv <= 1000))
                {
                    SetProperty(v => { if (Context?.Settings?.DefaultPrioritySetValue != null) Context.Settings.DefaultPrioritySetValue = v; }, value);
                    //NotifyPropertyChanged(nameof(PrioritySetter));
                }
            }
        }
        private IEnumerable<ObjectiveInfo> GetObjectivesForPlan()
        {
            if ((SelectedPlan?.Structures.Count ?? 0) > 0)
            {
                foreach (StructureVM s in SelectedPlan.Structures)
                {
                    if (s.APIStructure != null && s.APIStructure.IsAssigned && s.Objectives.Count() > 0)
                    {

                        foreach (ObjectiveVM obj in s.Objectives)
                        {
                            if (obj.CachedObjective != null)
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
        public MetaCommand LoadIntoPlan => new MetaCommand(
            o =>
            {
                if (SelectedPlan != null)
                {
                    bool fillOnlyEmptyStructures = false;
                    MessageBoxResult answer = MessageBoxResult.Yes;
                    if (Context.CurrentPlan.ObjectivesCount > 0)
                    {
                        answer = MessageBox.Show("The plan already has Optimization Objectives.\nDo you want to add all of it?\nClick No if you want to fill only empty structures", "Do you?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (answer == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                        fillOnlyEmptyStructures = answer == MessageBoxResult.No;
                    }

                    List<ObjectiveInfo> objectives = GetObjectivesForPlan().ToList();

                    if (objectives.Count > 0)
                    {
                        PlanEdit.LoadObjectives(Context.CurrentPlan, objectives, fillOnlyEmptyStructures);
                        if (Context.Settings.LoadNto)
                        {
                            PlanEdit.LoadNtoIntoPlan(Context.CurrentPlan, SelectedPlan.NtoVM.CurrentNto);
                        }
                        SelectedPlan.CachedPlan.SelectionFrequency++;
                    }
                        
                }
                    
            },
            o => SelectedPlan != null && SelectedPlan.Structures.FirstOrDefault(s => s.APIStructure?.Structure != null) != null
        );

        
        public MetaCommand SetOarsPriority => new MetaCommand(
            priorityString =>
            {
                if (SelectedPlan != null)
                {
                    if (double.TryParse(priorityString as string, out double priority))
                    {
                        foreach (var structure in SelectedPlan.Structures)
                        {
                            if (!structure.IsTarget)
                            {
                                foreach (var objective in structure.Objectives)
                                {
                                    if (priority == -1)
                                    {
                                        objective.ResetPriority();
                                    }
                                    else
                                    {
                                        objective.Priority = priority;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Write(this, "Enter priority.", LogMessageType.Warning);
                    }
                }

            },
            o => SelectedPlan != null && SelectedPlan.Structures.Count > 0
        );
    }
}
