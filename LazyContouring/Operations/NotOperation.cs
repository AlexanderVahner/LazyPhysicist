namespace LazyContouring.Operations
{
    public sealed class NotOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Not();
        }

        public override OperationType OperationType => OperationType.Not;
        public override bool LeftNodeOnlyNedded { get; } = true;
    }

}
