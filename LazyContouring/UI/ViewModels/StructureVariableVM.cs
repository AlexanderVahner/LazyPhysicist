using LazyContouring.Models;
using LazyPhysicist.Common;
using System.Windows;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class StructureVariableVM : Notifier
    {
        private StructureVariable structureVar;
        private Brush strokeBrush;
        private Brush fillBrush;
        private string structureId;
        private readonly Brush defaultStrokeBrush = new SolidColorBrush(Colors.DarkGray);

        public StructureVariableVM(StructureVariable structureVar)
        {
            StructureVariable = structureVar;
        }

        private void SetStructureVariable(StructureVariable strVar)
        {
            if (structureVar != null)
            {
                structureVar.PropertyChanged -= StructureVar_PropertyChanged;
            }

            structureVar = strVar;

            if (structureVar != null)
            {
                structureVar.PropertyChanged += StructureVar_PropertyChanged;
            }

            StructureId = StructureVariable?.StructureId ?? "Empty";
            StrokeBrush = structureVar != null ? new SolidColorBrush(structureVar.Color) : defaultStrokeBrush;
            FillBrush = (structureVar?.IsEmpty ?? true) ? null : new SolidColorBrush(structureVar.Color);
            NotifyPropertyChanged(nameof(CanEditVisibility));
        }

        private void StructureVar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(StructureVariable.StructureId)):
                    StructureId = StructureVariable?.StructureId ?? "Empty";
                    break;
                case (nameof(StructureVariable.Color)):
                    StrokeBrush = new SolidColorBrush(this.structureVar.Color);
                    break;
                case (nameof(StructureVariable.IsEmpty)):
                    FillBrush = StructureVariable.IsEmpty ? null : new SolidColorBrush(this.structureVar.Color);
                    break;
                case (nameof(StructureVariable.CanEditSegmentVolume)):
                    NotifyPropertyChanged(nameof(CanEditVisibility));
                    break;
            }
        }

        public StructureVariable StructureVariable { get => structureVar; set => SetStructureVariable(value); }
        public string StructureId
        {
            get => structureId;
            set => SetProperty(ref structureId, value);
        }

        public Visibility CanEditVisibility => (StructureVariable?.CanEditSegmentVolume ?? true) ? Visibility.Hidden : Visibility.Visible;
        public Brush StrokeBrush { get => strokeBrush; set => SetProperty(ref strokeBrush, value); }
        public Brush FillBrush { get => fillBrush; set => SetProperty(ref fillBrush, value); }
    }
}
