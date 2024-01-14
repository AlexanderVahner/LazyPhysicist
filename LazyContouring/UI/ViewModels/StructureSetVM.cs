using LazyContouring.Models;
using LazyContouring.Operations;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels
{
    public sealed class StructureSetVM : Notifier
    {
        private readonly StructureSetModel structureSetModel;
        private ObservableCollection<OperationStringVM> operations;
        private SlaveCollection<StructureVariable, StructureVariableVM> structures;

        public StructureSetVM(StructureSetModel structureSetModel)
        {
            this.structureSetModel = structureSetModel;
        }

        
        public OperationStringVM AddOperationString(OperationNode node)
        {
            var opString = new OperationStringVM() { Node = node, StructureSetVM = this };
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

        public override string ToString() => structureSetModel.Id;

        private SlaveCollection<StructureVariable, StructureVariableVM> InitStructures() =>
            new SlaveCollection<StructureVariable, StructureVariableVM>(
                structureSetModel.Structures, 
                m => new StructureVariableVM(m), 
                s => s.StructureVariable);

        public string Id => structureSetModel.Id;
        public StructureSetModel StructureSetModel => structureSetModel;
        public SlaveCollection<StructureVariable, StructureVariableVM> Structures => structures ?? (structures = InitStructures());
        public ObservableCollection<OperationStringVM> Operations => operations ?? (operations = new ObservableCollection<OperationStringVM>());
    }
}
