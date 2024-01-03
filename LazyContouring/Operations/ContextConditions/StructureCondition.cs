using ScriptArgsNameSpace;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations.ContextConditions
{    
    public enum StructureConditionType { ExactId, IdStartsWith, RegexId }

    public sealed class StructureCondition : ContextCondition
    {
        public const string AnyDicomType = "<any>";

        public static readonly List<string> StrucutreDicomTypes = new List<string>
        {
            AnyDicomType,
            "EXTERNAL",
            "ORGAN",
            "PTV",
            "CTV",
            "GTV",
            "BOOST",
            "SUPPORT", 
            "FIXATION", 
            "REGISTRATION",
            "AVOIDANCE",
            "CAVITY", 
            "CONTRAST_AGENT", 
            "IRRAD_VOLUME", 
            "TREATED_VOLUME", 
            "DOSE_REGION"
        };

        private string searchText = "";
        private string searchDicomType = AnyDicomType;
        private StructureConditionType searchType = StructureConditionType.ExactId;

        protected override bool Check(ScriptArgs args)
        {
            return args?.StructureSet?.Structures.FirstOrDefault(s => CheckStructure(s)) != null;
        }

        private bool CheckStructure(Structure structure)
        {
            bool result = false;            

            if (structure == null)
            {
                return false;
            }

            string desiredId = SearchText?.Trim().ToUpper() ?? "";
            string desiredDicomType = SearchDicomType?.Trim().ToUpper() ?? "";

            bool IsDicomTypeMatch = desiredDicomType == AnyDicomType
                || desiredDicomType == ""
                || structure.DicomType.ToUpper() == desiredDicomType;

            switch (SearchType)
            {
                case StructureConditionType.ExactId:
                    result = desiredId == "" || structure.Id.Trim().ToUpper() == desiredId;
                    break;
                case StructureConditionType.IdStartsWith:
                    result = desiredId == "" || structure.Id.Trim().ToUpper().StartsWith(desiredId);
                    break;
                case StructureConditionType.RegexId:
                    result = desiredId == "" || Regex.IsMatch(structure.Id, desiredId);
                    break;
            }

            return result && IsDicomTypeMatch;
        }

        public string SearchText { get => searchText; set => SetProperty(ref searchText, value); }
        public string SearchDicomType { get => searchDicomType; set => SetProperty(ref searchDicomType, value); }
        public StructureConditionType SearchType { get => searchType; set => SetProperty(ref searchType, value); }
    }
}
