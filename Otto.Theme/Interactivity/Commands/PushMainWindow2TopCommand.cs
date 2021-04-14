﻿using Otto.Theme.Tools.Helper;

using System;
using System.Windows;
using System.Windows.Input;

namespace Otto.Theme.Interactivity
{
    public class PushMainWindow2TopCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility != Visibility.Visible)
            {
                Application.Current.MainWindow.Show();
                WindowHelper.SetWindowToForeground(Application.Current.MainWindow);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
