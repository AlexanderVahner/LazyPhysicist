using LazyContouring.Operations.ContextConditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class StructureConditionVM : ConditionNodeVM
    {
        private readonly StructureCondition structureCondition;

        public StructureConditionVM(StructureCondition structureCondition) : base(structureCondition)
        {
            this.structureCondition = structureCondition;
            Title = "The SS should " + (StructureCondition.ShouldBe ? "" : "NOT ") + "have this structure:";
            structureCondition.PropertyChanged += (s, e) => Title = "The SS should " + (StructureCondition.ShouldBe ? "" : "NOT ") + "have this structure:";
        }

        public StructureCondition StructureCondition => structureCondition;
        public List<StructureConditionType> StructureConditionTypes => Enum.GetValues(typeof(StructureConditionType)).Cast<StructureConditionType>().ToList();
        public List<string> StructureConditionDicomTypes => StructureCondition.StrucutreDicomTypes;
    }


}
