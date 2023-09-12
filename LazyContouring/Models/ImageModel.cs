using LazyContouring.Graphics;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyContouring.Models
{
    public sealed class ImageModel
    {
        private readonly Image image;
        private readonly int[,] voxelBuffer;
        private readonly WriteableBitmap bitmap;
        private int currentPlaneIndex;
        private readonly int xSize;
        private readonly int ySize;
        private readonly int zSize;
        private readonly double xRes;
        private readonly double yRes;
        private readonly double zRes;
        private readonly VVector origin;

        public ImageModel(Image image)
        {
            this.image = image;
            xSize = image.XSize;
            ySize = image.YSize;
            zSize = image.ZSize;
            xRes = image.XRes;
            yRes = image.YRes;
            zRes = image.ZRes;
            origin = image.Origin;

            voxelBuffer = new int[xSize, ySize];

            bitmap = new WriteableBitmap(
                xSize,
                ySize,
                96,
                96,
                PixelFormats.Bgra32,
                null);
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

            RepaintBitmap();
        }

        public void RepaintBitmap()
        {
            try
            {
                // Reserve the back buffer for updates.
                bitmap.Lock();
                image.GetVoxels(currentPlaneIndex, voxelBuffer);

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = bitmap.BackBuffer;

                    for (int y = 0; y < xSize; y++)
                    {
                        for (int x = 0; x < ySize; x++)
                        {
                            *((int*)pBackBuffer) = Converter.VoxelToBgra32(voxelBuffer[x, y]);
                            pBackBuffer += 4;
                        }
                    }
                    // Specify the area of the bitmap that changed.
                    bitmap.AddDirtyRect(new Int32Rect(0, 0, xSize, ySize));
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                PlaneBitmap.Unlock();
            }
        }

        public int XSize => xSize;
        public int YSize => ySize;
        public int ZSize => zSize;
        public double XRes => xRes;
        public double YRes => yRes;
        public double ZRes => zRes;
        public VVector Origin => origin;
        public IVoxelToPixelConverter Converter { get; set; }
        public int CurrentPlaneIndex { get => currentPlaneIndex; set => SetPlane(value); }

        public WriteableBitmap PlaneBitmap => bitmap;
    }
}
