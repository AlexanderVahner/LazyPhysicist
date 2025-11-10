namespace LazyContouring.Operations
{
    public sealed class AndOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.And(node.NodeRight.SegmentVolume);
        }

        public override OperationType OperationType => OperationType.And;
    }

}
