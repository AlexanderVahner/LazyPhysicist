namespace LazyContouring.Operations
{
    public sealed class MarginOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(MarginInMM);
        }

        public override OperationType OperationType => OperationType.Margin;
        public double MarginInMM { get; set; } = 5.0;
        public override bool LeftNodeOnlyNedded { get; } = true;
    }

}
