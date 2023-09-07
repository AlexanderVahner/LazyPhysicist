﻿using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class MarginOperationVM : OperationVM
    {
        public MarginOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new MarginOperationControl() { DataContext = Node.Operation as MarginOperation };
        }
    }

}