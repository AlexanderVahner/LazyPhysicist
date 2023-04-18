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
        public StructuresBroker(IEnumerable<IStructureSuggestionModel> structuresCollection)
        {
            UndefinedStructures = new ObservableCollection<IStructureSuggestionModel>();
            StructureSuggestions = new ObservableCollection<IStructureSuggestionModel>();
            StructureSuggestions.Insert(0, new StructureSuggestionModel(null)); // Suggestions have <none> suggestion
            foreach (var structure in structuresCollection)
            {
                UndefinedStructures.Add(structure);
                StructureSuggestions.Add(structure);
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
