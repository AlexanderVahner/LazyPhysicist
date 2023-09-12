using LazyPhysicist.Common;
using System;
using System.Xml.Serialization;

namespace LazyContouring.Operations
{
    public enum OperationType { Empty, Assign, And, Or, Not, Sub, Xor, Wall, Margin, AsymmetricMargin, Crop, Unknown }

    [XmlInclude(typeof(AndOperation))]
    [XmlInclude(typeof(AssignOperation))]
    [XmlInclude(typeof(AsymmetricMarginOperation))]
    [XmlInclude(typeof(CropOperation))]
    [XmlInclude(typeof(EmptyOperation))]
    [XmlInclude(typeof(MarginOperation))]
    [XmlInclude(typeof(NotOperation))]
    [XmlInclude(typeof(OrOperation))]
    [XmlInclude(typeof(SubOperation))]
    [XmlInclude(typeof(WallOperation))]
    [XmlInclude(typeof(XorOperation))]
    public abstract class Operation : Notifier, ICloneable
    {
        public void Execute(OperationNode node)
        {
            if (!CanExecute(node))
            {
                return;
            }

            ExecuteMethod(node);
        }

        protected void ExecuteMethod(OperationNode node)
        {
            if (LeftNodeNedded)
            {
                node.NodeLeft.Operation.ExecuteMethod(node.NodeLeft);
            }
            if (RightNodeNedded)
            {
                node.NodeRight.Operation.ExecuteMethod(node.NodeRight);
            }
            Method(node);
        }

        public bool CanExecute(OperationNode node)
        {
            return node != null && CheckStructure(node) && CheckLeftNode(node) && CheckRightNode(node) && AdditionalCheck(node);
        }

        private bool CheckStructure(OperationNode node)
        {
            return !StructureNedded || node.StructureVar?.Structure != null;
        }

        private bool CheckLeftNode(OperationNode node)
        {
            return !LeftNodeNedded || (node.NodeLeft?.Operation != null && node.NodeLeft.Operation.CanExecute(node.NodeLeft));
        }

        private bool CheckRightNode(OperationNode node)
        {
            return !RightNodeNedded || (node.NodeRight?.Operation != null && node.NodeRight.Operation.CanExecute(node.NodeRight));
        }

        protected virtual bool AdditionalCheck(OperationNode node)
        {
            return true;
        }

        protected abstract void Method(OperationNode node);

        public object Clone()
        {
            return MemberwiseClone();
        }

        public abstract OperationType OperationType { get; }
        public virtual bool StructureNedded { get; } = false;
        public virtual bool LeftNodeNedded { get; } = true;
        public virtual bool RightNodeNedded { get; } = true;
    }
}
