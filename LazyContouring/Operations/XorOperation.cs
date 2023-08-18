namespace LazyContouring.Operations
{
    public sealed class XorOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Xor(node.NodeRight.SegmentVolume);
        }
        public override OperationType OperationType => OperationType.Xor;
    }

}
