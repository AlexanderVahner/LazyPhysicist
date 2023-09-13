using LazyContouring.Graphics;
using LazyContouring.Models;
using LazyPhysicist.Common;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.Types;

namespace LazyContouring.UI.ViewModels
{
    public sealed class ViewPlaneVM : Notifier
    {
        private StructureSetModel structureSet;
        private int currentPlaneIndex;
        private ImageModel imageModel;
        private readonly SlaveCollection<StructureVariable, ContourModel> contours = new SlaveCollection<StructureVariable, ContourModel>();
        private int planeCount;
        private WriteableBitmap planeBitmap;
        private Canvas planeCanvas;
        private Image planeImage;

        private void SetStructureSet(StructureSetModel ss)
        {
            structureSet = ss;
            ImageModel = new ImageModel(structureSet.StructureSet.Image) { Converter = new VoxelToPixelConverter() };
            PlaneCount = ImageModel.ZSize;
            PlaneBitmap = ImageModel.PlaneBitmap;
            

            planeImage = new Image()
            {
                Source = PlaneBitmap,
                Width = ImageModel.XSize,
                Height = ImageModel.YSize
            };

            PlaneCanvas = new Canvas() { Width = ImageModel.XSize, Height = ImageModel.YSize };
            PlaneCanvas.Children.Add(planeImage);

            contours.CollectionChanged += Contours_CollectionChanged;
            contours.ObeyTheMaster(structureSet.Structures, m => CreateContour(m), s => s.Structure);

            NotifyPropertyChanged(nameof(structureSet));
        }

        private void Contours_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var contour in e.NewItems.OfType<ContourModel>())
                    {
                        PlaneCanvas.Children.Add(contour.Path);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var contour in e.OldItems.OfType<ContourModel>())
                    {
                        PlaneCanvas.Children.Remove(contour.Path);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PlaneCanvas.Children.Clear();
                    PlaneCanvas.Children.Add(planeImage);
                    break;
            }
        }

        private ContourModel CreateContour(StructureVariable structure)
        {
            var newContour = new ContourModel(structure, ImageModel);

            return newContour;
        }

        private void SetPlane(int value)
        {
            if (value < 0)
            {
                currentPlaneIndex = 0;
            }
            else if (value >= PlaneCount)
            {
                currentPlaneIndex = PlaneCount - 1;
            }
            else
            {
                currentPlaneIndex = value;
            }

            ImageModel.CurrentPlaneIndex = currentPlaneIndex;

            foreach (var contour in contours)
            {
                contour.CurrentPlaneIndex = currentPlaneIndex;
            }

            NotifyPropertyChanged(nameof(CurrentPlaneIndex));
        }

        public int PlaneIndexOf(double z)
        {
            return (int)((z - ImageModel.Origin.z) / ImageModel.ZRes);
        }

        public StructureSetModel StructureSet { get => structureSet; set => SetStructureSet(value); }
        public int CurrentPlaneIndex { get => currentPlaneIndex; set => SetPlane(value); }
        public ImageModel ImageModel { get => imageModel; private set => SetProperty(ref imageModel, value); }
        public int PlaneCount { get => planeCount; private set => SetProperty(ref planeCount, value); }
        public WriteableBitmap PlaneBitmap { get => planeBitmap; private set => SetProperty(ref planeBitmap, value); }
        public Canvas PlaneCanvas { get => planeCanvas; private set => SetProperty(ref planeCanvas, value); }
    }
}
