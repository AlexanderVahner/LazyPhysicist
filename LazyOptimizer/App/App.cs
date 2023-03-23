using Common;
using ESAPIInfo.Patients;
using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizer.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyOptimizer.App
{
    public class App : IDisposable
    {
        private readonly AppContext context;
        public App(ScriptArgs args)
        {
            Logger.Logged += (s, message, type) => Debug.WriteLine($"{type} from {s?.GetType().Name ?? "UNKNOWN"}: {message}");

            MainVM mainViewModel = InitializeUI(args.Window);

            if (args?.Patient != null && args?.Plan != null && args.Window != null)
            {
                context = new AppContext()
                {
                    Patient = new PatientInfo(args.Patient),
                    Plan = new PlanInfo(args.Plan),
                    Settings = Settings.ReadSettings(),
                    
                };
                if (CheckPlanEditability(context.Plan))
                {
                    context.PlansFilterArgs = new PlansFilterArgs()
                    {
                        StructuresString = context.Plan.StructuresString,
                        FractionsCount = context.Plan.FractionsCount,
                        SingleDose = context.Plan.SingleDose,
                        MachineId = context.Plan.MachineId,
                        Technique = context.Plan.Technique,
                        MatchMachine = context.Settings.MatchMachine,
                        MatchTechnique = context.Settings.MatchTechnique,
                        Limit = context.Settings.PlansSelectLimit
                    };

                    context.DataService = new DataService(new DataServiceSettings() { DBPath = $"{context.Settings.SqliteDbPath}" });

                    context.Settings.PropertyChanged += (s, e) =>
                    {
                        switch (e.PropertyName)
                        {
                            case "MatchMachine":
                                context.PlansFilterArgs.MatchMachine = context.Settings.MatchMachine;
                                break;
                            case "MatchTechnique":
                                context.PlansFilterArgs.MatchTechnique = context.Settings.MatchTechnique;
                                break;
                            case "PlansSelectLimit":
                                context.PlansFilterArgs.Limit = context.Settings.PlansSelectLimit;
                                break;
                        }
                    };

                    context.PlansFilterArgs.PropertyChanged += (s, e) =>
                    {
                        SelectPlans(context);
                    };

                    mainViewModel.Context = context;
                    mainViewModel.RefreshPlansClick += (s, context) =>
                    {
                        PlansCacheAppStart(context);
                        SelectPlans(context);
                        context.Settings.PlansCacheRecheckAllPatients = false;
                    };

                    SelectPlans(context);
                }
                else
                {
                    Logger.Write(this, "The plan is not ready for Optimization.", LogMessageType.Error);
                }
            }
            else
            {
                Logger.Write(this, "Plan is not opened.", LogMessageType.Error);
            }
        }
        public bool CheckPlanEditability(PlanInfo plan)
        {
            bool result = false;

            if (!PlanInfo.EditablePlanStatuses.Contains(plan.ApprovalStatus))
            {
                Logger.Write(this, "Unapprove plan for make chages.", LogMessageType.Warning);
            }
            else if (plan.MachineId == "")
            {
                Logger.Write(this, "Plan doesn't have beams.", LogMessageType.Warning);
            }
            else if (plan.Structures.Count() == 0)
            {
                Logger.Write(this, "Plan doesn't have structures.", LogMessageType.Warning);
            }
            else
            {
                result = true;
            }
            return result;
        }

        public void SelectPlans(AppContext context)
        {            
            if (context.DataService?.Connected ?? false)
            {
                context.DataService.GetPlans(context.PlansFilterArgs);

                int plansCount = context.DataService.DBPlans.Count;
                if (plansCount == 0)
                {
                    Logger.Write(this, "Seems like you don't have matched plans. Maybe you need to recheck them?", LogMessageType.Warning);
                }
                else
                {
                    Logger.Write(this, $"You have {plansCount} matched plan" + (plansCount == 1 ? "." : "s."));
                }
            }
        }

        public MainVM InitializeUI(Window window)
        {
            MainVM mainVM = new MainVM();
            MainPage mainPage = new MainPage() { DataContext = mainVM };

            window.Width = 900;
            window.Height = 800;
            window.Title = "LazyOptimizer";
            window.Closing += (s, e) => Dispose();
            window.Content = mainPage;

            Logger.Write(this, "Welcome!", LogMessageType.Info);

            return mainVM;
        }

        public void PlansCacheAppStart(AppContext context)
        {
            if (File.Exists(context.Settings.PlansCacheAppPath))
            {
                StringBuilder appArgs = new StringBuilder($"-db \"{context.DataService.DBPath}\"");
                if (context.Settings.PlansCacheRecheckAllPatients)
                {
                    appArgs.Append(" -all");
                }
                if (context.Settings.PlansCacheVerboseMode)
                {
                    appArgs.Append(" -verbose");
                }
                if (context.Settings.DebugMode)
                {
                    appArgs.Append(" -debug");
                }
                context.DataService.Connected = false;
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = context.Settings.PlansCacheAppPath;
                    process.StartInfo.Arguments = appArgs.ToString();
                    process.Start();
                    process.WaitForExit();
                };
                context.DataService.Connected = true;
            }
            else
            {
                Logger.Write(this, "PlansCache App not found.", LogMessageType.Error);
            }
            
        }
        
        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
