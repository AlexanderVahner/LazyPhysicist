namespace LazyContouring.Operations
{
    public sealed class WallOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(OuterMarginInMM)
                .Sub(node.NodeLeft.SegmentVolume.Margin(InnerMarginInMM));
        }

        public override OperationType OperationType => OperationType.Wall;
        public override bool LeftNodeOnlyNedded { get; } = true;
        public double InnerMarginInMM { get; set; }
        public double OuterMarginInMM { get; set; }
    }

}
