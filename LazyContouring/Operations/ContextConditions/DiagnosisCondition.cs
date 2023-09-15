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
        private string codeShouldStartsWith = "";

        protected override bool Check(ScriptArgs args)
        {
            string code = CodeShouldStartsWith.Trim().ToUpper();
            return code == "" ||
                args.Course?.Diagnoses?.FirstOrDefault(d => d.Code.ToUpper().StartsWith(code)) != null;
        }

        public string CodeShouldStartsWith { get => codeShouldStartsWith; set => SetProperty(ref codeShouldStartsWith, value); }
    }
}
