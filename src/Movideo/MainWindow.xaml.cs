using System;
using System.Threading.Tasks;
using System.Windows;
using Grappachu.Core.Preview.Runtime.Threading;
using Grappachu.Core.Preview.UI;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Dtos;
using Grappachu.Movideo.Core.Interfaces;
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
        private readonly TextBoxFileScanner _scanner;

        public MainWindow()
        {
            InitializeComponent();
            LogViewer.DisplayLevel = Level.Debug;

            IMovieDb db = new MovieDb();
            _configReader = new ConfigReader();
            _scanner = new TextBoxFileScanner();
            _analyzer = new FileAnalyzer(db);

            _movideo = new MovideoApp(_configReader, _scanner, _analyzer, db);
            _movideo.MatchFound += _movideo_MatchFound;
        }

        private void _movideo_MatchFound(object sender, MatchFoundEventArgs args)
        {
            Task.Yield();
            RuntimeHelper.RunAsStaThread(() =>
            {
                UI.Dialogs.MatchDialog.Prompt(args);
            });
        }


        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            _scanner.Path = TxtFile.SelectedValue;
            var settings = new MovideoSettings
            {
                Reorganize = ChkRename.IsChecked.GetValueOrDefault(),
                TargetPath = TxtTarget.SelectedValue
            };
            try
            {
                var task = _movideo.ScanAsync(settings);
                task.Wait();
            }
            catch (Exception ex)
            {
                Dialogs.ShowError(ex.ToString());
            }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            TxtTarget.IsEnabled = true;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            TxtTarget.IsEnabled = false;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
           
             
        }
    }
}