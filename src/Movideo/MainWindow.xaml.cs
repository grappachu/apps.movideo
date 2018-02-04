using System;
using System.Threading.Tasks;
using System.Windows;
using Grappachu.Apps.Movideo.Properties;
using Grappachu.Apps.Movideo.UI.Dialogs;
using Grappachu.Core.Preview.Runtime.Threading;
using Grappachu.Core.Preview.UI;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.MediaScanner;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Core.Models;
using Grappachu.Movideo.Data.LocalDb;
using log4net.Core;

namespace Grappachu.Apps.Movideo
{
    /// <summary>
    ///     Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileAnalyzer _analyzer;
        private readonly IConfigReader _configReader;
        private readonly MovideoApp _movideo;
        private readonly BasicFileScanner _scanner;

        public MainWindow()
        {
            InitializeComponent();
            LogViewer.DisplayLevel = Level.Debug;

            IMovieDb db = new MovieDb();
            _configReader = new ConfigReader();
            _scanner = new BasicFileScanner();
            _analyzer = new FileAnalyzer(db);

            _movideo = new MovideoApp(_configReader, _scanner, _analyzer, db);
            _movideo.MatchFound += _movideo_MatchFound;
            _movideo.ProgressChanged += _movideo_ProgressChanged;
        }

        private void _movideo_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                PrgBar.Value = e.ProgressPercentage;
            }));
        }

        private void _movideo_MatchFound(object sender, MatchFoundEventArgs args)
        {
            Task.Yield();
            RuntimeHelper.RunAsStaThread(() => { MatchDialog.Prompt(args); });
        }


        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            _scanner.Path = TxtFile.SelectedValue;
            var settings = new MovideoSettings
            {
                Reorganize = ChkRename.IsChecked.GetValueOrDefault(),
                TargetPath = TxtTarget.SelectedValue,
                RenameTemplate = TxtRenameTemplate.Text,
                DeleteEmptyFolders = Settings.Default.RemoveEmtyFolders
            };

            try
            {
                Settings.Default.LastSourceFolder = TxtFile.SelectedValue;
                Settings.Default.LastOutputFolder = TxtTarget.SelectedValue;
                Settings.Default.LastRenameTemplate = TxtRenameTemplate.Text;
                Settings.Default.Save();

                PrgBar.Value = 0;
                PrgBar.Visibility = Visibility.Visible;
                _movideo.ScanAsync(settings)
                      .ContinueWith(t => { ScanCompleted(settings); });
            }
            catch (Exception ex)
            {
                Dialogs.ShowError(ex.ToString());
            }
        }

        private void ScanCompleted(MovideoSettings settings)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                PrgBar.Value = 0;
                PrgBar.Visibility = Visibility.Hidden;
            }));
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ToggleMoveAndRename(true);
        }

        private void ToggleMoveAndRename(bool enabled)
        {
            TxtTarget.IsEnabled = enabled;
            TxtRenameTemplate.IsEnabled = enabled;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ToggleMoveAndRename(false);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var jobSettings = _configReader.GetJobSettings();
            TxtFile.SelectedValue = jobSettings.SourceFolder;
            TxtTarget.SelectedValue = jobSettings.OutputFolder;
            TxtRenameTemplate.Text = jobSettings.RenameTemplate;
        }

        private void BtnScanSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new ScanSettingsDialog();
            dlg.ShowDialog();
        }
    }
}