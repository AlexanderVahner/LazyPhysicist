using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations.ContextConditions
{
    public sealed class ImageSliceThicknessCondition : ContextCondition
    {
        protected override bool CheckCondition(ScriptArgs args)
        {
            return (args.StructureSet?.Image?.ZRes ?? -1.0) == SliceThickness;
        }

        public double SliceThickness { get; set; } = 1.0;
    }
}
