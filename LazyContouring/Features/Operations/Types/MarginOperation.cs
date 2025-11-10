namespace LazyContouring.Operations
{
    public sealed class MarginOperation : Operation
    {
        private double marginInMM = 5.0;

        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(MarginInMM);
        }

        public override OperationType OperationType => OperationType.Margin;
        public double MarginInMM { get => marginInMM; set => SetProperty(ref marginInMM, value); }
        public override bool RightNodeNedded { get; } = false;
    }

}
