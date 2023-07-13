using System.Windows.Media;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Graphics
{
    public sealed class ContourStorage
    {
        private readonly Structure structure;
        private readonly Image image;
        private readonly int xSize;
        private readonly int ySize;
        private readonly int zSize;

        private readonly ContourSlice[] contourSlices;

        public ContourStorage(Structure structure, Image image)
        {
            this.structure = structure;
            this.image = image;
            xSize = image.XSize;
            ySize = image.YSize;
            zSize = image.ZSize;

            contourSlices = new ContourSlice[zSize];
        }

        public PathFigure[] GetSegments(int planeIndex)
        {
            if (contourSlices[planeIndex] == null)
            {
                contourSlices[planeIndex] = new ContourSlice(Structure, image, planeIndex);
            }

            return contourSlices[planeIndex].Segments;
        }

        public int XSize { get => xSize; }
        public int YSize { get => ySize; }
        public int ZSize { get => zSize; }

        public Image Image => image;
        public Structure Structure => structure;
    }
}

