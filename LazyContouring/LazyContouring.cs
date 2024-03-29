﻿using LazyContouring.UI.ViewModels;
using LazyContouring.UI.Views;
using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0")]

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
                Patient = context.Patient,
                StructureSet = context.StructureSet,
                Window = window
            });
        }
        public void Run(ScriptArgs args)
        {
            var mainVM = new MainVM();
            
            mainVM.StructureSet = args.StructureSet;
            mainVM.Init();
            mainVM.PaintSlice(100);
            var mainPage = new MainPage() { DataContext = mainVM };

            args.Window.Content = mainPage;
        }
    }
}