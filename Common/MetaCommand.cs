using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LazyPhysicist.Common
{
    // Get from https://metanit.com/sharp/wpf/22.3.php
    public class MetaCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public delegate void AdditionalActionsDelegate();
        public AdditionalActionsDelegate AdditionalActions;

        public MetaCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter)
        {
            execute(parameter);
            AdditionalActions?.Invoke();
        }
    }
}
