using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Reflection;
using VMS.TPS.Common.Model.API;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.4")]
[assembly: AssemblyFileVersion("1.0.0.4")]
[assembly: AssemblyInformationalVersion("1.3")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace PlansCache
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parameters parameters = new Parameters(args);
            ConsoleApp.InitializeLogger(parameters);

            try
            {
                using (Application app = Application.CreateApplication())
                {
                    var context = new PlansDbContext(new SQLiteService(parameters.DbPath));
                    if (context.Connected)
                    {
                        new ConsoleApp(app, context, parameters).Execute();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Write(e, e.ToString(), LogMessageType.Error);
                Console.ReadKey();
            }
        }


    }

}
