using LazyOptimizer.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Windows.Controls;

namespace LazyOptimizer.UI.ViewModels
{
    public class MainVM : ViewModel
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

                HabitsVM habitsVM = new HabitsVM(context);

                habitsPage = new HabitsPage()
                {
                    DataContext = habitsVM
                };

                SettingsVM settingsVM = new SettingsVM()
                {
                    Settings = context.Settings
                };
                settingsPage = new SettingsPage()
                {
                    DataContext = settingsVM,
                    Settings = context.Settings
                };

                CurrentPage = habitsPage;

                NotifyPropertyChanged(nameof(MatchMachine));
                NotifyPropertyChanged(nameof(MatchTechnique));
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
            get => Context?.Settings?.MatchTechnique ?? false;
            set
            {
                SetProperty(v => { if (Context?.Settings?.MatchTechnique != null) Context.Settings.MatchTechnique = v; }, value);
                NotifyPropertyChanged(nameof(MatchTechnique));
            }
        }
        public bool MatchMachine
        {
            get => Context?.Settings?.MatchMachine ?? false;
            set
            {
                SetProperty(v => { if (Context?.Settings?.MatchMachine != null) Context.Settings.MatchMachine = v; }, value);
                NotifyPropertyChanged(nameof(MatchMachine));
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
                        context.Settings.Save();
                        CurrentPage = habitsPage;
                    }
                }
            );

        public event EventHandler<App.AppContext> RefreshPlansClick;
    }
}
