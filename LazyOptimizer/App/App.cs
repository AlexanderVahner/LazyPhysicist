using ESAPIInfo.Plan;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizer.UI.Views;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using ScriptArgsNameSpace;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace LazyOptimizer.App
{
    public class App : IDisposable
    {
        private readonly AppContext context;
        public App(ScriptArgs args)
        {
            Logger.Logged += (s, message, type) => Debug.WriteLine($"{type} from {s?.GetType().Name ?? "UNKNOWN"}: {message}");

            MainVM mainViewModel = InitializeUI(args.Window);

            if (args?.Plan == null)
            {
                Logger.Write(this, "Plan is not opened.", LogMessageType.Error);
                return;
            }

            var generalSettings = GeneralSettings.ReadGeneralSettings();
            var userSettings = UserSettings.ReadUserSettings(generalSettings.UserPath, args.CurrentUser.Id);

            context = new AppContext()
            {
                CurrentPlan = new PlanInfo(args.Plan),
                GeneralSettings = generalSettings,
                UserSettings = userSettings
            };

            if (!CheckPlanEditability(context.CurrentPlan))
            {
                Logger.Write(this, "The plan is not ready for Optimization.", LogMessageType.Error);
                return;
            }

            context.PlansFilterArgs = new PlansFilterArgs()
            {
                StructuresString = context.CurrentPlan.StructuresPseudoHash,
                FractionsCount = context.CurrentPlan.FractionsCount,
                SingleDose = context.CurrentPlan.SingleDose,
                MachineId = context.CurrentPlan.MachineId,
                Technique = context.CurrentPlan.Technique,
                MatchMachine = context.UserSettings.MatchMachine,
                MatchTechnique = context.UserSettings.MatchTechnique,
                Limit = context.UserSettings.PlansSelectLimit,
                StarredOnly = context.UserSettings.ShowStarredOnly,
                CheckedApprovalStatuses = context.UserSettings.GetCheckedApprovalStatusesInInt(),
                CheckedApprovalStatusesOnly = context.UserSettings.ShowCheckedApprovalStatusOnly
            };

            context.DbService = new SQLiteService(context.UserSettings.SqliteDbPath);
            context.PlansContext = new PlansDbContext(context.DbService);

            context.UserSettings.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(context.UserSettings.MatchMachine):
                        context.PlansFilterArgs.MatchMachine = context.UserSettings.MatchMachine;
                        break;
                    case nameof(context.UserSettings.MatchTechnique):
                        context.PlansFilterArgs.MatchTechnique = context.UserSettings.MatchTechnique;
                        break;
                    case nameof(context.UserSettings.PlansSelectLimit):
                        context.PlansFilterArgs.Limit = context.UserSettings.PlansSelectLimit;
                        break;
                    case nameof(context.UserSettings.ShowStarredOnly):
                        context.PlansFilterArgs.StarredOnly = context.UserSettings.ShowStarredOnly;
                        break;
                    case nameof(context.UserSettings.ShowCheckedApprovalStatusOnly):
                        context.PlansFilterArgs.CheckedApprovalStatuses = context.UserSettings.GetCheckedApprovalStatusesInInt();
                        context.PlansFilterArgs.CheckedApprovalStatusesOnly = context.UserSettings.ShowCheckedApprovalStatusOnly;
                        break;
                }
            };

            mainViewModel.Context = context;
            mainViewModel.RefreshPlansClick += (s, context) =>
            {
                PlansCacheAppStart(context);
                context.UserSettings.PlansCacheRecheckAllPatients = false;
                context.PlansFilterArgs.Update();
            };
        }
        public bool CheckPlanEditability(IPlanInfo plan)
        {
            bool hasError = false;

            if (!PlanInfo.EditablePlanStatuses.Contains(plan.ApprovalStatus))
            {
                Logger.Write(this, "Unapprove plan for make changes.", LogMessageType.Warning);
                hasError = true;
            }
            if (plan.MachineId == "")
            {
                Logger.Write(this, "Plan doesn't have beams.", LogMessageType.Warning);
                hasError = true;
            }
            if (plan.Structures.Count() == 0)
            {
                Logger.Write(this, "Plan doesn't have structures.", LogMessageType.Warning);
                hasError = true;
            }

            return !hasError;
        }

        public MainVM InitializeUI(Window window)
        {
            MainVM mainVM = new MainVM();
            MainPage mainPage = new MainPage() { DataContext = mainVM };

            window.Width = 1000;
            window.Height = 800;
            window.Title = "LazyOptimizer";
            window.Closing += (s, e) => Dispose();
            window.Content = mainPage;

            Logger.Write(this, "Welcome!", LogMessageType.Info);

            return mainVM;
        }

        public void PlansCacheAppStart(AppContext context)
        {
            if (!File.Exists(context.GeneralSettings.PlansCacheFullFileName))
            {
                Logger.Write(this, $"PlansCache App not found. Check the Settings in \"{context.GeneralSettings.SettingsFullName}\".", LogMessageType.Error);
                return;
            }

            StringBuilder appArgs = new StringBuilder($"-db \"{context.UserSettings.SqliteDbPath}\"");
            if (context.UserSettings.PlansCacheRecheckAllPatients)
            {
                appArgs.Append(" -all");
            }
            if (context.UserSettings.PlansCacheVerboseMode)
            {
                appArgs.Append(" -verbose");
            }
            if (context.UserSettings.DebugMode)
            {
                appArgs.Append(" -debug");
            }
            if (context.UserSettings.YearsLimit > 0)
            {
                appArgs.Append($" -years {context.UserSettings.YearsLimit}");
            }
            context.PlansContext.Connected = false;
            using (Process process = new Process())
            {
                process.StartInfo.FileName = context.GeneralSettings.PlansCacheFullFileName;
                process.StartInfo.Arguments = appArgs.ToString();
                process.Start();
                process.WaitForExit();
            };
            context.PlansContext.Connected = true;
        }

        public void Dispose()
        {
            context?.DbService?.Dispose();
            context?.UserSettings?.Save();
            context?.GeneralSettings?.Save();
        }
    }
}
