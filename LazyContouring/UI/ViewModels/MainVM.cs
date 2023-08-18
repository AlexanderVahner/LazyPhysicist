using LazyContouring.Graphics;
using LazyContouring.Models;
using LazyContouring.Operations;
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

        public OperationPage OperationPage { get; set; }
        private OperationsVM operations = new OperationsVM();
        private StructureSetModel structureSetModel;

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

            var node = new OperationNode() 
            {
                IsRootNode = true,
                StructureVar = new StructureVariable { Structure = BodyStructure, StructureId = "BODY" },
                Operation = new AssignOperation(),
                NodeLeft = new OperationNode()
                {
                    Operation = new SubOperation(),
                    NodeLeft = new OperationNode()
                    {
                        Operation = new EmptyOperation(),
                        StructureVar = new StructureVariable { Structure = BodyStructure, StructureId = "BODY" }
                    },
                    NodeRight = new OperationNode()
                    {
                        Operation = new EmptyOperation(),
                        StructureVar = new StructureVariable { Structure = StructureBrain, StructureId = "Brain" }
                    }
                }
            };

            operations.AddOperationString(node);


            OperationPage = new OperationPage() { DataContext = operations };
            OperationPage.ViewModel = operations;
        }

        public void PaintSlice(int slice)
        {
            sliceCanvas.CurrentSlice = slice;

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

        private Point3D cameraPosition = new Point3D(0, 0, 1000);
        public Point3D CameraPosition { get => cameraPosition; set => SetProperty(ref cameraPosition, value); }

        private Vector3D lookDirection = new Vector3D(0, 0, -1);
        

        public Vector3D LookDirection { get => lookDirection; set => SetProperty(ref lookDirection, value); }

        public int Width => W;
        public int Height => H;

        
    }
}

