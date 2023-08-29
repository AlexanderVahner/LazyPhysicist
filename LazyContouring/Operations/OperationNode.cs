﻿using LazyContouring.Models;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public sealed class OperationNode
    {
        private SegmentVolume segmentVolume;
        private void Materialize(StructureSet structureSet)
        {
            NodeLeft?.Materialize(structureSet);
            NodeRight?.Materialize(structureSet);

            if (Operation.StructureNedded)
            {
                FindOrCreateStructure(structureSet);
            }

            Operation.Perform(this);
        }

        public void InsertNodeBefore(OperationNode insertNode, ref OperationNode beforeThisNode)
        {
            insertNode.NodeLeft = beforeThisNode;
            beforeThisNode = insertNode;
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

        public SegmentVolume SegmentVolume 
        { 
            get => segmentVolume ?? StructureVar?.SegmentVolume; 
            set => segmentVolume = value; 
        }

        public bool IsRootNode { get; set; } = false;
        public StructureVariable StructureVar { get; set; }
        public Operation Operation { get; set; }
        public OperationNode NodeLeft { get; set; }
        public OperationNode NodeRight { get; set; }
    }
}
