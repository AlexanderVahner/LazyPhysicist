using LazyContouring.Models;
using LazyContouring.Operations;
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
        private PatientModel patientModel;
        private OperationsVM operationsVM;
        private OperationPage operationPage;
        private ViewPlaneVM viewPlaneVM;
        private SlaveCollection<StructureVariable, StructureVariableVM> structures;
        private SliceControl sliceControl;
        private StructureVariableVM selectedStructure;

        public MainVM(PatientModel patientModel, ScriptArgs args)
        {
            this.patientModel = patientModel;

            operationsVM = new OperationsVM();
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
                sliceControl.ViewModel = new ViewPlaneVM() { StructureSet = currentStructureSetModel
                    , CurrentPlaneIndex = 100 }; ///////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
            if (selectedStructure?.StructureVariable != null) { selectedStructure.StructureVariable.IsSelected = true; }

            NotifyPropertyChanged(nameof(SelectedStructure));
        }

        public ObservableCollection<StructureSetModel> StructureSets => patientModel.StructureSets;
        public StructureSetModel CurrentStructureSet { get => currentStructureSetModel; set => SetCurrentStructureSet(value); }
        public SlaveCollection<StructureVariable, StructureVariableVM> Structures => structures ?? (structures = new SlaveCollection<StructureVariable, StructureVariableVM>());
        public StructureVariableVM SelectedStructure { get => selectedStructure; set => SetSelectedStructure(value); }
        public OperationPage OperationPage { get => operationPage; set => SetProperty(ref operationPage, value); }
        public SliceControl SliceControl { get => sliceControl; set => SetProperty(ref sliceControl, value); }
    }
}

