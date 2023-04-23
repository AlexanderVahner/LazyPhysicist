using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
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
        ObjectiveInfo GetObjectiveInfo(IStructureInfo structure);
        void ResetPriority();
        double Priority { get; set; }
        double? Dose { get; }
        double? Volume { get; }
        double? ParameterA { get; }
        ObjectiveType ObjType { get; }
        Operator Operator { get; }
    }
}
