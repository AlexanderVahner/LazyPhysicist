using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public interface IStructureModel : INotifyPropertyChanged
    {
        double GetObjectivesMaxDose();
        string CachedStructureId { get; }
        List<IObjectiveModel> Objectives { get; }
        bool IsTarget { get; }
        IStructureInfo CurrentPlanStructure { get; set; }
        double OrderByDoseDescProperty { get; }
    }
}
