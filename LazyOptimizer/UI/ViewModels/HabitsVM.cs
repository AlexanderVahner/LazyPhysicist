using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.App;
using LazyOptimizer.ESAPI;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class HabitsVM : ViewModel<HabitsModel>
    {
        private readonly AppContext context;
        private PlanVM selectedPlanVM;
        private string selectedNtoString;
        public HabitsVM(HabitsModel habitsModel, AppContext context) : base(habitsModel)
        {
            this.context = context;
            Plans = new SlaveCollection<IPlanBaseModel, PlanVM>(habitsModel.PlanModels, m => new PlanVM(m), vm => vm.SourceModel);

            NotifyPropertyChanged(nameof(LoadNto));
            NotifyPropertyChanged(nameof(PrioritySetter));
        }
        private void BindCollections(PlanVM plan)
        {
            if (plan?.SourceModel == null)
            {
                Structures.BreakFree();
                UnusedStructures.BreakFree();
            }
            else
            {
                Structures.ObeyTheMaster(plan.SourceModel.Structures, m => CreateStructureVM(m), vm => vm.SourceModel);
                UnusedStructures.ObeyTheMaster(plan.SourceModel.StructureSuggestions, m => m, vm => vm);
            }
        }
        private StructureVM CreateStructureVM(IStructureModel model)
        {
            StructureVM result = new StructureVM(model);
            return result;
        }
        private void FillCurrentPlan()
        {
            if (SelectedPlan == null)
            {
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

            SourceModel.LoadObjectivesIntoCurrentPlan(SelectedPlan.SourceModel, fillOnlyEmptyStructures);

            if (context.Settings.LoadNto && SelectedPlan.Nto != null)
            {
                SourceModel.LoadNtoIntoCurrentPlan(SelectedPlan.Nto);
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

            foreach (var structure in Structures)
            {
                if (structure.IsTarget)
                {
                    continue;
                }
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
        public void UpdateNto(INtoInfo nto)
        {
            if (nto == null)
            {
                SelectedNtoString = "Load NTO";
                return;
            }

            if (nto.IsAutomatic)
            {
                SelectedNtoString = $"NTO: Automatic, Priority: {nto.Priority}";
            }
            else
            {
                SelectedNtoString = $"NTO: Manual, Priority: {nto.Priority}, {nto.DistanceFromTargetBorderInMM}mm, {nto.StartDosePercentage}%=>{nto.EndDosePercentage}%, f={nto.FallOff}";
            }
        }
        public SlaveCollection<IPlanBaseModel, PlanVM> Plans { get; }
        public SlaveCollection<IStructureModel, StructureVM> Structures { get; } = new SlaveCollection<IStructureModel, StructureVM> { };
        public SlaveCollection<IStructureSuggestionModel, IStructureSuggestionModel> UnusedStructures { get; } = new SlaveCollection<IStructureSuggestionModel, IStructureSuggestionModel> { };
        public PlanVM SelectedPlan
        {
            get => selectedPlanVM;
            set
            {
                BindCollections(value);
                SetProperty(ref selectedPlanVM, value);
                UpdateNto(value?.SourceModel.NtoInfo);
            }
        }
        public string SelectedNtoString { get => selectedNtoString; private set => SetProperty(ref selectedNtoString, value); }
        public bool LoadNto
        {
            get => context?.Settings?.LoadNto ?? false;
            set => SetProperty(v => { if (context?.Settings?.LoadNto != null) context.Settings.LoadNto = v; }, value);
        }
        public string PrioritySetter
        {
            get => context?.Settings?.DefaultPrioritySetValue ?? "";
            set
            {
                if (value == "" || (double.TryParse(value, out double dv) && dv <= 1000))
                {
                    SetProperty(v => { if (context?.Settings?.DefaultPrioritySetValue != null) context.Settings.DefaultPrioritySetValue = v; }, value);
                }
            }
        }
        public MetaCommand LoadIntoPlan => new MetaCommand(
            o => FillCurrentPlan(),
            o => Structures.Count > 0
        );
        public MetaCommand SetOarsPriority => new MetaCommand(
            priorityString => SetPriorityForOars(priorityString as string),
            o => Structures.Count > 0
        );

        
    }
}
