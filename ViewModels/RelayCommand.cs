using System;
using System.Windows.Input;

namespace MyLights.ViewModels
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
        {
            this._execute = execute;
        }

        public RelayCommand(Action execute)
        {
            this._execute = (_) => execute();
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);
            else
                return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private Action<T> _execute;
        private Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute)
        {
            this._execute = execute;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute((T)parameter);
            else
                return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T t)
                _execute(t);

            else
                throw new ArgumentException($"Wrong parameter type. Expected {typeof(T)} and got {parameter.GetType()}");
        }
    }
}