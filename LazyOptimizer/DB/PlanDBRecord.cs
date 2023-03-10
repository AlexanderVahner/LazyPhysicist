using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    [DBTableName("Plans")]
    public class PlanDBRecord : DBRecord
    {
        public PlanDBRecord(SQLiteService dbService) : base(dbService, "Plans") { }

        public string PlanId;
        public string PatientId;
        public string CourseId;
        public long? FractionsCount;
        public double? SingleDose;
        public string MachineId;
        public long? Technique;
        public long? SelectionFrequency;
        public long? LDistance;
        public string StructuresString;
        public string Description;

        public string DescriptionProperty
        {
            get => Description;
            set
            {
                if (SetValue("Description", value) == 1)
                {
                    Description = value;
                }
            }
        }
        public long? SelectionFrequencyProperty
        {
            get => SelectionFrequency;
            set
            {
                if (SetValue("SelectionFrequency", value) == 1)
                {
                    SelectionFrequency = value;
                }
            }
        }

        private List<ObjectiveDBRecord> objectives;
        public List<ObjectiveDBRecord> Objectives => objectives ?? (objectives = new List<ObjectiveDBRecord>());
    }
}
