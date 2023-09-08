using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class ViewPlaneModel : Notifier
    {
        private StructureSetModel structureSet;

        private void SetStructureSet(StructureSetModel value)
        {
            structureSet = value;
            NotifyPropertyChanged(nameof(structureSet));
        }

        private void SetImage()
        {
            if (structureSet?.StructureSet?.Image == null)
            {
                return;
            }
        }

        public StructureSetModel StructureSet { get => structureSet; set => SetStructureSet(value); }

        
    }
}
