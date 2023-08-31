using LazyContouring.Images;
using LazyContouring.Operations;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class SubOperationVM : OperationVM
    {
        public SubOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image() { 
                Width = defaultImageWidth, 
                Height = defaultImageHeight, 
                Source = ImageLoader.GetImage("SubOperation.png") };
        }
    }

}
