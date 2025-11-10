namespace LazyContouring.Operations
{
    public sealed class SubOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Sub(node.NodeRight.SegmentVolume);
        }
        public override OperationType OperationType => OperationType.Sub;
    }

}
