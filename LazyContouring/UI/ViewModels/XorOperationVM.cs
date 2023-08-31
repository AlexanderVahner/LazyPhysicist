using LazyContouring.Images;
using LazyContouring.Operations;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class XorOperationVM : OperationVM
    {
        public XorOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("XorOperation.png")
            };
        }
    }

}
