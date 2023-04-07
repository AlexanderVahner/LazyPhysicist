using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.ESAPI;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.UI.ViewModels
{
    // This class have a part in "HabitsVM" file
    public partial class HabitsVM : ViewModel
    {
        
        
        

        private void SetSelectedPlan(PlanVM planVM)
        {
            if (Equals(selectedPlanVM, planVM))
            {
                return;
            }

            if (selectedPlanVM != null)
            {
                selectedPlanVM.StructureSuggestions.CollectionChanged -= StructureSuggestions_CollectionChanged;
            }
            Structures.Clear();
            UnusedStructures.Clear();

            selectedPlanVM = planVM;

            if (selectedPlanVM != null)
            {
                if (selectedPlanVM.Structures.Count > 0)
                {
                    foreach (StructureVM structure in selectedPlanVM.Structures)
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
        private IEnumerable<ObjectiveInfo> GetObjectivesForPlan()
        {
            if ((SelectedPlan?.Structures.Count ?? 0) == 0)
            {
                yield break;
            }
            foreach (StructureVM s in SelectedPlan.Structures)
            {
                if (s.APIStructure != null && s.APIStructure.IsAssigned && s.Objectives.Count() > 0)
                {
                    foreach (ObjectiveVM obj in s.Objectives)
                    {
                        if (obj.CachedObjective != null)
                        {
                            yield return GetObjectiveInfo(obj.CachedObjective, s.APIStructure.Structure);
                        }
                    }
                }
            }
        }
        private void FillCurrentPlan()
        {
            if (SelectedPlan == null)
            {
                return;
            }

            List<ObjectiveInfo> objectives = GetObjectivesForPlan().ToList();
            if (objectives.Count == 0)
            {
                Logger.Write(this, "There are no objectives to add.", LogMessageType.Warning);
                return;
            }

            bool fillOnlyEmptyStructures = false;
            MessageBoxResult answer;
            if (context.CurrentPlan.ObjectivesCount > 0)
            {
                answer = MessageBox.Show("The plan already has Optimization Objectives.\nDo you want to add all of it?\nClick No if you want to fill only empty structures", "Do you?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (answer == MessageBoxResult.Cancel)
                {
                    return;
                }
                fillOnlyEmptyStructures = answer == MessageBoxResult.No;
            }

            PlanEdit.LoadObjectives(context.CurrentPlan, objectives, fillOnlyEmptyStructures);
            SelectedPlan.SelectionFrequency++;

            if (context.Settings.LoadNto)
            {
                PlanEdit.LoadNtoIntoPlan(context.CurrentPlan, SelectedPlan.NtoVM.CurrentNto);
            }
        }
        private void SetPriorityForOars(string priorityString)
        {
            if (SelectedPlan == null)
            {
                return;
            }
            if (!double.TryParse(priorityString, out double priority))
            {
                Logger.Write(this, "Enter priority.", LogMessageType.Warning);
                return;
            }

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
        public ObjectiveInfo GetObjectiveInfo(CachedObjective objective, Structure structure)
        {
            ObjectiveInfo result = null;
            if (objective == null)
            {
                Logger.Write(this, "Objective View Model doesn't have an CachedObjective.", LogMessageType.Error);
            }
            else
            {
                result = new ObjectiveInfo()
                {
                    Type = (ObjectiveType)(objective.ObjType ?? 99),
                    Structure = structure,
                    StructureId = objective.StructureId,
                    Priority = objective.Priority ?? .0,
                    Operator = (Operator)objective.Operator,
                    Dose = objective.Dose ?? .0,
                    Volume = objective.Volume ?? .0,
                    ParameterA = objective.ParameterA ?? .0
                };
            }

            return result;
        }
    }
}
