using LazyContouring.Models;
using LazyPhysicist.Common;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public enum OperationType { Empty, Assign, And, Or, Not, Sub, Xor, Wall, Margin, AsymmetricMargin, Crop, Unknown }

    public abstract class Operation : Notifier
    {
        public abstract void Perform(OperationNode node);
        public abstract OperationType OperationType { get; }
        public virtual bool StructureNedded { get; } = false;
        public virtual bool LeftNodeOnlyNedded { get; } = false;
    }
}
