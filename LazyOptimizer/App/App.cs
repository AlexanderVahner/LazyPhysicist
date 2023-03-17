using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizer.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
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
        private IDataService dataService;
        private MainVM mainVM;
        private Patient patient;
        private PlanInfo planInfo;
        private Settings settings;
        private PlansFilterArgs filterArgs;

        private HabitsVM habitsVM = new HabitsVM();
        public App(ScriptArgs args)
        {
            Logger.Logged += (s, message, type) => Debug.WriteLine($"{type} from {s?.GetType().Name ?? "UNKNOWN"}: {message}");

            if (args?.Patient != null && args?.Plan != null && args.Window != null)
            {
                patient = args.Patient;
                planInfo = new PlanInfo() { Plan = args.Plan };

                settings = new Settings();
                mainVM = new MainVM();
                InitializeUI(args.Window);
                
                if (planInfo.IsReadyForOptimizerLoad)
                {
                    dataService = new DataService(new DataServiceSettings() { DBPath = $"{settings.UserPath}\\{settings.SqliteDBName}" });

                    filterArgs = new PlansFilterArgs()
                    {
                        StructuresString = planInfo.StructuresString,
                        FractionsCount = planInfo.FractionsCount,
                        SingleDose = planInfo.SingleDose,
                        Limit = settings.PlansSelectLimit
                    };

                    UpdateFilterArgs(filterArgs);
                    UpdatePlans(filterArgs);
                }
                else
                {
                    Logger.Write(this, "The plan is not ready for Optimization.\nMake some beams, or unapprove it.", LogMessageType.Error);
                }
                
            }
            else
            {
                Logger.Write(this, "Plan is not opened.", LogMessageType.Error);
            }
        }

        public void UpdatePlans(PlansFilterArgs filterArgs)
        {
            string many(int listCount) => listCount > 1 ? "s" : "";
            
            if (dataService?.Connected ?? false)
            {
                dataService.GetPlans(filterArgs);
                habitsVM.UpdatePlans(dataService.DBPlans);

                if (dataService.DBPlans.Count == 0)
                {
                    Logger.Write(this, "Seems like you don't have matched plans. Maybe you need to recheck them.", LogMessageType.Warning);
                }
                else
                {
                    Logger.Write(this, $"You have {dataService.DBPlans.Count} matched plan{many(dataService.DBPlans.Count)}.", LogMessageType.Info);
                }
            }
        }

        public void InitializeUI(Window window)
        {
            mainVM = new MainVM();
            MainPage mainPage = new MainPage() { DataContext = mainVM };

            habitsVM.CurrentPlan = planInfo;
            habitsVM.SelectedDBPlanChanged += (s, plan) =>
            {
                if ((plan?.DBObjectives.Count ?? -1) == 0)
                {
                    dataService.GetObjectives(plan.DBObjectives, (int)plan.DBPlan.rowid);
                    plan.Nto.NtoDB = dataService.GetNto((int)plan.DBPlan.rowid);
                }
            };
            habitsVM.LoadIntoPlanClick += (s, objectives) => LoadObjectivesIntoPlan(objectives);
            habitsVM.LoadNtoIntoPlanClick += (s, nto) => LoadNtoIntoPlan(nto);

            mainVM.FiltersChanged += (s, e) =>
            {
                UpdateFilterArgs(filterArgs);
                UpdatePlans(filterArgs);
            };

            window.Width = 900;
            window.Height = 800;
            window.Closing += (s, e) => Dispose();
            window.Content = mainPage;

            Logger.Write(this, "Welcome", LogMessageType.Info);

            if (planInfo.ObjectivesCount > 0)
            {
                Logger.Write(this, $"This plan already have Optimization Objectives. Keep in mind...", LogMessageType.Warning);
            }

            HabitsPage habitsPage = new HabitsPage()
            {
                DataContext = habitsVM
            };

            SettingsVM settingsVM = new SettingsVM()
            {
                Settings = settings
            };
            SettingsPage settingsPage = new SettingsPage()
            {
                DataContext = settingsVM
            };

            mainVM.CurrentPage = habitsPage;
            mainVM.RefreshHabitsClick += (s, e) => RefreshHabits();
            mainVM.TogglePagesClick += (s, e) =>
            {
                mainVM.BtnSettingsContent = mainVM.BtnSettingsContent == "Settings" ? "Back To Plans" : "Settings";
                if (Equals(mainVM.CurrentPage, habitsPage))
                {
                    mainVM.CurrentPage = settingsPage;
                }
                else
                {
                    mainVM.CurrentPage = habitsPage;
                }
            };
        }

        public void RefreshHabits()
        {
            if (File.Exists(settings.PlansCacheAppPath))
            {
                dataService.Connected = false;
                using (System.Diagnostics.Process process = new Process())
                {
                    // Configure the process using the StartInfo properties.
                    process.StartInfo.FileName = settings.PlansCacheAppPath;
                    process.StartInfo.Arguments = $@"-db ""{dataService.DBPath}"" -all -verbose -debug";
                    process.Start();
                    process.WaitForExit();
                };
                dataService.Connected = true;
                UpdatePlans(filterArgs);
            }
            
        }
        public void LoadNtoIntoPlan(NtoInfo nto)
        {
            patient.BeginModifications();
            if (nto != null && planInfo?.Plan != null)
            {
                if (nto.IsAutomatic)
                {
                    planInfo.Plan.OptimizationSetup.AddAutomaticNormalTissueObjective(nto.Priority);
                }
                else
                {
                    planInfo.Plan.OptimizationSetup.AddNormalTissueObjective(nto.Priority, nto.DistanceFromTargetBorderInMM, nto.StartDosePercentage, nto.EndDosePercentage, nto.FallOff);
                }

            }
        }

        public void LoadObjectiveIntoPlan(ObjectiveInfo objective)
        {
            if (planInfo?.Plan == null)
            {
                Logger.Write(this, "Can't load the objective. The Plan is null", LogMessageType.Error);
            }
            else
            {
                if (!planInfo.IsReadyForOptimizerLoad)
                {
                    Logger.Write(this, "Can't load the objective. The Plan is not unapproved, or it doesn't have beams", LogMessageType.Error);
                }
                else
                {
                    if (objective.Structure == null)
                    {
                        Logger.Write(this, "Can't load the objective. Structure is not defined", LogMessageType.Error);
                    }
                    else
                    {
                        switch (objective.Type)
                        {
                            case ObjectiveType.Point:
                                planInfo.Plan.OptimizationSetup.AddPointObjective(objective.Structure, objective.Operator, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.Volume, objective.Priority);
                                break;
                            case ObjectiveType.Mean:
                                planInfo.Plan.OptimizationSetup.AddMeanDoseObjective(objective.Structure, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.Priority);
                                break;
                            case ObjectiveType.EUD:
                                planInfo.Plan.OptimizationSetup.AddEUDObjective(objective.Structure, objective.Operator, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.ParameterA, objective.Priority);
                                break;
                            case ObjectiveType.Unknown:
                                Logger.Write(this, "Can't load the objective. Type is unknown.", LogMessageType.Error);
                                break;
                        }
                    }
                }
            }
        }
        public void LoadObjectivesIntoPlan(List<ObjectiveInfo> objectives)
        {
            if (objectives != null)
            {
                MessageBoxResult answer = MessageBoxResult.Yes;
                if (planInfo.ObjectivesCount > 0)
                {
                    answer = MessageBox.Show("The plan already has Optimization Objectives.\nDo you want to add all of it?\nClick No if you want to fill only empty structures", "Do you?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (answer == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                patient.BeginModifications();
                objectives.ForEach(o =>
                    {
                        if (answer == MessageBoxResult.No && planInfo.StructureHasObjectives(o.Structure))
                        {
                            return;
                        }
                        LoadObjectiveIntoPlan(o);
                    });
                Logger.Write(this, "Objectives added. Go optimize!", LogMessageType.Info);
            }
            else
            {
                Logger.Write(this, "Can't load objectives. Collection is null.", LogMessageType.Error);
            }
        }

        private void UpdateFilterArgs(PlansFilterArgs args)
        {
            args.MachineId = mainVM.MatchMachine ? planInfo.MachineId : "";
            args.Technique = mainVM.MatchTechnique ? planInfo.Technique : "";
        }

        public void Dispose()
        {
            dataService?.Dispose();
        }
    }
}
