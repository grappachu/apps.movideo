using System.Globalization;
using System.Windows;
using Grappachu.Movideo.Core;

namespace Grappachu.Apps.Movideo.Views.Dialogs
{
    /// <summary>
    ///     Logica di interazione per ApiKeyDialog.xaml
    /// </summary>
    public partial class ApiKeyDialog
    {
        public ApiKeyDialog()
        {
            InitializeComponent();
        }

        public static bool Prompt(ApiSettings settings)
        {
            var dlg = new ApiKeyDialog
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,

                TxtApiKey = {Text = settings.ApiKey},
                TxtCulture = {Text = settings.ApiCulture.ToString()}
            };

            var res = dlg.ShowDialog();

            if (res == true)
            {
                settings.ApiKey = dlg.TxtApiKey.Text;
                settings.ApiCulture = new CultureInfo(dlg.TxtCulture.Text);
                return true;
            }

            return false;
        }

        private void BtnNavigateToTmdb(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://developers.themoviedb.org/3/getting-started");
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid())
            {
                Core.Preview.UI.Dialogs.ShowError("I dati inseriti non sono validi");
                return;
            }
            
            DialogResult = true;
            Close();
        }

        private bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(TxtApiKey.Text);
        }
    }
} 