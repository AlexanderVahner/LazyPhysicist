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

        public void LoadOperationStringsFromTempalte(OperationTemplate template)
        {
            foreach (var node in template.OperationNodes) 
            {
                var newNode = (OperationNode)node.Clone();
                AddOperationString(newNode);                
            }

            var allNodes = Operations.SelectMany(opVM => opVM.Node.GetAllNodes().Where(n => n.StructureVar != null)).ToList();

            foreach (var structure in structureSetModel.Structures)
            {
                var searchId = structure.StructureId.ToUpper().Replace(" ", "").Replace("-", "").Replace("_", "");
                var firstFound = allNodes.FirstOrDefault(n => searchId == n.StructureVar.StructureId.ToUpper().Replace(" ", "").Replace("-", "").Replace("_", ""));
                if (firstFound != null)
                {
                    var oldId = firstFound.StructureVar.StructureId;
                    foreach (var node in allNodes)
                    {
                        if (node.StructureVar.StructureId == oldId)
                        {
                            node.StructureVar = structure;
                        }
                    }
                }
            }
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
