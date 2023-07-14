using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyContouring.Operations
{
    //public enum OperationType { Assign, And, Or, Not, Sub, Xor, Wall, Margin, AsymmetricMargin }
    public abstract class Operation
    {
        public abstract void Perform(OperationNode node);
        public virtual bool StructureNedded { get; } = false;
    }

    public sealed class AssignOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.StructureVar.Structure.SegmentVolume = node.NodeLeft.SegmentVolume;
        }
        public override bool StructureNedded { get; } = true;
    }

    public sealed class AndOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.And(node.NodeRight.SegmentVolume);
        }
    }

    public sealed class OrOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Or(node.NodeRight.SegmentVolume);
        }
    }

    public sealed class NotOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Not();
        }
    }

    public sealed class SubOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Sub(node.NodeRight.SegmentVolume);
        }
    }

    public sealed class XorOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Xor(node.NodeRight.SegmentVolume);
        }
    }

    public sealed class MarginOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(MarginInMM);
        }

        public double MarginInMM { get; set; } = 5.0;
    }

    public sealed class AsymmetricMarginOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry, X1, Y1, Z1, X2, Y2, Z2));
        }

        public StructureMarginGeometry StructureMarginGeometry { get; set; } = StructureMarginGeometry.Outer;
        public double X1 { get; set; } = 5;
        public double X2 { get; set; } = 5;
        public double Y1 { get; set; } = 5;
        public double Y2 { get; set; } = 5;
        public double Z1 { get; set; } = 5;
        public double Z2 { get; set; } = 5;
    }

    public sealed class WallOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(OuterMarginInMM)
                .Sub(node.NodeLeft.SegmentVolume.Margin(InnerMarginInMM));
        }

        public double InnerMarginInMM { get; set; }
        public double OuterMarginInMM { get; set; }
    }

    public enum CropPart { Inside, Outside }
    public sealed class CropOperation : Operation
    {
        public override void Perform(OperationNode node)
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

        public CropPart CropPart { get; set; } = CropPart.Inside;
        public double MarginInMM { get; set; }
    }

}
