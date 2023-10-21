using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations.ContextConditions
{
    public sealed class ImageCondition : ContextCondition
    {
        private double sliceThickness = 1;

        protected override bool Check(ScriptArgs args)
        {
            return (args.StructureSet?.Image?.ZRes ?? -1.0) == SliceThickness;
        }

        public double SliceThickness { get => sliceThickness; set => SetProperty(ref sliceThickness, value); }
    }
}
