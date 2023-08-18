namespace LazyContouring.Operations
{
    public sealed class SubOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Sub(node.NodeRight.SegmentVolume);
        }
        public override OperationType OperationType => OperationType.Sub;
    }

}
