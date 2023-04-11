using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public abstract class PlanBaseModel : IPlanBaseModel
    {
        public ObservableCollection<IStructureModel> Structures => GetStructures();
        public ObservableCollection<IStructureInfo> StructureSuggestions { get; }
        public INtoInfo NtoInfo => GetNto();
        public abstract string PlanTitle { get; }
        protected abstract ObservableCollection<IStructureModel> GetStructures();
        protected abstract ObservableCollection<IStructureInfo> GetStructureSuggestions();
        protected abstract INtoInfo GetNto();
    }
}
