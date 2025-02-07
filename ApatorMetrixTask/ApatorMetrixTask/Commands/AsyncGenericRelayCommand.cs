﻿using System.Windows.Input;

namespace ApatorMetrixTask.Commands
{
    public class AsyncGenericRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncGenericRelayCommand(Func<T, Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public async void Execute(object parameter)
        {
            await _execute((T)parameter);
        }
    }

}
