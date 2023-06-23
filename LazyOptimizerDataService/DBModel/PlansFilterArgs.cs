using LazyPhysicist.Common;
using System;
using System.Collections.Generic;

namespace LazyOptimizerDataService.DBModel
{
    public sealed class PlansFilterArgs : Notifier
    {
        public PlansFilterArgs()
        {
            PropertyChanged += (s, e) => Update();
        }
        private string structuresString;
        private double singleDose = .0;
        private int fractionsCount = 0;
        private string technique = "";
        private string machineId = "";
        private bool matchTechnique = false;
        private bool matchMachine = false;
        private bool starredOnly = false;
        private bool checkedApprovalStatusesOnly = false;
        private List<int> checkedApprovalStatuses;
        private int limit = 0;
        public string StructuresString { get => structuresString; set => SetProperty(ref structuresString, value); }
        public double SingleDose { get => singleDose; set => SetProperty(ref singleDose, value); }
        public int FractionsCount { get => fractionsCount; set => SetProperty(ref fractionsCount, value); }
        public string Technique { get => technique; set => SetProperty(ref technique, value); }
        public string MachineId { get => machineId; set => SetProperty(ref machineId, value); }
        public bool MatchTechnique { get => matchTechnique; set => SetProperty(ref matchTechnique, value); }
        public bool MatchMachine { get => matchMachine; set => SetProperty(ref matchMachine, value); }
        public bool StarredOnly { get => starredOnly; set => SetProperty(ref starredOnly, value); }
        public int Limit { get => limit; set => SetProperty(ref limit, value); }
        public bool CheckedApprovalStatusesOnly { get => checkedApprovalStatusesOnly; set => SetProperty(ref checkedApprovalStatusesOnly, value); }
        public List<int> CheckedApprovalStatuses { get => checkedApprovalStatuses; set => SetProperty(ref checkedApprovalStatuses, value); }

        public void Update()
        {
            UpdateRequest?.Invoke(this, this);
        }
        public event EventHandler<PlansFilterArgs> UpdateRequest;
    }
}
