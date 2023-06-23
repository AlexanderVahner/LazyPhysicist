using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Reflection;
using VMS.TPS.Common.Model.API;

[assembly: AssemblyVersion("1.0.0.9")]
[assembly: AssemblyFileVersion("1.0.0.9")]
[assembly: AssemblyInformationalVersion("1.7")]

// ATTENTION! There are Run Arguments in Project Properties > Debug > Command line arguments. Write yours for debug

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
