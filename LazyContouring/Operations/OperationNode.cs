﻿using LazyContouring.Models;
using LazyPhysicist.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public enum NodeDirection { Left, Right };

    public sealed class OperationNode : Notifier, ICloneable
    {
        private SegmentVolume segmentVolume;
        private StructureVariable structureVar;
        private Operation operation;
        private OperationNode nodeLeft;
        private OperationNode nodeRight;

        public void Materialize()
        {
            Operation.Execute(this);
        }

        public void InsertNode(OperationNode newNode, NodeDirection direction)
        {
            if (newNode == null)
            {
                return;
            }

            OperationNode nextNode = newNode.Operation.OperationType != OperationType.Empty ?
                (direction == NodeDirection.Left ? NodeLeft : NodeRight) :
                null;

            newNode.NodeLeft = nextNode;

            if (direction == NodeDirection.Left)
            {
                NodeLeft = newNode;
            }
            else
            {
                NodeRight = newNode;
            }


        }

        public void ReplaceNode(OperationNode newNode, NodeDirection direction)
        {
            if (newNode == null) // then removing the node and saving next nodes 
            {
                if (direction == NodeDirection.Left)
                {
                    if (NodeLeft?.NodeLeft != null)
                    {
                        NodeLeft = NodeLeft.NodeLeft;
                    }
                }
                else
                {
                    if (NodeRight?.NodeLeft != null)
                    {
                        NodeRight = NodeRight.NodeLeft;
                    }
                }
            }
            else
            {
                newNode.NodeLeft = direction == NodeDirection.Left ? NodeLeft.NodeLeft : NodeRight.nodeLeft;

                if (newNode.Operation.RightNodeNedded)
                {
                    newNode.NodeRight = direction == NodeDirection.Left ? NodeLeft.NodeRight : NodeRight.NodeRight;
                }

                if (direction == NodeDirection.Left)
                {
                    NodeLeft = newNode;
                }
                else
                {
                    NodeRight = newNode;
                }
            }
        }

        public void DeleteNode(NodeDirection direction)
        {
            if (direction == NodeDirection.Left)
            {
                NodeLeft = null;
            }
            else
            {
                NodeRight = null;
            }
        }

        public void FindOrCreateStructure(StructureSet structureSet)
        {
            if (StructureVar == null)
            {
                StructureVar = new StructureVariable();
            }
            else if (StructureVar.Structure != null)
            {
                return;
            }

            var findedStructure = structureSet.Structures.FirstOrDefault(s => s.Id == StructureVar.StructureId);
            if (findedStructure != null)
            {
                StructureVar.Structure = findedStructure;
            }

            if (structureSet.CanAddStructure(StructureVar.DicomType, StructureVar.StructureId))
            {
                StructureVar.Structure = structureSet.AddStructure(StructureVar.DicomType, StructureVar.StructureId);
            }
        }

        public IEnumerable<OperationNode> GetAllNodes()
        {
            yield return this;
            if (NodeLeft != null)
            {
                foreach (var ln in NodeLeft.GetAllNodes())
                {
                    yield return ln;
                }
            }
            if (NodeRight != null)
            {
                foreach (var rn in NodeRight.GetAllNodes())
                {
                    yield return rn;
                }
            }
        }

        public object Clone()
        {
            return new OperationNode
            {
                IsRootNode = this.IsRootNode,
                StructureVar = this.StructureVar,
                Operation = (Operation)this.Operation.Clone(),
                NodeLeft = (OperationNode)this.NodeLeft?.Clone(),
                NodeRight = (OperationNode)this.NodeRight?.Clone()
            };
        }

        [XmlIgnore]
        public SegmentVolume SegmentVolume
        {
            get => segmentVolume ?? StructureVar?.GetSegmentVolume();
            set => segmentVolume = value;
        }

        public bool IsRootNode { get; set; } = false;
        public StructureVariable StructureVar { get => structureVar; set => SetProperty(ref structureVar, value); }
        public Operation Operation { get => operation; set => SetProperty(ref operation, value); }
        public OperationNode NodeLeft { get => nodeLeft; set => SetProperty(ref nodeLeft, value); }
        public OperationNode NodeRight { get => nodeRight; set => SetProperty(ref nodeRight, value); }
    }
}
