namespace LazyContouring.Operations
{
    public enum CropPart { Inside, Outside }

    public sealed class CropOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            if (CropPart == CropPart.Inside)
            {
                node.SegmentVolume = node.NodeLeft.SegmentVolume.Sub(node.NodeRight.SegmentVolume.Margin(MarginInMM));
            }
            else
            {
                node.SegmentVolume = node.NodeLeft.SegmentVolume.And(node.NodeRight.SegmentVolume.Margin(MarginInMM));
            }
        }

        public override OperationType OperationType => OperationType.Crop;
        public CropPart CropPart { get; set; } = CropPart.Inside;
        public double MarginInMM { get; set; }
    }

}
