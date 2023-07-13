using VMS.TPS.Common.Model.Types;

namespace LazyContouring.Operations
{
    public enum OperationType { Assign, And, Or, Not, Sub, Xor, Wall, Margin, AsymmetricMargin }
    public class Operation
    {
        public OperationType Type { get; set; }
    }

    public sealed class MarginOperation : Operation
    {
        public double MarginInMM { get; set; }
    }

    public sealed class AsymmetricMarginOperation : Operation
    {
        public AxisAlignedMargins Margins { get; set; }
    }

    public sealed class WallOperation : Operation
    {
        public double InnerMarginInMM { get; set; }
        public double OuterMarginInMM { get; set; }
    }

}
