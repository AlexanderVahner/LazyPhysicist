namespace LazyContouring.Operations
{
    public sealed class NotOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Not();
        }

        public override OperationType OperationType => OperationType.Not;
        public override bool RightNodeNedded { get; } = false;
    }

}
