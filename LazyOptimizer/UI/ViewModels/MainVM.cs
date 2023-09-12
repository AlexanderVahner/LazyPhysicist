using LazyOptimizer.Model;
using LazyOptimizer.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class MainVM : Notifier
    {
        private App.AppContext context;
        private Page currentPage;
        private HabitsPage habitsPage;
        private SettingsPage settingsPage;
        private CheckPlansPage checkPlansPage;
        private void InitializeModel(App.AppContext context)
        {
            if (context != null)
            {
                if (context.CurrentPlan.ObjectivesCount > 0)
                {
                    Logger.Write(this, $"There are already Optimization Objectives in this plan. Keep in mind...", LogMessageType.Warning);
                }

                HabitsVM habitsVM = new HabitsVM(new HabitsModel(context), context);

                habitsPage = new HabitsPage()
                {
                    DataContext = habitsVM
                };

                SettingsVM settingsVM = new SettingsVM()
                {
                    Settings = context.UserSettings
                };
                settingsPage = new SettingsPage()
                {
                    DataContext = settingsVM,
                    Settings = context.UserSettings
                };

                checkPlansPage = new CheckPlansPage()
                {
                    DataContext = new CheckPlansVM()
                    {
                        MainVM = this,
                        Settings = context.UserSettings
                    }
                };

                SelectTab(habitsPage);

                NotifyPropertyChanged(nameof(MatchMachine));
                NotifyPropertyChanged(nameof(MatchTechnique));
                NotifyPropertyChanged(nameof(StarredOnly));
                NotifyPropertyChanged(nameof(CheckedApprovalStatusesOnly));
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
        public Page CurrentPage
        {
            get => currentPage;
            set => SetProperty(ref currentPage, value);
        }
        public bool MatchTechnique
        {
            get => Context?.UserSettings?.MatchTechnique ?? false;
            set
            {
                SetProperty(v => { if (Context?.UserSettings?.MatchTechnique != null) Context.UserSettings.MatchTechnique = v; }, value);
                NotifyPropertyChanged(nameof(MatchTechnique));
            }
        }
        public bool MatchMachine
        {
            get => Context?.UserSettings?.MatchMachine ?? false;
            set
            {
                SetProperty(v => { if (Context?.UserSettings?.MatchMachine != null) Context.UserSettings.MatchMachine = v; }, value);
                NotifyPropertyChanged(nameof(MatchMachine));
            }
        }
        public bool StarredOnly
        {
            get => Context?.UserSettings?.ShowStarredOnly ?? false;
            set
            {
                SetProperty(v => { if (Context?.UserSettings?.ShowStarredOnly != null) Context.UserSettings.ShowStarredOnly = v; }, value);
                NotifyPropertyChanged(nameof(StarredOnly));
            }
        }
        public bool CheckedApprovalStatusesOnly
        {
            get => Context?.UserSettings?.ShowCheckedApprovalStatusOnly ?? false;
            set
            {
                SetProperty(v => { if (Context?.UserSettings?.ShowCheckedApprovalStatusOnly != null) Context.UserSettings.ShowCheckedApprovalStatusOnly = v; }, value);
                NotifyPropertyChanged(nameof(CheckedApprovalStatusesOnly));
            }
        }

        private readonly static Brush selectedTabColor = new SolidColorBrush(Color.FromRgb(255, 11, 121));
        private readonly static Brush defaultTabColor = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        private Brush planSelectionButtonColor = defaultTabColor;
        private Brush checkPlansButtonColor = defaultTabColor;
        private Brush settingsButtonColor = defaultTabColor;
        public Brush PlanSelectionButtonColor { get => planSelectionButtonColor; set => SetProperty(ref planSelectionButtonColor, value); }
        public Brush CheckPlansButtonColor { get => checkPlansButtonColor; set => SetProperty(ref checkPlansButtonColor, value); }
        public Brush SettingsButtonColor { get => settingsButtonColor; set => SetProperty(ref settingsButtonColor, value); }

        public MetaCommand OpenPlanSelectionTab => new MetaCommand(o => SelectTab(habitsPage));
        public MetaCommand OpenCheckPlansTab => new MetaCommand(o => SelectTab(checkPlansPage));
        public MetaCommand OpenSettingsTab => new MetaCommand(o => SelectTab(settingsPage));
        public MetaCommand RefreshPlans => new MetaCommand(
                o =>
                {
                    RefreshPlansClick?.Invoke(this, Context);
                },
                o => RefreshPlansClick != null
            );

        public void SelectTab(Page selectedPage)
        {
            CurrentPage = selectedPage;

            PlanSelectionButtonColor = defaultTabColor;
            CheckPlansButtonColor = defaultTabColor;
            SettingsButtonColor = defaultTabColor;

            if (Equals(selectedPage, habitsPage))
            {
                PlanSelectionButtonColor = selectedTabColor;
            }
            else if (Equals(selectedPage, checkPlansPage))
            {
                CheckPlansButtonColor = selectedTabColor;
            }
            else if (Equals(selectedPage, settingsPage))
            {
                SettingsButtonColor = selectedTabColor;
            }
        }

        public event EventHandler<App.AppContext> RefreshPlansClick;
    }
}
