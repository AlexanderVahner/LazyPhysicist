using ESAPIInfo.Plan;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.Model
{
    public interface IObjectiveModel : INotifyPropertyChanged
    {
        void ResetPriority();
        CachedObjective CachedObjective { get; }
        double Priority { get; set; }
        double? Dose { get; }
        double? Volume { get; }
        double? ParameterA { get; }
        ObjectiveType ObjectiveDBType { get; }
        Operator ObjectiveDBOperator { get; }
    }
}
