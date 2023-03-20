using LazyOptimizer.App;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LazyOptimizer.UI.ViewModels
{
    public class MainVM : ViewModel
    {
        private string patientId;
        private bool matchTechnique = true;
        private bool matchMachine = true;
        private string btnSettingsContent = "Settings";
        private Page currentPage;

        public Settings Settings { get; set; }
        public Page CurrentPage
        {
            get => currentPage;
            set => SetProperty(ref currentPage, value);
        }
        public string PatientId
        {
            get => patientId;
            set => SetProperty(ref patientId, value);
        }
        public bool MatchTechnique
        {
            get => matchTechnique;
            set
            {
                SetProperty(ref matchTechnique, value);
                FiltersChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool MatchMachine
        {
            get => matchMachine;
            set
            {
                SetProperty(ref matchMachine, value);
                FiltersChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public string BtnSettingsContent
        {
            get => btnSettingsContent;
            set => SetProperty(ref btnSettingsContent, value);
        }
        public MetaCommand RefreshHabits => new MetaCommand(
                o =>
                {
                    RefreshHabitsClick?.Invoke(this, EventArgs.Empty);
                }
            );
        public MetaCommand TogglePages => new MetaCommand(
                o =>
                {
                    TogglePagesClick?.Invoke(this, EventArgs.Empty);
                }
            );
        public event EventHandler FiltersChanged;
        public event EventHandler RefreshHabitsClick;
        public event EventHandler TogglePagesClick;
    }
}
