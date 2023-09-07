using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public sealed class AssignOperation : Operation
    {
        private SegmentVolume prevSegmentVolume;
        private bool executed = false;
        protected override void Method(OperationNode node)
        {
            if (Executed)
            {
                return;
            }

            prevSegmentVolume = node.StructureVar.SegmentVolume;
            node.StructureVar.SegmentVolume = node.NodeLeft.SegmentVolume;
            Executed = true;
        }

        public void Undo(OperationNode node)
        {
            if (!Executed)
            {
                return;
            }

            node.StructureVar.SegmentVolume = prevSegmentVolume;
            Executed = false;
        }

        public bool Executed { get => executed; set => SetProperty(ref executed, value); }
        public override OperationType OperationType => OperationType.Assign;
        public override bool StructureNedded { get; } = true;
        public override bool RightNodeNedded { get; } = false;
    }

}
