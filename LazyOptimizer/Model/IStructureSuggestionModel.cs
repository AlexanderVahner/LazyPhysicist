using ESAPIInfo.Structures;

namespace LazyOptimizer.Model
{
    public interface IStructureSuggestionModel
    {
        string Id { get; }
        IStructureInfo StructureInfo { get; }
    }
}