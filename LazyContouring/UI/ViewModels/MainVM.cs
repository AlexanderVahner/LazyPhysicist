﻿using LazyContouring.Models;
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
        private readonly PatientModel patientModel;
        private readonly ScriptArgs scriptArgs;
        private readonly OperationsVM operationsVM;
        private readonly TemplateManagerVM templateManagerVM;
        private StructureSetVM currentStructureSetModel;
        private OperationPage operationPage;
        private ObservableCollection<StructureVariableVM> structures;
        private SliceControl sliceControl;
        private ViewPlaneVM viewPlaneVM;
        private StructureVariableVM selectedStructure;
        private OperationTemplate selectedTemplate;

        public MainVM(PatientModel patientModel, UserSettings userSettings, ScriptArgs args)
        {
            this.patientModel = patientModel;
            UserSettings = userSettings;
            scriptArgs = args;
            operationsVM = new OperationsVM();
            templateManagerVM = new TemplateManagerVM(userSettings.TemplateManager);
            OperationPage = new OperationPage() { ViewModel = operationsVM };
            sliceControl = new SliceControl();

            StructureSets = new SlaveCollection<StructureSetModel, StructureSetVM>(patientModel.StructureSets, m => new StructureSetVM(m), s => s.StructureSetModel);
            CurrentStructureSet = StructureSets.FirstOrDefault(ss => ss.Id == args.StructureSet.Id);           
        }

        private void SetCurrentStructureSet(StructureSetVM ss)
        {
            currentStructureSetModel = ss;
            if (ss == null)
            {
                Structures.Clear();
                operationsVM.Operations.Clear();
                sliceControl.ViewModel = null;
            }
            else
            {
                scriptArgs.StructureSet = ss.StructureSetModel.StructureSet;
                Structures = ss.Structures;
                operationsVM.Operations = ss.Operations;
                viewPlaneVM = new ViewPlaneVM() { StructureSet = ss.StructureSetModel };
                sliceControl.ViewModel = viewPlaneVM;

                viewPlaneVM.CurrentPlaneIndex = viewPlaneVM.PlaneIndexOf(viewPlaneVM.ImageModel.UserOrigin.z);
                CheckAutomaticTemplates(UserSettings.TemplateManager, scriptArgs);
            }

            NotifyPropertyChanged(nameof(CurrentStructureSet));
        }

        public void AddStructure()
        {
            var newStructureVar = new StructureVariable() { IsNew = true, };
            var newStructureVarVM = new StructureVariableVM(newStructureVar);
            var structureEditWindow = new StructureVariableEditWindow() { DataContext = newStructureVarVM };

            if (structureEditWindow.ShowDialog() ?? false)
            {
                CurrentStructureSet?.StructureSetModel.AddStructure(newStructureVar);
            }
        }

        public void CheckAutomaticTemplates(TemplateManager templateManager, ScriptArgs args)
        {
            var matchedWindowVM = new MatchedAutoTemplatesVM();
            matchedWindowVM.LoadMatched(templateManager.AutomaticTemplates, args);
            if (!matchedWindowVM.Items.Any())
            {
                return;
            }

            var matchedWindow = new MatchedAutoTemplatesWindow() { DataContext = matchedWindowVM };
            if (matchedWindow.ShowDialog() ?? false)
            {
                foreach (var item in matchedWindowVM.Items.Where(i => i.IsChecked))
                {
                    LoadNodesFromTemplate(item.Template);
                }
            }
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

            CurrentStructureSet.AddOperationString(node);
            //operationsVM.AddOperationString(node);
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
            CurrentStructureSet = new StructureSetVM(newSs);
        }

        public void SaveNodesAsTemplate()
        {
            if (operationsVM.Operations.Count == 0)
            {
                return;
            }

            var template = UserSettings.TemplateManager.CreateTemplate(operationsVM.GetCurrentNodes());
            UserSettings.TemplateManager.SaveTemplate(template);

            OpenTemplateSetupWindow(template);
        }

        public void LoadNodesFromTemplate(OperationTemplate template)
        {
            if (CurrentStructureSet == null || template == null) { return; }

            CurrentStructureSet.LoadOperationStringsFromTempalte(template);
        }

        public void RemoveTemplate(OperationTemplate template)
        {
            if (SelectedTemplate == null) 
            { 
                return; 
            }

            if (SelectedTemplate.IsAutomatic)
            {
                templateManagerVM.TemplateManager.AutomaticTemplates.Remove(template);
            }
            else
            {
                templateManagerVM.TemplateManager.ManualTemplates.Remove(template);
            }
        }

        public void ConvertSelectedToHighRes()
        {
            ConvertStructureToHighRes(selectedStructure);
        }

        public void ConvertAllToHighResSmallerThan(double volumeThreshold)
        {
            foreach (var structure in Structures)
            {
                if ((structure.StructureVariable?.VolumeCC ?? 0) > 0 
                    && (structure.StructureVariable?.VolumeCC ?? 0) < volumeThreshold)
                {
                    ConvertStructureToHighRes(structure);
                }
            }
        }

        public void ConvertStructureToHighRes(StructureVariableVM structureVariableVM)
        {
            var structure = structureVariableVM?.StructureVariable?.Structure;
            if (structure == null || !structure.CanConvertToHighResolution()) 
            {
                return;
            }
            structure.ConvertToHighResolution();
        }

        public void OpenTemplateSetupWindow(OperationTemplate template)
        {
            var templateSetupWindow = new TemplateSetupWindow { ViewModel = new OperationTemplateVM(template) };
            templateSetupWindow.Closing += (s, e) => UserSettings.TemplateManager.SaveTemplate(template);
            templateSetupWindow.ShowDialog();
        }

        public MetaCommand DuplicateStructureSetCommand => new MetaCommand(
            o => DuplicateStructureSet(CurrentStructureSet.StructureSetModel),
            o => CurrentStructureSet != null && patientModel.CanModifyData
        );

        public MetaCommand SaveNodesAsTemplateCommand => new MetaCommand(
            o => SaveNodesAsTemplate()
        );

        public MetaCommand LoadNodesFromTemplateCommand => new MetaCommand(
            o => LoadNodesFromTemplate(SelectedTemplate),
            o => SelectedTemplate != null
        );

        public MetaCommand RemoveTemplateCommand => new MetaCommand(
            o => RemoveTemplate(SelectedTemplate),
            o => SelectedTemplate != null
        );

        public MetaCommand ConvertStructureToHighResCommnd => new MetaCommand(
            o => ConvertStructureToHighRes(SelectedStructure),
            o => SelectedStructure != null && patientModel.CanModifyData
        );

        public MetaCommand ConvertAllToHighResSmallerThanCommnd => new MetaCommand(
            o => ConvertAllToHighResSmallerThan(UserSettings.StructureVolumeHighResThreshold),
            o => patientModel.CanModifyData
        );

        public MetaCommand AddStructureCommand => new MetaCommand(
            o => AddStructure(),
            o => CurrentStructureSet != null
        );

        public SlaveCollection<StructureSetModel, StructureSetVM> StructureSets { get; }
        public StructureSetVM CurrentStructureSet { get => currentStructureSetModel; set => SetCurrentStructureSet(value); }
        public ObservableCollection<StructureVariableVM> Structures { get => structures; set => SetProperty(ref structures, value); }
        public StructureVariableVM SelectedStructure { get => selectedStructure; set => SetSelectedStructure(value); }
        public OperationPage OperationPage { get => operationPage; set => SetProperty(ref operationPage, value); }
        public TemplateManagerVM TemplateManagerVM => templateManagerVM;
        public OperationTemplate SelectedTemplate { get => selectedTemplate; set => SetProperty(ref selectedTemplate, value); }
        public SliceControl SliceControl { get => sliceControl; set => SetProperty(ref sliceControl, value); }
        public UserSettings UserSettings { get; }
    }
}

