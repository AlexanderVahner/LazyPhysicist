using LazyContouring.Images;
using LazyContouring.Operations;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class NotOperationVM : OperationVM
    {
        public NotOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("NotOperation.png")
            };
        }
    }

}
