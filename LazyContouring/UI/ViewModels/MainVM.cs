using LazyContouring.Graphics;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using V = VMS.TPS.Common.Model.API;

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
        ImageStorage imageStorage;
        ContourStorage bodyStorage;
        VoxelsToPixels converter;

        SliceCanvas sliceCanvas;
        public MainVM()
        {



        }
        public void Init()
        {
            
            sliceCanvas = new SliceCanvas(new StructureSetStorage(StructureSet), new ImageStorage(StructureSet.Image));
            var sliceVm = new SliceVM(sliceCanvas);
            SliceControl = new SliceControl() { DataContext = sliceVm };
            SliceControl.ViewModel = sliceVm;


            /*
            sliceCanvas.CurrentSlice = 100;

            W = CTImage.XSize;
            H = CTImage.YSize;
            D = CTImage.ZSize;

            imageStorage = new ImageStorage(StructureSet.Image);
            converter = new VoxelsToPixels();

            writeableBitmap = new WriteableBitmap(
                W,
                H,
                96,
                96,
                PixelFormats.Bgra32,
                null);

            pixelBuffer = new byte[W * H * 4];

            bodyStorage = new ContourStorage(BodyStructure, imageStorage.Image);
            pathBody = new PathGeometry();*/
        }

        public void PaintSlice(int slice)
        {
            sliceCanvas.CurrentSlice = slice;
            /*if (slice > D - 1 || slice < 0)
            {
                return;
            }

            PaintBody(slice);

            buffer = imageStorage.GetVoxels(slice);
            //CTImage.GetVoxels(slice, buffer);

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
                            *((int*)pBackBuffer) = converter.GetBrga32(buffer[x, y]);

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
            }*/

        }

        public SliceControl SliceControl { get; private set; }
        public StructureSet StructureSet { get; set; }
        public SliceCanvas SliceCanvas => sliceCanvas;
        public Structure BodyStructure => StructureSet.Structures.FirstOrDefault(s => s.DicomType == "EXTERNAL");
        public Structure StructureBrain => StructureSet.Structures.FirstOrDefault(s => s.Id == "Brain");



        public MeshGeometry3D Mesh => BodyStructure.MeshGeometry;
        public MeshGeometry3D MeshBrain => StructureBrain.MeshGeometry;

        public V.Image CTImage => StructureSet.Image;
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

        public int Width => W;
        public int Height => H;

        
    }
}

