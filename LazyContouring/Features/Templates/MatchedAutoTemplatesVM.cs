using LazyContouring.Operations;
using LazyPhysicist.Common;
using ScriptArgsNameSpace;
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
        public void LoadMatched(IEnumerable<OperationTemplate> automaticTemplates, ScriptArgs args)
        {
            foreach (var template in automaticTemplates)
            {
                Items.Clear();
                bool matched = true;
                foreach (var conditionNode in template.ConditionNodes) 
                {
                    matched = conditionNode.CheckNode(args);
                    if (!matched)
                    {
                        break;
                    }
                }

                if (matched)
                {
                    Items.Add(new MatchedAutoTemplateItemVM(template));
                }
            }
        }

        public MetaCommand SelectAllCommand => new MetaCommand(
            o => { foreach (var item in Items) item.IsChecked = true; }
        );

        public MetaCommand SelectNoneCommand => new MetaCommand(
            o => { foreach (var item in Items) item.IsChecked = false; }
        );

        public ObservableCollection<MatchedAutoTemplateItemVM> Items { get; } = new ObservableCollection<MatchedAutoTemplateItemVM>();
    }

    public sealed class MatchedAutoTemplateItemVM : Notifier
    {
        public MatchedAutoTemplateItemVM(OperationTemplate template)
        {
            Template = template;
        }

        private bool isChecked = true;

        public OperationTemplate Template { get; }
        public string Name => Template.Name;
        public bool IsChecked { get => isChecked; set => SetProperty(ref isChecked, value); }
    }


}
