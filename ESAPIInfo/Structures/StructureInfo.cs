using System.Linq;
using System.Windows.Media;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Structures
{
    public sealed class StructureInfo : IStructureInfo
    {
        public static readonly string[] TargetNames = { "PTV", "CTV", "GTV", "BOOST" };
        public static readonly string[] SupportStructureDicomTypes = { "SUPPORT", "FIXATION", "ARTIFACT", "CONTRAST", "REGISTRATION", "UNKNOWN" };
        public static readonly string[] NonOptimizedStructureDicomTypes = { "SUPPORT", "FIXATION", "REGISTRATION", "UNKNOWN" };

        public static bool IsTarget(string structureId)
        {
            return StructureInfo.TargetNames.FirstOrDefault(tn => structureId.StartsWith(tn)) != null;
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
        public string Id => Structure?.Id ?? "<none>";
        public string DicomType => Structure?.DicomType ?? "UNKNOWN";
        public bool IsSupport => SupportStructureDicomTypes.Contains(DicomType);
        public bool IsEmpty => Structure?.IsEmpty ?? true;
        public bool CanOptimize => !IsEmpty && !NonOptimizedStructureDicomTypes.Contains(DicomType);
        public Color Color => Structure?.Color ?? Color.FromRgb(200, 200, 200);
        public override string ToString() => Id;
    }
}
