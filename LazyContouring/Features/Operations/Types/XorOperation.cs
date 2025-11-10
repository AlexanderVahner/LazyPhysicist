namespace LazyContouring.Operations
{
    public sealed class XorOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Xor(node.NodeRight.SegmentVolume);
        }
        public override OperationType OperationType => OperationType.Xor;
    }

}
