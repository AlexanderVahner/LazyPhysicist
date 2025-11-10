namespace LazyContouring.Operations
{
    public sealed class OrOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Or(node.NodeRight.SegmentVolume);
        }

        public override OperationType OperationType => OperationType.Or;
    }

}
