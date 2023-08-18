using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class StructureVariable
    {
        private Structure structure;

        private void SetStructure(Structure structure)
        {
            this.structure = structure;
            StructureId = structure.Id;
            DicomType = structure.DicomType;
        }

        public Structure Structure { get => structure; set => SetStructure(value); }
        public string StructureId { get; set; }
        public string DicomType { get; set; }

        public bool IsNew { get; set; }
        public bool IsTemporary { get; set; }
    }
}
