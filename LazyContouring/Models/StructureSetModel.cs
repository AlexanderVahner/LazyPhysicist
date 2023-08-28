using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class StructureSetModel
    {
        private StructureSet structureSet;
        private readonly Patient patient;

        public StructureSetModel(Patient patient)
        {
            this.patient = patient;

            InitStructureSets();
        }

        private void InitStructureSets()
        {
            StructureSets.Clear();
            foreach (var ss in patient.StructureSets.OrderBy(ss => ss.Id))
            {
                StructureSets.Add(ss);
            }
        }

        private void SetStructureSet(StructureSet structureSet)
        {
            this.structureSet = structureSet;
            Structures.Clear();

            foreach (var structure in structureSet.Structures)
            {
                var strVar = new StructureVariable
                {
                    Structure = structure
                };
                Structures.Add(strVar);
            }
        }

        public void AddStructure(StructureVariable structure)
        {
            patient.BeginModifications();
            if (StructureSet != null &&  structure != null && StructureSet.CanAddStructure(structure.DicomType, structure.StructureId))
            {
                structure.Structure = StructureSet.AddStructure(structure.DicomType, structure.StructureId);
            }
            Structures.Add(structure);
        }

        public void RemoveStructure(StructureVariable structure)
        {
            patient.BeginModifications();
            if (StructureSet != null && structure?.Structure != null && StructureSet.CanRemoveStructure(structure.Structure))
            {
                StructureSet.RemoveStructure(structure.Structure);
                Structures.Remove(structure);
            }
        }

        public StructureSet DuplicateStructureSet(StructureSet structureSet)
        {
            patient.BeginModifications();
            var newSs = structureSet.Copy();
            InitStructureSets();            
            return newSs;
        }

        public StructureSet StructureSet { get => structureSet; set => SetStructureSet(value); }
        public ObservableCollection<StructureSet> StructureSets { get; } = new ObservableCollection<StructureSet>();
        public ObservableCollection<StructureVariable> Structures { get; } = new ObservableCollection<StructureVariable>();
    }
}
