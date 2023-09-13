using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationsVM
    {
        public OperationStringVM AddOperationString(OperationNode node)
        {
            var opString = new OperationStringVM() { Node = node, OperationsVM = this };
            Operations.Add(opString);
            return opString;
        }

        public void RemoveOperationString(OperationStringVM value)
        {
            Operations.Remove(value);
        }

        public void DuplicateOperationString(OperationStringVM value)
        {
            var newOpString = AddOperationString((OperationNode)value.Node.Clone());
            int index = Operations.IndexOf(value);
            if (index < Operations.Count - 1)
            {
                Operations.Move(Operations.IndexOf(newOpString), index + 1);
            }
        }

        public void MoveOperationStringUp(OperationStringVM value)
        {
            int index = Operations.IndexOf(value);
            if (index > 0)
            {
                Operations.Move(index, index - 1);
            } 
        }

        public void MoveOperationStringDown(OperationStringVM value)
        {
            int index = Operations.IndexOf(value);
            if (index < Operations.Count - 1)
            {
                Operations.Move(index, index + 1);
            }
        }

        public ObservableCollection<OperationStringVM> Operations { get; private set; } = new ObservableCollection<OperationStringVM>();
    }
}
