namespace LazyContouring.Operations
{
    public sealed class OrOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Or(node.NodeRight.SegmentVolume);
        }

        public override OperationType OperationType => OperationType.Or;
    }

}
