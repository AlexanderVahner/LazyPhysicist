namespace LazyContouring.Operations
{
    public sealed class AndOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.And(node.NodeRight.SegmentVolume);
        }

        public override OperationType OperationType => OperationType.And;
    }

}
