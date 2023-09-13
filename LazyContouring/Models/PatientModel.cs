using LazyPhysicist.Common;
using System.Collections.ObjectModel;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class PatientModel : Notifier
    {
        private readonly Patient patient;

        public PatientModel(Patient patient)
        {
            this.patient = patient;

            NotifyPropertyChanged(nameof(CanModifyData));
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
        public bool CanModifyData => patient.CanModifyData();
    }
}
