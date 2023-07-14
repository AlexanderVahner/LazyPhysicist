using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class StructureVariableVM
    {
        private readonly StructureVariableVM structureVar;

        public StructureVariableVM(StructureVariableVM structureVar)
        {
            this.structureVar = structureVar;
        }

        public void CreateUIElement()
        {
            UIElement = new StackPanel();
        }

        public UIElement UIElement { get; set; }
    }
}
