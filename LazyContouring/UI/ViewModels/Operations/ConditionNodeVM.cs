using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public class ConditionNodeVM : Notifier
    {
        private readonly ConditionNode node;
        private string title;

        public ConditionNodeVM(ConditionNode node)
        {
            this.node = node;
            if (node is ConditionGroup)
            {
                ChildrenConditionNodes = new SlaveCollection<ConditionNode, ConditionNodeVM> { };
                ChildrenConditionNodes.ObeyTheMaster(node.Children, m => GetConditionNodeVM(m), s => s.Node);
            }
        }

        public ConditionNodeVM GetConditionNodeVM(ConditionNode node)
        {
            if (node is ConditionGroup cg)
            {
                var cgVm = new ConditionGroupVM(cg);
                cgVm.RemoveRequest += OnRemoveRequest;
                return cgVm;
            }

            if (node is StructureCondition sc)
            {
                return new StructureConditionVM(sc);
            }

            if (node is DiagnosisCondition dc)
            {
                return new DiagnosisConditionVM(dc);
            }

            if (node is ImageCondition ic)
            {
                return new ImageConditionVM(ic);
            }

            return null;
        }

        public void OnRemoveRequest(object sender, ConditionNodeVM nodeVm)
        {
            nodeVm.RemoveRequest -= OnRemoveRequest;
            Node.Children.Remove(nodeVm.Node);
        }

        public MetaCommand SelfRemoveCommand => new MetaCommand(
            o => RemoveRequest?.Invoke(this, this),
            o => Node != null
        );

        public EventHandler<ConditionNodeVM> RemoveRequest;

        public ConditionNode Node => node;
        public string Title { get => title; set => SetProperty(ref title, value); }
        public SlaveCollection<ConditionNode, ConditionNodeVM> ChildrenConditionNodes { get; private set; }
    }
}
