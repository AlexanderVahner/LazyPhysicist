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
        protected override bool CheckCondition(ScriptArgs args)
        {
            return args?.StructureSet?.Structures.FirstOrDefault(s => CheckStructure(s)) != null;
        }

        private bool CheckStructure(Structure structure)
        {
            bool result = false;

            if (structure != null)
            {
                switch (SearchType)
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

        public string SearchText { get; set; }
        public StructureConditionType SearchType { get; set; } = StructureConditionType.ExactId;

    }
}
