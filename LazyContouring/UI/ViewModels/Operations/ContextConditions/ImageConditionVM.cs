using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class ImageConditionVM : Notifier
    {
        public ImageConditionVM(ImageCondition inageCondition)
        {
            ImageCondition = inageCondition;
            inageCondition.PropertyChanged += (s, e) => NotifyPropertyChanged(nameof(Title));
        }

        public string Title => "Image slice thickness must " + (ImageCondition.ShouldBe ? "" : "NOT") + " be:";

        public ImageCondition ImageCondition { get; }
    }
}
