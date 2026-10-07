using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Otto.Theme.Interactivity;

public class OpenLinkCommand : ICommand
{
    public bool CanExecute(object parameter) => TryGetUri(parameter, out _);

    public void Execute(object parameter)
    {
        if (TryGetUri(parameter, out var uri))
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
    }

    private static bool TryGetUri(object parameter, out Uri uri)
    {
        uri = null;
        return parameter is string link && Uri.TryCreate(link, UriKind.Absolute, out uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
