using System.Collections.ObjectModel;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Graphics
{
    public sealed class StructureSetStorage
    {
        private readonly StructureSet structureSet;
        private readonly ObservableCollection<StructureStorage> structures = new ObservableCollection<StructureStorage>();

        public StructureSetStorage(StructureSet structureSet)
        {
            this.structureSet = structureSet;
            foreach (var structure in structureSet.Structures)
            {
                structures.Add(new StructureStorage(new ContourStorage(structure, structureSet.Image)));
            }
        }

        public void RepaintSlice(int planeIndex)
        {
            foreach (var structure in Structures)
            {
                structure.RepaintPath(planeIndex);
            }
        }

        public StructureSet StructureSet => structureSet;
        public Image Image => structureSet.Image;

        public ObservableCollection<StructureStorage> Structures => structures;
    }
}
