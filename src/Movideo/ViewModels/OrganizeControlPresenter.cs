using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Grappachu.Apps.Movideo.Common;
using Grappachu.Apps.Movideo.Config;
using Grappachu.Apps.Movideo.Properties;
using Grappachu.Apps.Movideo.Views.Dialogs;
using Grappachu.Core.Preview.Runtime.Threading;
using Grappachu.Core.Preview.UI;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Components.MediaOrganizer;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Core.Models;
using log4net;

namespace Grappachu.Apps.Movideo.ViewModels
{
    public class OrganizeControlPresenter : ObservableObject
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OrganizeControlPresenter));
        private readonly MovideoApp _movideoApp;
        private double _progressBarValue;
        private Visibility _progressBarVisibility = Visibility.Hidden;
        private ICommand _scanCommand;


        public OrganizeControlPresenter()
        {
            _movideoApp = AppFactory.GetInstance();
            _movideoApp.ProgressChanged += (sender, args) => ProgressBarValue = args.ProgressPercentage;
            _movideoApp.MatchFound += _movideo_MatchFound;
           
            RaisePropertyChangedEvent(nameof(CurrentJob));
        }

        public double ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                RaisePropertyChangedEvent(nameof(ProgressBarValue));
            }
        }

        public string UiRenameTooltip { get
        {
            return string.Format("I seguenti token sono supportati: \n\n {0}", string.Join(", ",
                Tokens.List));
        }}

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChangedEvent(nameof(ProgressBarVisibility));
            }
        }


        public JobSettings CurrentJob => _movideoApp.CurrentJob;


        public ICommand ScanCommand
        {
            get
            {
                if (_scanCommand == null)
                    _scanCommand = new RelayCommand(
                        param => BeginScan(),
                        param => true
                    );
                return _scanCommand;
            }
        }

        public bool ReorganizeFiles { get; set; }


        private void _movideo_MatchFound(object sender, MatchFoundEventArgs args)
        {
            Task.Yield();
            RuntimeHelper.RunAsStaThread(() => { MatchDialog.Prompt(args); });
        }


        private void BeginScan()
        {
            var settings = new MovideoSettings
            {
                SourcePath = Settings.Default.LastSourceFolder,
                Reorganize = ReorganizeFiles,
                TargetPath = Settings.Default.LastOutputFolder,
                RenameTemplate = CurrentJob.RenameTemplate,
                DeleteEmptyFolders = Settings.Default.RemoveEmtyFolders
            };

            try
            {
                Settings.Default.LastRenameTemplate = settings.RenameTemplate;
                Settings.Default.Save();

                ProgressBarValue = 0;
                ProgressBarVisibility = Visibility.Visible;
                _movideoApp.ScanAsync(settings)
                    .ContinueWith(t =>
                    {
                        if (t.Exception != null)
                            Log.Error(t.Exception.Message, t.Exception);
                        ScanCompleted();
                    });
            }
            catch (Exception ex)
            {
                Dialogs.ShowError(ex.ToString());
            }
        }


        private void ScanCompleted()
        {
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;
        }
    }
}