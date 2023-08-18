using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public enum OperationType { Empty, Assign, And, Or, Not, Sub, Xor, Wall, Margin, AsymmetricMargin, Crop }
    public abstract class Operation
    {
        public abstract void Perform(OperationNode node);
        public abstract OperationType OperationType { get; }
        public virtual bool StructureNedded { get; } = false;
        public virtual bool LeftNodeOnlyNedded { get; } = false;
    }
}
