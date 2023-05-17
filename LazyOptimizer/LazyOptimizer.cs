using LazyOptimizer;
using LazyOptimizer.App;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.9")]
[assembly: AssemblyFileVersion("1.0.0.9")]
[assembly: AssemblyInformationalVersion("1.8")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        public Script() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context, System.Windows.Window window/*, ScriptEnvironment environment*/)
        {
            Run(new ScriptArgs()
            {
                CurrentUser = context.CurrentUser,
                Plan = context.ExternalPlanSetup,
                Window = window
            });
        }
        public void Run(ScriptArgs args)
        {
            new App(args);
        }
    }
}
