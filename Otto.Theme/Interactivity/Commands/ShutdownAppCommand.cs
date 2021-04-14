using System;
using System.Windows;
using System.Windows.Input;

namespace Otto.Theme.Interactivity
{
    public class ShutdownAppCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => Application.Current.Shutdown();

        public event EventHandler CanExecuteChanged;
    }
}
