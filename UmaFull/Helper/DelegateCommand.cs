using System;
using System.Windows.Input;

namespace UmaFull
{
    /// <summary>
    /// コマンド実装ヘルパー
    /// </summary>
    public class DelegateCommand : ICommand
    {
        Action execute;
        Func<bool> canExecute;

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            return canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public DelegateCommand(Action execute)
        {
            this.execute = execute;
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }
}
