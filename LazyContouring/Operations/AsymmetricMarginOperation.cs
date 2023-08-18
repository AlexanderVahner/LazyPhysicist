using VMS.TPS.Common.Model.Types;

namespace LazyContouring.Operations
{
    public sealed class AsymmetricMarginOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry, X1, Y1, Z1, X2, Y2, Z2));
        }

        public override OperationType OperationType => OperationType.AsymmetricMargin;
        public override bool LeftNodeOnlyNedded { get; } = true;
        public StructureMarginGeometry StructureMarginGeometry { get; set; } = StructureMarginGeometry.Outer;
        public double X1 { get; set; } = 5;
        public double X2 { get; set; } = 5;
        public double Y1 { get; set; } = 5;
        public double Y2 { get; set; } = 5;
        public double Z1 { get; set; } = 5;
        public double Z2 { get; set; } = 5;
    }

}
