using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class DiagnosisConditionVM : Notifier
    {
        public DiagnosisConditionVM(DiagnosisCondition diagnosisCondition)
        {
            DiagnosisCondition = diagnosisCondition;
            diagnosisCondition.PropertyChanged += (s, e) => NotifyPropertyChanged(nameof(Title));
        }

        public string Title => "Diagnosis Code must " + (DiagnosisCondition.ShouldBe ? "" : "NOT") + " start with:";

        public DiagnosisCondition DiagnosisCondition { get; }
    }
}
