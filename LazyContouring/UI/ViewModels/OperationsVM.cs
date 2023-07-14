using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationsVM
    {
        public OperationsVM() { }

        public RichTextBox OpsTextBox { get; set; } = new RichTextBox();
    }
}
