using System;
using System.Windows.Input;

namespace WindowsUpdateNotifier.Desktop
{
    public class SimpleCommand : ICommand
    {
        private readonly Action mExecuteCallback;
        private readonly Func<bool> mCanExecuteCallback;

        public SimpleCommand(Action executeCallback)
        {
            mExecuteCallback = executeCallback;
        }

        public SimpleCommand(Action executeCallback, Func<bool> canExecuteCallback)
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