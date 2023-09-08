using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class PatientModel : Notifier
    {
        private readonly Patient patient;

        public PatientModel(Patient patient)
        {
            this.patient = patient;

            InitStructureSets();
        }

        private void InitStructureSets()
        {
            StructureSets.Clear();
            foreach (var ss in patient.StructureSets.OrderBy(ss => ss.Id))
            {
                StructureSets.Add(new StructureSetModel(ss));
            }
        }

        public StructureSetModel DuplicateStructureSet(StructureSetModel ss)
        {
            if (ss == null)
            {
                return null;
            }

            patient.BeginModifications();
            var newSsModel = ss.DuplicateStructureSet();
            StructureSets.Add(newSsModel);

            return newSsModel;
        }

        public ObservableCollection<StructureSetModel> StructureSets { get; } = new ObservableCollection<StructureSetModel>();
    }
}
