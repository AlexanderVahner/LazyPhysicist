using LazyContouring.Models;
using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class StructureVariableVM
    {
        private readonly StructureVariable structureVar;
        private readonly Border mainElement;

        public StructureVariableVM(StructureVariable structureVar)
        {
            this.structureVar = structureVar;

            mainElement = new Border() 
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(structureVar.Structure.Color),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(25)
            };

            mainElement.Child = new TextBlock() { Text = structureVar.StructureId };

            UIElement = mainElement;
        }

        /*public void CreateUIElement()
        {
            UIElement = new StackPanel();
        }*/

        public UIElement UIElement { get; set; }
    }
}
