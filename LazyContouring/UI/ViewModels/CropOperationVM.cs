using LazyContouring.Images;
using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Media.Imaging;

namespace LazyContouring.UI.ViewModels
{
    public sealed class CropOperationVM : OperationVM
    {
        private readonly BitmapImage insideImage = ImageLoader.GetImage("CropInnerOperation.png");
        private readonly BitmapImage outsideImage = ImageLoader.GetImage("CropOuterOperation.png");
        private BitmapImage bitmapImage;
        private readonly CropOperation cropOperation;
        private bool insideChecked;
        private bool outsideChecked;

        public CropOperationVM(OperationNode node) : base(node)
        {
            cropOperation = node.Operation as CropOperation;
            insideChecked = cropOperation.CropPart == CropPart.Inside;
            outsideChecked = !insideChecked;
            UpdateImage();
        }

        protected override void InitUIElement()
        {
            UIElement = new CropOperationControl() { DataContext = this };
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
        public bool InsideChecked
        {
            get => insideChecked;
            set
            {
                SetProperty(ref insideChecked, value);
                outsideChecked = !insideChecked;
                NotifyPropertyChanged(nameof(OutsideChecked));
                UpdateCropPart();
            }
        }
        public bool OutsideChecked
        {
            get => outsideChecked;
            set
            {
                SetProperty(ref outsideChecked, value);
                insideChecked = !outsideChecked;
                NotifyPropertyChanged(nameof(InsideChecked));
                UpdateCropPart();
            }
        }
        public CropOperation CropOperation => cropOperation;
    }

}
