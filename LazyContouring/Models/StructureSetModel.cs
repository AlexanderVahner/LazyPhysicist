using System.Collections.ObjectModel;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class StructureSetModel
    {
        private StructureSet structureSet;
        private readonly Patient patient;
        private ObservableCollection<StructureVariable> structures;

        public StructureSetModel(StructureSet ss)
        {
            structureSet = ss;
            patient = ss.Patient;
        }

        private ObservableCollection<StructureVariable> InitStructures()
        {
            var structCollection = new ObservableCollection<StructureVariable>();
            foreach (var structure in structureSet.Structures)
            {
                structCollection.Add(new StructureVariable() { Structure = structure });
            }
            return structCollection;
        }

        public StructureSetModel DuplicateStructureSet()
        {
            patient.BeginModifications();
            StructureSetModel result = null;
            if (StructureSet != null)
            {
                var newSs = StructureSet.Copy();
                result = new StructureSetModel(newSs);
            }

            return result;
        }

        public void AddStructure(StructureVariable structure)
        {
            patient.BeginModifications();
            if (structureSet != null && structure != null && structureSet.CanAddStructure(structure.DicomType, structure.StructureId))
            {
                structure.Structure = structureSet.AddStructure(structure.DicomType, structure.StructureId);
                structure.IsNew = true;
                Structures.Add(structure);
            }
        }

        public void RemoveStructure(StructureVariable structure)
        {
            patient.BeginModifications();
            if (structureSet != null && structure?.Structure != null && structureSet.CanRemoveStructure(structure.Structure))
            {
                structureSet.RemoveStructure(structure.Structure);
                Structures.Remove(structure);
            }
        }

        public override string ToString()
        {
            return Id;
        }

        public StructureSet StructureSet => structureSet;
        public string Id => structureSet.Id;
        public ObservableCollection<StructureVariable> Structures => structures ?? (structures = InitStructures());
    }
}
