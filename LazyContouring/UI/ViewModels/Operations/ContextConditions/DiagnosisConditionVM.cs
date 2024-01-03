using LazyContouring.Operations.ContextConditions;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class DiagnosisConditionVM : ConditionNodeVM
    {
        public DiagnosisConditionVM(DiagnosisCondition diagnosisCondition) : base(diagnosisCondition)
        {
            DiagnosisCondition = diagnosisCondition;
            Title = "Diagnosis Code must " + (DiagnosisCondition.ShouldBe ? "" : "NOT ") + "start with:";
            diagnosisCondition.PropertyChanged += (s, e) => Title = "Diagnosis Code must " + (DiagnosisCondition.ShouldBe ? "" : "NOT ") + "start with:";
        }

        public DiagnosisCondition DiagnosisCondition { get; }
    }
}
