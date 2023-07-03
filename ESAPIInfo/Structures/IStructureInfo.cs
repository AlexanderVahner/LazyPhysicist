using System.Windows.Media;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Structures
{
    public interface IStructureInfo
    {
        bool IsTarget();
        Structure Structure { get; set; }
        string Id { get; }
        string DicomType { get; }
        bool IsSupport { get; }
        bool IsEmpty { get; }
        bool CanOptimize { get; }
        Color Color { get; }
    }
}
