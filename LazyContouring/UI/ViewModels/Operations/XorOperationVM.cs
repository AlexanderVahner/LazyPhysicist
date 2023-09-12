using LazyContouring.Images;
using LazyContouring.Operations;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class XorOperationVM : OperationVM
    {
        public XorOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("XorOperation.png")
            };
        }
    }

}
