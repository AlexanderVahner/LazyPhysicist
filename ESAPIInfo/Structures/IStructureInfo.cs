using System.Linq;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Structures
{
    public interface IStructureInfo
    {
        bool IsTarget();
        Structure Structure { get; set; }
        string Id { get; }
        bool IsAssigned { get; }
        string DicomType { get; }
        bool IsSupport { get; }
        bool CanOptimize { get; }
}
}
