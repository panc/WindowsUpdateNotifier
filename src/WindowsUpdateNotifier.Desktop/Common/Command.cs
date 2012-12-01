using System;
using System.Windows.Input;

namespace WindowsUpdateNotifier.Desktop.Common
{
    public class Command : ICommand
    {
        private readonly Action mExecuteCallback;
        private readonly Func<bool> mCanExecuteCallback;

        public Command(Action executeCallback)
        {
            mExecuteCallback = executeCallback;
        }

        public Command(Action executeCallback, Func<bool> canExecuteCallback)
        {
            mExecuteCallback = executeCallback;
            mCanExecuteCallback = canExecuteCallback;
        }

        public void Execute(object parameter)
        {
            mExecuteCallback();
        }

        public bool CanExecute(object parameter)
        {
            return mCanExecuteCallback == null || mCanExecuteCallback();
        }

        public event EventHandler CanExecuteChanged;
    }
}