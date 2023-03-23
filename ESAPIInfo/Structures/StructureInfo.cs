using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Structures
{
    public class StructureInfo
    {
        public static readonly string[] TargetNames = { "PTV", "CTV", "GTV", "BOOST" };
        public static readonly string[] SkipStructureDicomTypes = { "EXTERNAL", "SUPPORT", "FIXATION", "ARTIFACT", "CONTRAST", "REGISTRATION" };
        public static bool IsTarget(Structure structure)
        {
            return StructureInfo.IsTarget(structure?.Id ?? "");
        }

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
        public Structure Structure { get; set; }
        public string Id
        {
            get => Structure?.Id ?? "<none>";
            set
            {

            }
        }
            
        public override string ToString() => Id;
    }
}
