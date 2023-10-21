using LazyContouring.Models;
using LazyContouring.Operations;
using LazyContouring.UI.ViewModels.Operations;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
using ScriptArgsNameSpace;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LazyContouring.UI.ViewModels
{
    public sealed class MainVM : Notifier
    {
        private StructureSetModel currentStructureSetModel;
        private readonly PatientModel patientModel;
        private readonly OperationsVM operationsVM;
        private readonly TemplateManagerVM templateManagerVM;
        private OperationPage operationPage;
        private SlaveCollection<StructureVariable, StructureVariableVM> structures;
        private SliceControl sliceControl;
        private ViewPlaneVM viewPlaneVM;
        private StructureVariableVM selectedStructure;

        public MainVM(PatientModel patientModel, UserSettings userSettings, ScriptArgs args)
        {
            this.patientModel = patientModel;
            UserSettings = userSettings;
            operationsVM = new OperationsVM();
            templateManagerVM = new TemplateManagerVM(userSettings.TemplateManager);
            OperationPage = new OperationPage() { ViewModel = operationsVM };
            sliceControl = new SliceControl();

            CurrentStructureSet = patientModel.StructureSets.FirstOrDefault(ss => ss.Id == args.StructureSet.Id);
        }

        private void SetCurrentStructureSet(StructureSetModel ss)
        {
            currentStructureSetModel = ss;
            if (ss == null)
            {
                Structures.BreakFree();
                sliceControl.ViewModel = null;
            }
            else
            {
                Structures.ObeyTheMaster(ss.Structures, m => new StructureVariableVM(m), s => s.StructureVariable);
                viewPlaneVM = new ViewPlaneVM() { StructureSet = currentStructureSetModel }; 
                sliceControl.ViewModel = viewPlaneVM;

                viewPlaneVM.CurrentPlaneIndex = viewPlaneVM.PlaneIndexOf(viewPlaneVM.ImageModel.UserOrigin.z);
            }

            NotifyPropertyChanged(nameof(CurrentStructureSet));
        }

        public void AddNodeStringFromDrop(IDataObject drop)
        {
            object data = drop.GetData(typeof(StructureVariable));
            if (data == null)
            {
                return;
            }

            var node = new OperationNode()
            {
                IsRootNode = true,
                StructureVar = data as StructureVariable,
                Operation = new AssignOperation()
            };

            operationsVM.AddOperationString(node);
        }

        public void SetSelectedStructure(StructureVariableVM value)
        {
            if (selectedStructure?.StructureVariable != null) { selectedStructure.StructureVariable.IsSelected = false; }
            selectedStructure = value;
            if (selectedStructure?.StructureVariable != null) 
            { 
                selectedStructure.StructureVariable.IsSelected = true;
                viewPlaneVM.CurrentPlaneIndex = viewPlaneVM.PlaneIndexOf(selectedStructure.StructureVariable.Structure.CenterPoint.z);
            }

            NotifyPropertyChanged(nameof(SelectedStructure));
        }

        public void DuplicateStructureSet(StructureSetModel ss)
        {
            if (ss == null) { return; }

            var newSs = ss.DuplicateStructureSet();
            CurrentStructureSet = newSs;
        }

        public void SaveNodesAsTemplate()
        {
            if (operationsVM.Operations.Count == 0)
            {
                return;
            }

            var template = UserSettings.TemplateManager.CreateTemplate(operationsVM.GetCurrentNodes());
            UserSettings.TemplateManager.SaveTemplate(template);

            var templateSetupWindow = new TemplateSetupWindow { ViewModel = new OperationTemplateVM(template) };
            templateSetupWindow.ShowDialog();
        }

        public MetaCommand DuplicateStructureSetCommand => new MetaCommand(
            o => DuplicateStructureSet(CurrentStructureSet),
            o => CurrentStructureSet != null && patientModel.CanModifyData
        );

        public MetaCommand SaveNodesAsTemplateCommand => new MetaCommand(
            o => SaveNodesAsTemplate()
        );

        public ObservableCollection<StructureSetModel> StructureSets => patientModel.StructureSets;
        public StructureSetModel CurrentStructureSet { get => currentStructureSetModel; set => SetCurrentStructureSet(value); }
        public SlaveCollection<StructureVariable, StructureVariableVM> Structures => structures ?? (structures = new SlaveCollection<StructureVariable, StructureVariableVM>());
        public StructureVariableVM SelectedStructure { get => selectedStructure; set => SetSelectedStructure(value); }
        public OperationPage OperationPage { get => operationPage; set => SetProperty(ref operationPage, value); }
        public TemplateManagerVM TemplateManagerVM => templateManagerVM;
        public SliceControl SliceControl { get => sliceControl; set => SetProperty(ref sliceControl, value); }
        public UserSettings UserSettings { get; }
    }
}

