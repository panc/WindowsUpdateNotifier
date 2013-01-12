using System;
using System.Windows.Input;

namespace WindowsUpdateNotifier
{
    public class SimpleCommand : ICommand
    {
        private readonly Action mExecuteCallback;
        private readonly Func<bool> mCanExecuteCallback;
        private bool mCanExecute;

        public SimpleCommand(Action executeCallback)
        {
            mExecuteCallback = executeCallback;
        }

        public SimpleCommand(Action executeCallback, Func<bool> canExecuteCallback)
        {
            mExecuteCallback = executeCallback;
            mCanExecuteCallback = canExecuteCallback;
            mCanExecute = false;
        }

        public void Execute(object parameter)
        {
            mExecuteCallback();
        }

        public bool CanExecute(object parameter)
        {
            var canExecute = mCanExecuteCallback == null || mCanExecuteCallback();

            if(canExecute != mCanExecute)
            {
                mCanExecute = canExecute;
                OnCanExecuteChanged();
            }

            return canExecute;
        }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}