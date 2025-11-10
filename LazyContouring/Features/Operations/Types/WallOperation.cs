namespace LazyContouring.Operations
{
    public sealed class WallOperation : Operation
    {
        private double innerMarginInMM = 5;
        private double outerMarginInMM = 15;

        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(OuterMarginInMM)
                .Sub(node.NodeLeft.SegmentVolume.Margin(InnerMarginInMM));
        }

        public override OperationType OperationType => OperationType.Wall;
        public override bool RightNodeNedded { get; } = false;
        public double InnerMarginInMM { get => innerMarginInMM; set => SetProperty(ref innerMarginInMM, value); }
        public double OuterMarginInMM { get => outerMarginInMM; set => SetProperty(ref outerMarginInMM, value); }
    }

}
