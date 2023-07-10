using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.UI.ViewModels
{
    public sealed class MainVM : Notifier
    {
        public int W;
        public int H;
        public int D;
        int[,] buffer;
        byte[] pixelBuffer;

        WriteableBitmap writeableBitmap;
        public MainVM()
        {



        }
        public void Init()
        {
            W = CTImage.XSize;
            H = CTImage.YSize;
            D = CTImage.ZSize;

            writeableBitmap = new WriteableBitmap(
                W,
                H,
                96,
                96,
                PixelFormats.Bgra32,
                null);

            buffer = new int[W, H];
            pixelBuffer = new byte[W * H * 4];
        }

        public void PaintSlice(int slice)
        {
            if (slice > D - 1 || slice < 0)
            {
                return;
            }

            CTImage.GetVoxels(slice, buffer);

            try
            {
                // Reserve the back buffer for updates.
                writeableBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    for (int y = 0; y < H; y++)
                    {
                        for (int x = 0; x < W; x++)
                        {

                            Int32 value = buffer[x, y];
                            var c = ColorFromHU(buffer[x, y]);
                            value = c.R;

                            //pBackBuffer += y * writeableBitmap.BackBufferStride;
                            //pBackBuffer += x * 4;

                            int color_data = 255 << 24; // A
                            color_data |= value << 16; // R
                            color_data |= value << 8;   // G
                            color_data |= value << 0;   // B
                            
                            //color_data = 256;

                            // Assign the color data to the pixel.
                            *((int*)pBackBuffer) = color_data;

                            pBackBuffer += 4;

                            // Specify the area of the bitmap that changed.
                            writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                        }
                    }
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }
            //------------------------------------------------------------------------------------------------------------

            /*for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    Color color = ColorFromHU(buffer[x, y]);
                    int offset = (y * W + x) * 4;
                    if (offset > W * H * 4 + 4)
                    {
                        continue;
                    }
                    pixelBuffer[offset] = color.B;
                    pixelBuffer[offset + 1] = color.G;
                    pixelBuffer[offset + 2] = color.R;
                    pixelBuffer[offset + 3] = color.A;
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), pixelBuffer, writeableBitmap.PixelWidth * writeableBitmap.Format.BitsPerPixel / 8, 0);
            */

            //------------------------------------------------------------------------------------------------------------
            /*for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {

                    Int32 value = buffer[x, y];
                    var c = ColorFromHU(buffer[x, y]);
                    value = c.R;

                    pBackBuffer += y * writeableBitmap.BackBufferStride;
                    pBackBuffer += x * 4;

                    int color_data = value << 16; // R
                    color_data |= value << 8;   // G
                    color_data |= value << 0;   // B

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;

                    // Specify the area of the bitmap that changed.
                    writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                }
            }

            for (int i = 0; i < 1000; i++)
            {
                var x = _random.Next(bitmap.Width);
                var y = _random.Next(bitmap.Height);

                var red = (byte)_random.Next(byte.MaxValue);
                var green = (byte)_random.Next(byte.MaxValue);
                var blue = (byte)_random.Next(byte.MaxValue);
                var alpha = (byte)_random.Next(byte.MaxValue);

                _buffer[y * bitmap.BackBufferStride + x * 4] = blue;
                _buffer[y * bitmap.BackBufferStride + x * 4 + 1] = green;
                _buffer[y * bitmap.BackBufferStride + x * 4 + 2] = red;
                _buffer[y * bitmap.BackBufferStride + x * 4 + 3] = alpha;
            }


            */
            /*try
            {
                // Reserve the back buffer for updates.
                writeableBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    for (int y = 0; y < H; y++)
                    {
                        for (int x = 0; x < W; x++)
                        {

                            Int32 value = buffer[x, y];
                            var c = ColorFromHU(buffer[x, y]);
                            value = c.R;

                            pBackBuffer += x * writeableBitmap.BackBufferStride;
                            pBackBuffer += y * 4;

                            int color_data = value << 16; // R
                            color_data |= value << 8;   // G
                            color_data |= value << 0;   // B

                            // Assign the color data to the pixel.
                            *((int*)pBackBuffer) = color_data;

                            // Specify the area of the bitmap that changed.
                            writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                        }
                    }
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }*/
        }
        public Color ColorFromHU(int hu)
        {
            Color result = Color.FromRgb(0, 0, 0);
            int minHu = 500;
            int maxHu = 2000;

            if (hu < minHu)
            {
                return result;
            }

            if (hu > maxHu)
            {
                return Color.FromRgb(255, 255, 255);
            }

            byte c = (byte)((hu - minHu) * (double)255 / (maxHu - minHu));
            return Color.FromRgb(c, c, c);
        }
        public StructureSet StructureSet { get; set; }
        public Structure Structure => StructureSet.Structures.FirstOrDefault(s => s.DicomType == "EXTERNAL");
        public Structure StructureBrain => StructureSet.Structures.FirstOrDefault(s => s.Id == "Brain");

        public MeshGeometry3D Mesh => Structure.MeshGeometry;
        public MeshGeometry3D MeshBrain => StructureBrain.MeshGeometry;

        public Image CTImage => StructureSet.Image;
        private int currentSlice;
        public int CurrentSlice
        {
            get => currentSlice;
            set
            {
                if (currentSlice == value || value < 0 || value > D)
                {
                    return;
                }

                PaintSlice(value);
                SetProperty(ref currentSlice, value);
            }
        }
        public WriteableBitmap CTSlice => writeableBitmap;

        private Point3D cameraPosition = new Point3D(0, 0, 1000);
        public Point3D CameraPosition { get => cameraPosition; set => SetProperty(ref cameraPosition, value); }

        private Vector3D lookDirection = new Vector3D(0, 0, -1);
        

        public Vector3D LookDirection { get => lookDirection; set => SetProperty(ref lookDirection, value); }


    }

    public sealed class CTStore
    {
        private readonly StructureSet structureSet;

        public CTStore(StructureSet structureSet)
        {
            this.structureSet = structureSet;
        }
    }
}
