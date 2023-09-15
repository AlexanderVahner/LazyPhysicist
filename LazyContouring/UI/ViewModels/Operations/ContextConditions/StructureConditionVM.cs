using LazyContouring.Operations.ContextConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class StructureConditionVM
    {
        private readonly StructureCondition structureCondition;

        public StructureConditionVM(StructureCondition structureCondition)
        {
            this.structureCondition = structureCondition;
        }

        public StructureCondition StructureCondition => structureCondition;

        public string SearchText { get; set; }
        public StructureConditionTypeStruct SearchType { get; set; }
        public List<StructureConditionTypeStruct> StructureConditionTypes => StructureCondition.StrucutreConditionTypes;
    }

    
}
