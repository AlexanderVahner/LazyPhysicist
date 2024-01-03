using LazyContouring.Operations.ContextConditions;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class ImageConditionVM : ConditionNodeVM
    {
        public ImageConditionVM(ImageCondition imageCondition) : base(imageCondition)
        {
            ImageCondition = imageCondition;
            Title = "Image slice thickness should " + (ImageCondition.ShouldBe ? "" : "NOT ") + "be:";
            imageCondition.PropertyChanged += (s, e) => Title = "Image slice thickness should " + (ImageCondition.ShouldBe ? "" : "NOT ") + "be:";
        }

        public ImageCondition ImageCondition { get; }
    }
}
