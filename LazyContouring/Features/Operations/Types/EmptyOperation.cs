namespace LazyContouring.Operations
{
    public sealed class EmptyOperation : Operation
    {
        protected override void Method(OperationNode node)
        {
            node.SegmentVolume = node.StructureVar?.GetSegmentVolume();
        }

        public override OperationType OperationType => OperationType.Empty;
        public override bool LeftNodeNedded { get; } = false;
        public override bool RightNodeNedded { get; } = false;
    }

}
