namespace LazyContouring.Operations
{
    public sealed class EmptyOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.SegmentVolume = node.StructureVar?.Structure?.SegmentVolume;
        }

        public override OperationType OperationType => OperationType.Empty;
        //public override bool StructureNedded { get; } = true;
        public override bool LeftNodeOnlyNedded { get; } = true;
    }

}
