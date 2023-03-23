using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Patients
{
    public class PatientInfo
    {
        public PatientInfo(Patient patient)
        {
            Patient = patient;
        }
        public void BeginModifications()
        {
            Patient?.BeginModifications();
        }
        public Patient Patient { get; private set; }
    }
}
