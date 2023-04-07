using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public interface INtoModel : INotifyPropertyChanged
    {
        CachedNto CachedNto { get; set; }
        INtoInfo NtoInfo { get; }
        string NtoString { get; }
    }
}
