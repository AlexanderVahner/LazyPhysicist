using LazyOptimizer.Model;
using LazyOptimizer.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Windows.Controls;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class MainVM : Notifier
    {
        private App.AppContext context;
        private string btnSettingsContent = "Settings";
        private Page currentPage;
        private HabitsPage habitsPage;
        private SettingsPage settingsPage;
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

                CurrentPage = habitsPage;

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
        public string BtnSettingsContent
        {
            get => btnSettingsContent;
            set => SetProperty(ref btnSettingsContent, value);
        }
        public MetaCommand RefreshPlans => new MetaCommand(
                o =>
                {
                    RefreshPlansClick?.Invoke(this, Context);
                },
                o => RefreshPlansClick != null
            );
        public MetaCommand TogglePages => new MetaCommand(
                o =>
                {
                    BtnSettingsContent = BtnSettingsContent == "Settings" ? "Back To Plans" : "Settings";
                    if (Equals(CurrentPage, habitsPage))
                    {
                        CurrentPage = settingsPage;
                    }
                    else
                    {
                        context.UserSettings.Save();
                        CurrentPage = habitsPage;
                    }
                }
            );

        public event EventHandler<App.AppContext> RefreshPlansClick;
    }
}
