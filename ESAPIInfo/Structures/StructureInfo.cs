using System.Linq;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Structures
{
    public class StructureInfo : IStructureInfo
    {
        public static readonly string[] TargetNames = { "PTV", "CTV", "GTV", "BOOST" };
        public static readonly string[] SupportStructureDicomTypes = { "SUPPORT", "FIXATION", "ARTIFACT", "CONTRAST", "REGISTRATION", "UNKNOWN" };
        public static readonly string[] NonOptimizedStructureDicomTypes = { "SUPPORT", "FIXATION", "REGISTRATION", "UNKNOWN" };

        public static bool IsTarget(string structureId)
        {
            return StructureInfo.TargetNames.FirstOrDefault(tn => structureId.StartsWith(tn)) != null;
        }

        public StructureInfo()
        {

        }
        public StructureInfo(Structure structure)
        {
            Structure = structure;
        }
        public bool IsTarget()
        {
            return Structure != null && StructureInfo.IsTarget(Structure.Id);
        }
        public Structure Structure { get; set; }
        public string Id
        {
            get => Structure?.Id ?? "<none>";
            set
            {

            }
        }
        public bool IsAssigned => Structure != null;
        public string DicomType => Structure?.DicomType ?? "UNKNOWN";
        public bool IsSupport => SupportStructureDicomTypes.Contains(DicomType);
        public bool CanOptimize => !NonOptimizedStructureDicomTypes.Contains(DicomType);
        public override string ToString() => Id;
    }
}
