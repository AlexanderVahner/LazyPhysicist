using VMS.TPS.Common.Model.Types;

namespace LazyContouring.Operations
{
    public sealed class AsymmetricMarginOperation : Operation
    {
        private double x1 = 5;
        private double x2 = 5;
        private double y1 = 5;
        private double y2 = 5;
        private double z1 = 5;
        private double z2 = 5;

        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry, X1, Y1, Z1, X2, Y2, Z2));
        }

        public override OperationType OperationType => OperationType.AsymmetricMargin;
        public override bool RightNodeNedded { get; } = false;
        public StructureMarginGeometry StructureMarginGeometry { get; set; } = StructureMarginGeometry.Outer;
        public double X1 { get => x1; set => SetProperty(ref x1, value); }
        public double X2 { get => x2; set => SetProperty(ref x2, value); }
        public double Y1 { get => y1; set => SetProperty(ref y1, value); }
        public double Y2 { get => y2; set => SetProperty(ref y2, value); }
        public double Z1 { get => z1; set => SetProperty(ref z1, value); }
        public double Z2 { get => z2; set => SetProperty(ref z2, value); }
    }

}
