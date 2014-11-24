namespace SofiaPublicTransport.Commands
{
    using System;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        private Action execute;

        public DelegateCommand(Action execute,
                   Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute();
        }

        public event EventHandler CanExecuteChanged;
        private Func<bool> canExecute;


        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}
