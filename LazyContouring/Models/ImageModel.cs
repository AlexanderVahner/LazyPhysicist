using LazyContouring.Graphics;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class ImageModel
    {
        private readonly Image image;
        private readonly int[,] voxelBuffer;
        private readonly int xSize;
        private readonly int ySize;
        private readonly int zSize;
        private readonly WriteableBitmap bitmap;
        private int currentPlaneIndex;

        public ImageModel(Image image)
        {
            this.image = image;
            if (image != null)
            {
                xSize = image.XSize;
                ySize = image.YSize;
                zSize = image.ZSize;
                voxelBuffer = new int[xSize, ySize];

                bitmap = new WriteableBitmap(
                    xSize,
                    ySize,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null);
            }
        }

        private void SetPlane(int index)
        {
            if (index < 0)
            {
                currentPlaneIndex = 0;
            }
            else if (index >= zSize)
            {
                currentPlaneIndex = zSize - 1;
            }
            else
            {
                currentPlaneIndex = index;
            }

            RepaintBitmap(currentPlaneIndex);
        }

        public void RepaintBitmap(int index)
        {
            Image.GetVoxels(index, voxelBuffer);

            try
            {
                // Reserve the back buffer for updates.
                PlaneBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = PlaneBitmap.BackBuffer;

                    for (int y = 0; y < ySize; y++)
                    {
                        for (int x = 0; x < xSize; x++)
                        {
                            *((int*)pBackBuffer) = Converter.VoxelToBgra32(voxelBuffer[x, y]);

                            pBackBuffer += 4;

                            // Specify the area of the bitmap that changed.
                            //SliceBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                        }
                    }
                    // Specify the area of the bitmap that changed.
                    PlaneBitmap.AddDirtyRect(new Int32Rect(0, 0, xSize, ySize));
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                PlaneBitmap.Unlock();
            }
        }

        public int XSize { get => xSize; }
        public int YSize { get => ySize; }
        public int ZSize { get => zSize; }
        public IVoxelToPixelConverter Converter { get; set; } = new VoxelToPixelConverter();
        public int CurrentPlaneIndex { get => currentPlaneIndex; set => SetPlane(value); }

        public WriteableBitmap PlaneBitmap => bitmap;
    }
}
