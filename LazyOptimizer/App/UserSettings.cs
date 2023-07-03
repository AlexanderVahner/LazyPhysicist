using Common;
using ESAPIInfo.Plan;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace LazyOptimizer.App
{
    public sealed class UserSettings : Notifier
    {
        private static string currentUserId;
        private static string userPath;
        private static string settingsFileName;
        private static string settingsFullFileName;
        private static string sqliteDBName;
        

        public static UserSettings ReadUserSettings(string settingsPath, string userId)
        {
            currentUserId = FileSystem.ClearFileNameFromInvalidChars(userId);
            userPath = Environment.ExpandEnvironmentVariables(settingsPath);
            settingsFileName = $"settings_{currentUserId}.xml";
            sqliteDBName = $"data_{currentUserId}.db";

            settingsFullFileName = Path.Combine(userPath, settingsFileName);

            UserSettings settings = null;

            if (File.Exists(settingsFullFileName))
            {
                Xml.ReadXmlToObject(settingsFullFileName, ref settings);
            }
            else
            {
                settings = new UserSettings();
            }
            InitApprovalStatuses(settings);

            return settings;
        }

        private bool debugMode = false;

        private bool plansCacheVerboseMode = true;
        private bool plansCacheRecheckAllPatients = false;
        private ushort yearsLimit = 0;

        private int plansSelectLimit = 30;
        private bool matchMachine = true;
        private bool matchTechnique = true;
        private bool showStarredOnly = false;
        private bool showCheckedApprovalStatusOnly = false;
        private bool loadNto = true;
        private string defaultPrioritySetValue = "30";
        private string lastPriorityValue = "-1";
        private bool planMergeEnabled;
        private List<CheckedApprovaStatus> approvalStatuses;

        public void Save()
        {
            Xml.WriteXmlFromObject(settingsFullFileName, this);
        }

        public string SqliteDbPath => Path.Combine(userPath, sqliteDBName);

        public bool DebugMode { get => debugMode; set => SetProperty(ref debugMode, value); }

        public bool PlansCacheVerboseMode { get => plansCacheVerboseMode; set => SetProperty(ref plansCacheVerboseMode, value); }
        public bool PlansCacheRecheckAllPatients { get => plansCacheRecheckAllPatients; set => SetProperty(ref plansCacheRecheckAllPatients, value); }
        public ushort YearsLimit { get => yearsLimit; set => SetProperty(ref yearsLimit, value); }

        public int PlansSelectLimit { get => plansSelectLimit; set => SetProperty(ref plansSelectLimit, value); }
        public bool MatchMachine { get => matchMachine; set => SetProperty(ref matchMachine, value); }
        public bool MatchTechnique { get => matchTechnique; set => SetProperty(ref matchTechnique, value); }
        public bool ShowStarredOnly { get => showStarredOnly; set => SetProperty(ref showStarredOnly, value); }
        public bool ShowCheckedApprovalStatusOnly { get => showCheckedApprovalStatusOnly; set => SetProperty(ref showCheckedApprovalStatusOnly, value); }
        public bool LoadNto { get => loadNto; set => SetProperty(ref loadNto, value); }
        public string DefaultPrioritySetValue { get => defaultPrioritySetValue; set => SetProperty(ref defaultPrioritySetValue, value); }
        public string LastPriorityValue { get => lastPriorityValue; set => SetProperty(ref lastPriorityValue, value); }
        public bool PlanMergeEnabled { get => planMergeEnabled; set => SetProperty(ref planMergeEnabled, value); }
        public List<CheckedApprovaStatus> ApprovalStatuses { get => approvalStatuses; set => approvalStatuses = value; }

        public List<int> GetCheckedApprovalStatusesInInt()
        {
            List<int> list = new List<int>();
            ApprovalStatuses.ForEach(i =>
            {
                if (i.IsChecked)
                {
                    list.Add((int)i.Status);
                }
            });

            return list;
        }

        public static void InitApprovalStatuses(UserSettings settings)
        {
            if (settings.approvalStatuses == null)
            {
                settings.approvalStatuses = new List<CheckedApprovaStatus>();
            }

            if (settings.approvalStatuses.Count == 0)
            {
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.Unknown, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.TreatmentApproved, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.PlanningApproved, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.Completed, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.CompletedEarly, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.ExternallyApproved, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.Reviewed, true));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.UnApproved, false));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.Retired, false));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.Rejected, false));
                settings.approvalStatuses.Add(new CheckedApprovaStatus(PlanSetupApprovalStatus.UnPlannedTreatment, false));
            }

            settings.approvalStatuses.ForEach(i => i.PropertyChanged += (s, e) => 
            {
                if (settings.ShowCheckedApprovalStatusOnly)
                {
                    settings.NotifyPropertyChanged(nameof(settings.ShowCheckedApprovalStatusOnly));
                }
            });
        }
    }

    public sealed class CheckedApprovaStatus : Notifier
    {
        private bool isChecked = false;
        private PlanSetupApprovalStatus status = PlanSetupApprovalStatus.Unknown;

        public CheckedApprovaStatus() { }
        public CheckedApprovaStatus(PlanSetupApprovalStatus status, bool isChecked)
        {
            Status = status;
            IsChecked = isChecked;
        }
        public PlanSetupApprovalStatus Status { get => status; set => status = value; }
        public string StatusName => Status.ToString();
        public bool IsChecked { get => isChecked; set => SetProperty(ref isChecked, value); }

    }

}
