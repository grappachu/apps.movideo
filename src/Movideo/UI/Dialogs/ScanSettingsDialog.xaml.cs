using System.Windows;
using Grappachu.Apps.Movideo.Properties;

namespace Grappachu.Apps.Movideo.UI.Dialogs
{
    /// <summary>
    ///     Logica di interazione per ScanSettingsDialog.xaml
    /// </summary>
    public partial class ScanSettingsDialog : Window
    {
        public ScanSettingsDialog()
        {
            InitializeComponent();
        }

        private void ScanSettingsDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            ChkDeleteEmtyFolders.IsChecked = Settings.Default.RemoveEmtyFolders;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.RemoveEmtyFolders = ChkDeleteEmtyFolders.IsChecked.GetValueOrDefault();
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}