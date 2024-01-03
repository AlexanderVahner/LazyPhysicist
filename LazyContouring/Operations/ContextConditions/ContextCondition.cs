using ScriptArgsNameSpace;
using System.Xml.Serialization;

namespace LazyContouring.Operations.ContextConditions
{
    [XmlInclude(typeof(DiagnosisCondition))]
    [XmlInclude(typeof(ImageCondition))]
    [XmlInclude(typeof(StructureCondition))]
    public abstract class ContextCondition : ConditionNode
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
