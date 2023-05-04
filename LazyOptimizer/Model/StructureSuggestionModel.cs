using ESAPIInfo.Structures;

namespace LazyOptimizer.Model
{
    public sealed class StructureSuggestionModel : IStructureSuggestionModel
    {
        public StructureSuggestionModel(IStructureInfo structureInfo)
        {
            StructureInfo = structureInfo;
        }

        public IStructureInfo StructureInfo { get; }
        public string Id
        {
            get => StructureInfo?.Id ?? "<none>";
            set { } // Needs for wpf ComboBox.Text binding
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
