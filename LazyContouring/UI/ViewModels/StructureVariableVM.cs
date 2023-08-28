using LazyContouring.Models;
using LazyContouring.Operations;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class StructureVariableVM : Notifier
    {
        private readonly StructureVariable structureVar;
        private Brush strokeBrush;
        private Brush fillBrush;

        public StructureVariableVM(StructureVariable structureVar)
        {
            this.structureVar = structureVar;
            StrokeBrush = new SolidColorBrush(structureVar.Color);
            FillBrush = StructureVariable.IsEmpty ? null : new SolidColorBrush(structureVar.Color);
            NotifyPropertyChanged(nameof(CanEditVisibility));

            structureVar.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
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
            };
        }

        public StructureVariable StructureVariable => structureVar;
        public Visibility CanEditVisibility => StructureVariable.CanEditSegmentVolume ? Visibility.Hidden : Visibility.Visible;
        public Brush StrokeBrush { get => strokeBrush; set => SetProperty(ref strokeBrush, value); }
        public Brush FillBrush { get => fillBrush; set => SetProperty(ref fillBrush, value); }
    }
}
