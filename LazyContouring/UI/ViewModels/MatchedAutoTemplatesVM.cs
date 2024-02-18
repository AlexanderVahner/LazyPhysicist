using LazyContouring.Operations;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels
{
    public sealed class MatchedAutoTemplatesVM : Notifier
    {
        public MetaCommand SelectAllCommand => new MetaCommand(
            o => { foreach (var item in Items) item.IsChecked = true; }
        );

        public MetaCommand SelectNoneCommand => new MetaCommand(
            o => { foreach (var item in Items) item.IsChecked = false; }
        );

        public ObservableCollection<AutoTemplateItemVM> Items { get; } = new ObservableCollection<AutoTemplateItemVM>();
    }

    public sealed class AutoTemplateItemVM : Notifier
    {
        public AutoTemplateItemVM(OperationTemplate template)
        {
            Template = template;
        }

        private bool isChecked = false;

        public OperationTemplate Template { get; }
        public string Name => Template.Name;
        public bool IsChecked { get => isChecked; set => SetProperty(ref isChecked, value); }
    }


}
