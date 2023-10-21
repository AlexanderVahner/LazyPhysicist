using LazyContouring.Operations;
using LazyContouring.Operations.ContextConditions;
using LazyContouring.UI.Views.ContextConditionControls;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class ConditionNodeVM
    {
        private readonly ConditionTreeNode node;

        public ConditionNodeVM(ConditionTreeNode node, ConditionTreeNode parentGroup)
        {
            this.node = node;
            ParentGroup = parentGroup;
            if (node is ConditionGroup)
            {
                ChildrenConditionNodes = new SlaveCollection<ConditionTreeNode, ConditionNodeVM> { };
                ChildrenConditionNodes.ObeyTheMaster(node.Children, m => new ConditionNodeVM(m, Node), s => s.Node);
            }
            UIElement = GetUIElement();
        }

        public UIElement GetUIElement()
        {
            if (node is ConditionGroup cg)
            {
                var viewModel = new ConditionGroupVM(cg);
                return new ConditionGroupControl { DataContext = viewModel };
            }

            if (node is StructureCondition sc)
            {
                var viewModel = new StructureConditionVM(sc);
                return new StructureConditionControl { DataContext = viewModel };
            }

            if (node is DiagnosisCondition dc)
            {
                var viewModel = new DiagnosisConditionVM(dc);
                return new DiagnosisConditionControl { DataContext = viewModel };
            }

            if (node is ImageCondition ic)
            {
                var viewModel = new ImageConditionVM(ic);
                return new ImageConditionControl { DataContext = viewModel };
            }

            return null;
        }

        public ConditionTreeNode Node => node;
        public SlaveCollection<ConditionTreeNode, ConditionNodeVM> ChildrenConditionNodes { get; private set; }
        public UIElement UIElement { get; private set; }
        public ConditionTreeNode ParentGroup { get; }
    }
}
