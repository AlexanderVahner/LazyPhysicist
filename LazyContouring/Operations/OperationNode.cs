using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public sealed class OperationNode
    {
        private SegmentVolume segmentVolume;

        public SegmentVolume SegmentVolume 
        { 
            get => segmentVolume ?? StructureVar?.SegmentVolume; 
            set => segmentVolume = value; 
        }
        public StructureVariable StructureVar { get; set; }
        public Operation Operation { get; set; }
        public OperationNode NodeLeft { get; set; }
        public OperationNode NodeRight { get; set; }
    }
}
