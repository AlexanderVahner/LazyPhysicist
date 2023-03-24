using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using ESAPIInfo.Plan;
using LazyOptimizer.App;
using LazyPhysicist.Common;
using PlansCache.Properties;
using System.Runtime.InteropServices;
using LazyPhysicist.Common.Console;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace PlansCache
{
    class Program
    {
        private static string dbPath = "";
        private static bool verbose = false;
        private static bool debug = false;
        private static bool allPatients = false;

        private static string currentUserId = "";
        private static IDataService dataService;
        private static DateTime lastCheckDate;

        [STAThread]
        static void Main(string[] args)
        {
            DisableConsoleQuickEdit.Go();

            for (int i = 0; i < args.Count(); i++)
            {
                if (args[i] == "-db")
                {
                    dbPath = args[++i];
                    continue;
                }
                if (args[i] == "-all")
                {
                    allPatients = true;
                    continue;
                }
                if (args[i] == "-verbose")
                {
                    verbose = true;
                    continue;
                }
                if (args[i] == "-debug")
                {
                    debug = true;
                    continue;
                }
            }

            if (dbPath == "")
            {
                Console.WriteLine(Resources.README);
                Console.ReadKey();
                return;
            }

            Logger.Logged += (s, message, type) =>
            {
                if (!debug && type == LogMessageType.Debug)
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

            try
            {
                using (Application app = Application.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e, e.ToString(), LogMessageType.Error);
                Console.ReadKey();
            }
        }
        static void Execute(Application app)
        {
            currentUserId = app.CurrentUser.Id;

            DateTime startTime = DateTime.Now;

            dataService = new DataService(new DataServiceSettings() { DBPath = dbPath });
            if (dataService.Connected)
            {
                lastCheckDate = new DateTime(1789, 7, 14);
                if (!allPatients)
                {
                    lastCheckDate = dataService.GetLastCheckDate() ?? lastCheckDate;
                }

                Logger.Write(app, $"Welcome, {app.CurrentUser.Name}!", LogMessageType.Info);
                Logger.Write(app, "Check " + (allPatients ? "since the beginning of time" : $"from {lastCheckDate}") + " in progress...");

                IEnumerable<PatientSummary> summaries = allPatients ? app.PatientSummaries : app.PatientSummaries.Where(ps => ps.CreationDateTime >= lastCheckDate).OrderBy(ps => ps.CreationDateTime);
                PlanInfo planInfo = new PlanInfo();


                // Progress vars
                int pantientsCount = summaries.Count();
                int currentPatientNumber = 0;
                const int skipCountForProgress = 10;
                int currentSkip = 0;
                double progress = 0;

                if (allPatients)
                {
                    dataService.ClearData();
                }

                foreach (PatientSummary ps in summaries)
                {
                    // Progress 
                    currentPatientNumber++;
                    if (currentSkip++ == 0)
                    {
                        progress = (double)currentPatientNumber * 100 / pantientsCount;
                        Console.Title = $"{progress:F1}% - Plans checking in progress";
                    }
                    if (currentSkip >= skipCountForProgress)
                    {
                        currentSkip = 0;
                    }

                    Patient patient = app.OpenPatientById(ps.Id);
                    lastCheckDate = ps.CreationDateTime ?? lastCheckDate;

                    foreach (Course course in patient.Courses)
                    {
                        foreach (ExternalPlanSetup plan in course.ExternalPlanSetups)
                        {
                            planInfo.Plan = plan;

                            if (planInfo.ObjectivesCount > 0
                            //&& PlanInfo.TreatedPlanStatuses.Contains(planInfo.ApprovalStatus)
                            && planInfo.CreatorId == currentUserId
                            )
                            {

                                if (verbose)
                                {
                                    Logger.Write(app, $"\n{planInfo.PatientId}/{planInfo.CourseId}/{planInfo.PlanId}", LogMessageType.Default);
                                    Logger.Write(app, $"Objectives Count - {planInfo.ObjectivesCount}", LogMessageType.Debug);
                                }

                                dataService.SavePlanToDB(planInfo);
                            }
                            else
                            {
                                Console.Write('.');
                            }
                        }
                    }
                    app.ClosePatient();
                }

                dataService.SetLastCheckDate(lastCheckDate);
                dataService.Connected = false;

                TimeSpan executionTime = DateTime.Now - startTime;

                Console.Title = "All is done";
                Logger.Write(app, $"\nAll is done in {executionTime}.", LogMessageType.Info);

            }
            if (verbose)
            {
                Logger.Write(app, "Press Any Key.", LogMessageType.Info);
                Console.ReadKey();
            }
            
        }
    }
    
}
