using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDEvernote.WPF
{
    public class Command : ICommand
    {
        private Action execute;
        private Action<object> executeArg;
        private Func<object, bool> canExecuteArg;
        private Func<bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object> executeArg, Func<object, bool> canExecute = null)
        {
            this.executeArg = executeArg;
            this.canExecuteArg = canExecute;
        }
        public Command(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter = null)
        {
            if (parameter != null)
            {
                return this.canExecuteArg == null || this.canExecuteArg(parameter);
            }
            else
            {
                return this.canExecute == null || this.canExecute();
            }
        }

        public void Execute(object parameter = null)
        {
            if (execute != null)
            {
                this.execute();
            }
            else if(executeArg != null)
            {
                this.executeArg(parameter);
            }
        }
    }
}
