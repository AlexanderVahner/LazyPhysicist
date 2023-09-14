using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations.ContextConditions
{
    public sealed class DiagnosisCondition : ContextCondition
    {
        protected override bool CheckCondition(ScriptArgs args)
        {
            return CodeShouldStartsWith == "" || 
                args.Course?.Diagnoses?.FirstOrDefault(d => d.Code.ToUpper().StartsWith(CodeShouldStartsWith.Trim().ToUpper())) != null;
        }

        public string CodeShouldStartsWith { get; set; } = "";
    }
}
