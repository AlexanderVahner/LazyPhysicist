using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LazyContouring.Operations.ContextConditions
{
    [XmlInclude(typeof(DiagnosisCondition))]
    [XmlInclude(typeof(ImageSliceThicknessCondition))]
    [XmlInclude(typeof(PlanTechniqueCondition))]
    [XmlInclude(typeof(StructureCondition))]
    public abstract class ContextCondition : ConditionTreeNode
    {
        private bool shouldBe = true;

        public bool Meets(ScriptArgs args)
        {
            return ShouldBe == Check(args);
        }

        protected override bool CheckNodeDefinition(ScriptArgs args)
        {
            return Meets(args);
        }

        protected abstract bool Check(ScriptArgs args);
        public bool ShouldBe { get => shouldBe; set => SetProperty(ref shouldBe, value); }
    }
}
