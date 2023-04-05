using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using LazyPhysicist.Common.Console;
using PlansCache.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace PlansCache
{
    public class ConsoleApp
    {
        private readonly Application app;
        private readonly IPlansContext context;
        private readonly Parameters parameters;

        private const int SAVE_EVERY_PATIENT_PERIOD = 10;

        private readonly Cache cache;
        private readonly string currentUserId = "";

        public ConsoleApp(Application app, IPlansContext context, Parameters parameters)
        {
            this.app = app;
            this.context = context;
            this.parameters = parameters;

            cache = new Cache(context);
            currentUserId = app.CurrentUser.Id;
        }
        public void Execute()
        {
            if (parameters.DbPath == "")
            {
                Console.WriteLine(Resources.README);
                Console.ReadKey();
                return;
            }

            if (!parameters.DebugMode) // In Debug mode, you can select some text in the console, and thus pause the process
            {
                DisableConsoleQuickEdit.Go();
            }

            // Progress vars
            int patientsCount;
            int currentPatientNumber = 0;
            int currentSkip = 0;
            double progress = 0;

            DateTime startTime = DateTime.Now;

            if (context.Connected)
            {

                Logger.Write(app, $"Welcome, {app.CurrentUser.Name}!", LogMessageType.Info);
                Logger.Write(app, "Check " + (parameters.RecheckAll ? "since the beginning of time" : $"from {cache.LastCheckDate:g}") + " in progress...");

                IEnumerable<PatientSummary> summaries;
                if (parameters.RecheckAll)
                {
                    summaries = app.PatientSummaries.OrderBy(ps => ps.CreationDateTime);
                }
                else
                {
                    summaries = app.PatientSummaries.Where(ps => ps.CreationDateTime >= cache.LastCheckDate).OrderBy(ps => ps.CreationDateTime);
                }

                patientsCount = summaries.Count();
                patientsCount = patientsCount > 0 ? patientsCount : 1;
                cache.ClearData(!parameters.RecheckAll);

                foreach (PatientSummary ps in summaries)
                {
                    cache.LastCheckDate = ps.CreationDateTime ?? cache.LastCheckDate;
                    currentPatientNumber++;

                    ProcessPatient(ps.Id);

                    if (currentSkip++ >= SAVE_EVERY_PATIENT_PERIOD)
                    {
                        cache.WritePlans();

                        currentSkip = 0;
                        progress = (double)currentPatientNumber * 100 / patientsCount;
                        Console.Title = $"{progress:F1}% - Plans checking in progress ({currentPatientNumber}/{patientsCount})";
                    }
                }

                // Writing the latest data
                cache.WritePlans();

                context.Connected = false;

                TimeSpan executionTime = DateTime.Now - startTime;

                Console.Title = "All is done";
                Logger.Write(app, $"\nAll is done in {executionTime:g}.", LogMessageType.Info);

            }
            if (parameters.VerboseMode)
            {
                Logger.Write(app, "Press Any Key.", LogMessageType.Info);
                Console.ReadKey();
            }

        }
        private void ProcessPatient(string patientId)
        {
            Patient patient = app.OpenPatientById(patientId);
            PlanInfo planInfo = new PlanInfo();

            foreach (Course course in patient.Courses)
            {
                foreach (ExternalPlanSetup plan in course.ExternalPlanSetups)
                {
                    planInfo.Plan = plan;

                    if (planInfo.ObjectivesCount > 0 && planInfo.CreatorId == currentUserId)
                    {
                        if (parameters.VerboseMode)
                        {
                            Logger.Write(app, $"\n{planInfo.PatientId}/{planInfo.CourseId}/{planInfo.PlanId}", LogMessageType.Default);
                            Logger.Write(app, $"Objectives Count - {planInfo.ObjectivesCount}", LogMessageType.Debug);
                        }

                        cache.AddPlan(planInfo);
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
            }

            app.ClosePatient();
        }
        public static void InitializeLogger(Parameters parameters)
        {
            Logger.Logged += (s, message, type) =>
            {
                if (!parameters.DebugMode && type == LogMessageType.Debug)
                {
                    return;
                }

                switch (type)
                {
                    case LogMessageType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogMessageType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogMessageType.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogMessageType.Info:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                Console.WriteLine((type == LogMessageType.Error ? $"Error from {s?.GetType().Name} : " : "") + message);
                Console.ForegroundColor = ConsoleColor.White;
            };
        }
    }
}
