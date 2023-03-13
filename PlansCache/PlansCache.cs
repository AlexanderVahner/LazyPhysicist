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

        [STAThread]
        static void Main(string[] args)
        {
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
                Console.WriteLine("Welcome! \nThis app reads all plans optimization objectives in all patients using ESAPI, \nand writes data to a DB for the FillTheOptimizer Plugin.\n"
                    + "Using:\n"
                    + "\tCheckOptHabits -db \"<DBFilePath>\" [-all] [-verbose] [-debug]\n"
                    + "\t\t-db \"<DBFilePath>\"    - Path to DB.\n"
                    + "\t\t-all                  - recheck all patients.\n"
                    + "\t\t-verbose              - show additional info.\n"
                    + "\t\t-debug                - show debug info.\n");
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
            
            DataService dataProvider = new DataService(new DataServiceSettings() { DBPath = dbPath });
            if (dataProvider.Connected)
            {
                DateTime lastCheckDate = new DateTime(1789, 7, 14);
                if (!allPatients)
                {
                    lastCheckDate = dataProvider.GetLastCheckDate() ?? lastCheckDate;
                }

                Logger.Write(app, $"Welcome, {app.CurrentUser.Name}!", LogMessageType.Info);
                Logger.Write(app, "Check" + (allPatients ? " since the beginning of time" : "") + " in progress...");

                IEnumerable<PatientSummary> summaries = allPatients ? app.PatientSummaries : app.PatientSummaries.Where(ps => ps.CreationDateTime > lastCheckDate);
                PlanInfo planInfo = new PlanInfo();

                if (allPatients)
                {
                    dataProvider.ClearData();
                }

                foreach (PatientSummary ps in summaries)
                {
                    Patient patient = app.OpenPatientById(ps.Id);
                    foreach (Course course in patient.Courses)
                    {
                        foreach (ExternalPlanSetup plan in course.ExternalPlanSetups)
                        {
                            planInfo.Plan = plan;

                            if (planInfo.ObjectivesCount > 0)
                            //&& PlanInfo.TreatedPlanStatuses.Contains(planInfo.ApprovalStatus)
                            //&& planInfo.CreatorId == currentUserId)
                            {

                                if (verbose)
                                {
                                    Logger.Write(app, $"\n{planInfo.PatientId}/{planInfo.CourseId}/{planInfo.PlanId}", LogMessageType.Default);
                                    Logger.Write(app, $"Objectives Count - {planInfo.ObjectivesCount}", LogMessageType.Debug);
                                }

                                dataProvider.SavePlanToDB(planInfo);
                            }
                            else
                            {
                                Console.Write('.');
                            }
                        }
                    }
                    app.ClosePatient();
                }

                dataProvider.SetLastCheckDate(DateTime.Now);
                dataProvider.Connected = false;

                TimeSpan executionTime = DateTime.Now - startTime;


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
