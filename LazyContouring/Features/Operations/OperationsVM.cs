using LazyContouring.Operations;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationsVM : Notifier
    {
        private ObservableCollection<OperationStringVM> operations;
        public OperationStringVM AddOperationString(OperationNode node)
        {
            var opString = new OperationStringVM() { Node = node, StructureSetVM = null }; // Can be deleted
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

        public IEnumerable<OperationNode> GetCurrentNodes()
        {
            foreach (var opStringVM in Operations)
            {
                yield return opStringVM.Node;
            }
        }

        public ObservableCollection<OperationStringVM> Operations 
        { 
            get => operations ?? (operations = new ObservableCollection<OperationStringVM>()); 
            set => SetProperty(ref operations, value); 
        }
    }
}
