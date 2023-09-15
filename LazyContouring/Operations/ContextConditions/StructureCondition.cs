using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations.ContextConditions
{
    public enum StructureConditionType { ExactId, RegexId, DicomType }
    public sealed class StructureCondition : ContextCondition
    {
        public static readonly List<StructureConditionTypeStruct> StrucutreConditionTypes = new List<StructureConditionTypeStruct>
        {
            new StructureConditionTypeStruct(StructureConditionType.ExactId, "Exact Id"),
            new StructureConditionTypeStruct(StructureConditionType.RegexId, "Regex by Id"),
            new StructureConditionTypeStruct(StructureConditionType.DicomType, "Dicom type")
        };

        private string searchText;
        private StructureConditionTypeStruct searchType = StrucutreConditionTypes[0];

        protected override bool Check(ScriptArgs args)
        {
            return args?.StructureSet?.Structures.FirstOrDefault(s => CheckStructure(s)) != null;
        }

        private bool CheckStructure(Structure structure)
        {
            bool result = false;

            if (structure != null)
            {
                switch (SearchType.ConditionType)
                {
                    case StructureConditionType.ExactId:
                        result = structure?.Id == SearchText;
                        break;
                    case StructureConditionType.RegexId:
                        result = Regex.IsMatch(structure.Id, SearchText);
                        break;
                    case StructureConditionType.DicomType:
                        result = structure?.DicomType == SearchText;
                        break;
                }
            }

            return result;
        }

        public string SearchText { get => searchText; set => SetProperty(ref searchText, value); }
        public StructureConditionTypeStruct SearchType { get => searchType; set => SetProperty(ref searchType, value); }
    }

    public readonly struct StructureConditionTypeStruct
    {
        public readonly StructureConditionType ConditionType;
        public readonly string StringSearchType;

        public StructureConditionTypeStruct(StructureConditionType conditionType, string stringSearchType)
        {
            ConditionType = conditionType;
            StringSearchType = stringSearchType;
        }
        public override string ToString()
        {
            return StringSearchType;
        }
    }
}
