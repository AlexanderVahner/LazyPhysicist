using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public sealed class StructureVariable
    {
        public Structure Structure { get; set; }
        public string StructureId { get; set; }
        public string DicomType { get; set; }
        
        public bool IsNew { get; set; }
    }
}
