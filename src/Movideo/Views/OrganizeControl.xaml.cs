using System.Windows;
using System.Windows.Controls;
using Grappachu.Apps.Movideo.Properties;
using Grappachu.Apps.Movideo.ViewModels;
using log4net.Core;

namespace Grappachu.Apps.Movideo.Views
{
    /// <summary>
    ///     Logica di interazione per OrganizeControl.xaml
    /// </summary>
    public partial class OrganizeControl
    {
        public OrganizeControl()
        {
            InitializeComponent();
            LogViewer.DisplayLevel = Level.Debug;
            DataContextChanged += OrganizeControl_DataContextChanged;
            OnInit(DataContext as OrganizeControlPresenter);
            TxtFile.PropertyChanged += (sender, args) =>
            {
                Settings.Default.LastSourceFolder = TxtFile.SelectedValue; 
            };
            TxtTarget.PropertyChanged += (sender, args) =>
            {
                Settings.Default.LastOutputFolder = TxtTarget.SelectedValue;
            };
        }

        private void OrganizeControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uc = (UserControl)sender;
            var presenter = uc.DataContext as OrganizeControlPresenter;
            OnInit(presenter);
        }

        private void OnInit(OrganizeControlPresenter dc)
        {

            if (dc != null)
            {
                TxtFile.SelectedValue = dc.CurrentJob.SourceFolder;
                TxtTarget.SelectedValue = dc.CurrentJob.OutputFolder;
            }
            else
            {
                TxtFile.SelectedValue = string.Empty;
                TxtTarget.SelectedValue = string.Empty;
            }
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

    }
}