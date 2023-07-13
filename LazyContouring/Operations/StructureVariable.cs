using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public sealed class StructureVariable
    {
        private SegmentVolume segmentVolume;

        public Structure Structure { get; set; }
        public SegmentVolume SegmentVolume
        {
            get => segmentVolume ?? Structure?.SegmentVolume;
            set => segmentVolume = value;
        }
        public string StructureId { get; set; }
        public string DicomType { get; set; }
        
        public bool IsNew { get; set; }
    }
}
