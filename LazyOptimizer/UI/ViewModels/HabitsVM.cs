using ESAPIInfo.Plan;
using LazyOptimizer.App;
using LazyOptimizer.Model;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;
using System.Windows;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class HabitsVM : ViewModel<HabitsModel>
    {
        private readonly AppContext context;
        private PlanVM selectedPlanVM;
        private string selectedNtoString;
        private ObservableCollection<IStructureSuggestionModel> unusedStructures;
        private bool loadNto;
        private string prioritySetter;

        public HabitsVM(HabitsModel habitsModel, AppContext context) : base(habitsModel)
        {
            this.context = context;
            Plans = new SlaveCollection<IPlanBaseModel, PlanVM>(
                habitsModel.PlanModels,
                m => CreatePlanVM(m),
                vm => vm.SourceModel);

            loadNto = context.Settings.LoadNto;
            prioritySetter = context.Settings.DefaultPrioritySetValue;
            SetCanMergeFeature(context.Settings.PlanMergeEnabled);

            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {

                    case nameof(SelectedPlan):
                        BindCollections(selectedPlanVM);
                        UpdateNto(selectedPlanVM?.SourceModel.NtoInfo);
                        break;
                    case nameof(LoadNto):
                        context.Settings.LoadNto = LoadNto;
                        break;
                    case nameof(PrioritySetter):
                        context.Settings.DefaultPrioritySetValue = PrioritySetter;
                        break;
                }
            };

            context.Settings.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(context.Settings.PlanMergeEnabled):
                        SetCanMergeFeature(context.Settings.PlanMergeEnabled);
                        break;
                    case nameof(context.Settings.LoadNto):
                        LoadNto = context.Settings.LoadNto;
                        break;
                }
            };
        }

        private void SetCanMergeFeature(bool canMerge)
        {
            foreach (var plan in Plans)
            {
                plan.CanMerge = canMerge;
            }
        }

        private void BindCollections(PlanVM plan)
        {
            if (plan?.SourceModel == null)
            {
                Structures.BreakFree();
                UnusedStructures = null;
            }
            else
            {
                Structures.ObeyTheMaster(plan.SourceModel.Structures, m => CreateStructureVM(m), vm => vm.SourceModel);
                UnusedStructures = plan.SourceModel.UndefinedStructures;
            }
        }

        private PlanVM CreatePlanVM(IPlanBaseModel plan)
        {
            var planVM = new PlanVM(plan)
            {
                CanMerge = context.Settings.PlanMergeEnabled
            };
            return planVM;
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

        public void FindStructureInOtherPlans(IStructureSuggestionModel structure)
        {
            SourceModel.FindStructureInOtherPlans(SelectedPlan?.SourceModel, structure);
        }

        public SlaveCollection<IPlanBaseModel, PlanVM> Plans { get; }
        public SlaveCollection<IStructureModel, StructureVM> Structures { get; } = new SlaveCollection<IStructureModel, StructureVM>();
        public ObservableCollection<IStructureSuggestionModel> UnusedStructures { get => unusedStructures; set => SetProperty(ref unusedStructures, value); }
        public PlanVM SelectedPlan { get => selectedPlanVM; set => SetProperty(ref selectedPlanVM, value); }
        public string SelectedNtoString { get => selectedNtoString; private set => SetProperty(ref selectedNtoString, value); }
        public bool LoadNto { get => loadNto; set => SetProperty(ref loadNto, value); }
        public string PrioritySetter
        {
            get => prioritySetter;
            set
            {
                if (value == "" || (double.TryParse(value, out double dv) && dv <= 1000))
                {
                    SetProperty(ref prioritySetter, value);
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
