using ESAPIInfo.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.UI.ViewModels
{
    public class StructureVM : ViewModel
    {
        private StructureInfo apiStructure;
        private StructureInfo apiStructureHack;

        private ObservableCollection<ObjectiveVM> objectives;
        public string DBStructureId { get; set; }
        public StructureInfo APIStructure
        {
            get => apiStructure;
            set
            {
                if (!Equals(apiStructure, value) && value != null)
                {
                    if (apiStructure?.Structure != null)
                    {
                        StructureSuggestions.Add(apiStructure);
                    }
                    
                    if (value?.Structure != null)
                    {
                        StructureSuggestions.Remove(value);
                    }
                    SetProperty(ref apiStructure, value);
                }
            }

        }

        // Buffer for combobox
        public StructureInfo APIStructureHack
        {
            get => apiStructureHack;
            set
            {
                if (!Equals(apiStructureHack, value))
                {
                    SetProperty(ref apiStructureHack, value);
                    if (value != null)
                    {
                        APIStructure = value;
                    }
                }
            }
        }
        public ObservableCollection<StructureInfo> StructureSuggestions { get; set; }
        public ObservableCollection<ObjectiveVM> Objectives => objectives ?? (objectives = new ObservableCollection<ObjectiveVM>());
        public bool IsTarget => StructureInfo.IsTarget(DBStructureId);
        public double MaxObjectiveDose => objectives?.Max(o => o?.ObjectiveDB?.Dose ?? .0) ?? .0;
        public double OrderByDoseDescProperty => MaxObjectiveDose + (IsTarget ? 1000 : 0);
    }
}
