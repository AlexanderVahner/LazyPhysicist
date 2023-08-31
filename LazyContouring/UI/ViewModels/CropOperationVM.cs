using LazyContouring.Images;
using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LazyContouring.UI.ViewModels
{
    public sealed class CropOperationVM : OperationVM
    {
        private BitmapImage insideImage = ImageLoader.GetImage("CropInnerOperation.png");
        private BitmapImage outsideImage = ImageLoader.GetImage("CropOuterOperation.png");
        private BitmapImage bitmapImage;
        private string selectedCropPart;
        private CropOperation cropOperation;
        private bool insideChecked;
        private bool outsideChecked;

        public CropOperationVM(OperationNode node, Border border) : base(node, border)
        {
            cropOperation = node.Operation as CropOperation;
            insideChecked = cropOperation.CropPart == CropPart.Inside;
            outsideChecked = !insideChecked;
            UpdateImage();
        }

        protected override void InitBorder(Border border)
        {
            var cropControl = new CropOperationControl() { DataContext = this };
            border.Child = cropControl;
        }

        private void UpdateImage()
        {
            BitmapImage = cropOperation.CropPart == CropPart.Inside ? insideImage : outsideImage;
        }
        private void UpdateCropPart()
        {
            cropOperation.CropPart = insideChecked ? CropPart.Inside : CropPart.Outside;
            UpdateImage();
        }
        public BitmapImage BitmapImage { get => bitmapImage; set => SetProperty(ref bitmapImage, value); }
        public bool InsideChecked { 
            get => insideChecked;
            set
            {
                SetProperty(ref insideChecked, value);
                UpdateCropPart();
            }
        }
        public bool OutsideChecked { 
            get => outsideChecked;
            set
            {
                SetProperty(ref outsideChecked, value);
                UpdateCropPart();
            }
        }
        public CropOperation CropOperation => cropOperation;
    }

}
