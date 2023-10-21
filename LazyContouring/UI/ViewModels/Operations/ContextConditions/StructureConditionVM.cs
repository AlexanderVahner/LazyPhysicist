using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class StructureConditionVM : Notifier
    {
        private readonly StructureCondition structureCondition;

        public StructureConditionVM(StructureCondition structureCondition)
        {
            this.structureCondition = structureCondition;
            structureCondition.PropertyChanged += (s, e) => NotifyPropertyChanged(nameof(Title));
        }

        public string Title => "The SS should " + (StructureCondition.ShouldBe ? "" : "NOT") + " have this structure:";
        public StructureCondition StructureCondition => structureCondition;

        public List<StructureConditionTypeStruct> StructureConditionTypes => StructureCondition.StrucutreConditionTypes;
    }


}
