using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    /// <summary>
    /// Structure manager of the current plan, which modifies lists of suggestions and undefined structures.
    /// </summary>
    public sealed class StructuresBroker
    {
        private readonly List<IStructureSuggestionModel> structuresCollection;
        public StructuresBroker(IEnumerable<IStructureSuggestionModel> structuresCollection)
        {
            this.structuresCollection = structuresCollection.ToList();
            UndefinedStructures = new ObservableCollection<IStructureSuggestionModel>();
            StructureSuggestions = new ObservableCollection<IStructureSuggestionModel>();
            Reset();
        }
        public void Reset(IEnumerable<IStructureModel> structures = null)
        {
            UndefinedStructures.Clear();
            StructureSuggestions.Clear();
            StructureSuggestions.Insert(0, new StructureSuggestionModel(null)); // Suggestions have <none> suggestion
            foreach (var structure in structuresCollection)
            {
                UndefinedStructures.Add(structure);
                StructureSuggestions.Add(structure);
            }
            if (structures != null)
            {
                foreach (var structure in structures)
                {
                    Take(structure.CurrentPlanStructure);
                }
            }
        }
        public void Give(IStructureSuggestionModel giveStructure)
        {
            if (giveStructure?.StructureInfo != null)
            {
                UndefinedStructures.Insert(0, giveStructure);
                StructureSuggestions.Insert(1, giveStructure); // after <none> item
            }
        }
        public void Take(IStructureSuggestionModel takeStructure)
        {
            if (takeStructure?.StructureInfo != null)
            {
                UndefinedStructures.Remove(takeStructure);
                StructureSuggestions.Remove(takeStructure);
            }
        }
        public ObservableCollection<IStructureSuggestionModel> UndefinedStructures { get; }
        public ObservableCollection<IStructureSuggestionModel> StructureSuggestions { get; }
    }
}
