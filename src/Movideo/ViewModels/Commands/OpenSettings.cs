using System.Windows.Input;
using Grappachu.Apps.Movideo.UI.Dialogs;

namespace Grappachu.Apps.Movideo.ViewModels.Commands
{
    internal static class OpenSettings
    {
        private static ICommand _openSettingsCommand;


        public static ICommand GetCommand()
        {
            if (_openSettingsCommand == null)
                _openSettingsCommand = new RelayCommand(
                    param =>
                    {
                        var dlg = new ScanSettingsDialog();
                        dlg.ShowDialog();
                    },
                    param => true
                );
            return _openSettingsCommand;
        }
    }
}