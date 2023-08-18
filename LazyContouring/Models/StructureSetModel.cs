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

        public StructureSetModel(StructureSet structureSet)
        {
            StructureSet = structureSet;
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
        public StructureSet StructureSet { get => structureSet; set => SetStructureSet(value); }

        public ObservableCollection<StructureVariable> Structures { get; } = new ObservableCollection<StructureVariable>();
    }
}
