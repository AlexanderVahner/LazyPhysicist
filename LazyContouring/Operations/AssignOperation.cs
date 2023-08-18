namespace LazyContouring.Operations
{
    public sealed class AssignOperation : Operation
    {
        public override void Perform(OperationNode node)
        {
            node.StructureVar.Structure.SegmentVolume = node.NodeLeft.SegmentVolume;
        }

        public override OperationType OperationType => OperationType.Assign;
        public override bool StructureNedded { get; } = true;
        public override bool LeftNodeOnlyNedded { get; } = true;
    }

}
