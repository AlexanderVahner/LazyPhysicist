using ESAPIInfo.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.Model
{
    public sealed class StructureSuggestionModel : IStructureSuggestionModel
    {
        public StructureSuggestionModel(IStructureInfo structureInfo)
        {
            StructureInfo = structureInfo;
        }
        public IStructureInfo StructureInfo { get; }
        public string Id => StructureInfo?.Id ?? "<none>";
        public override string ToString()
        {
            return Id;
        }
    }
}
