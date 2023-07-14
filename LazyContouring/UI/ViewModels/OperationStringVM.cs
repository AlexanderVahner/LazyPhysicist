using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationStringVM
    {
        private readonly OperationNode node;

        public OperationStringVM(OperationNode node)
        {
            this.node = node;
        }
    }
}
