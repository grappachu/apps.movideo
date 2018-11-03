using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Grappachu.Movideo.Core.Models;

namespace Grappachu.Apps.Movideo.Views.Dialogs
{
    /// <summary>
    ///     Logica di interazione per MatchDialog.xaml
    /// </summary>
    public partial class MatchDialog
    {
        private MatchFoundEventArgs _args;

        public MatchDialog()
        {
            InitializeComponent(); 
        }


        public static void Prompt(MatchFoundEventArgs args)
        {
            var dlg = new MatchDialog
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,

                
                LblMatchScore = { Text = string.Format("{0:P0}", args.MatchAccuracy) },
                BadgeScore = { Background = GetColor(args.MatchAccuracy) },
                ImgPoster = { Source = new BitmapImage(new Uri(args.Movie.ImageUri)) },
                TxtMtitle = { Text = string.Format("Titolo: {0}", args.Movie.Title) },
                TxtOriginalTitle = { Text = string.Format("Titolo Originale: {0}", args.Movie.OriginalTitle) },
                TxtYear = { Text = string.Format("Anno: {0}", args.Movie.Year) },
                TxtDuration = { Text = string.Format("Durata: {0:N0} mins.", args.Movie.Duration.TotalMinutes) },
                TxtFname = { Text = string.Format("Nome File: {0}", args.LocalFile.Path.Name) },
                TxtFduration = { Text = string.Format("Durata: {0:N0} mins.", args.LocalFile.Duration.TotalMinutes) }
            };
            dlg._args = args;

            var res = dlg.ShowDialog();
            if (res == true)
            {
                args.IsMatch = dlg.IsMatch;
            }
            else
            {
                args.Cancel = true;
            }
        }

        private static Brush GetColor(double argsMatchAccuracy)
        {
            var g = (byte)(argsMatchAccuracy * 255);
            var r = (byte)(255 - g);
            Brush color = new SolidColorBrush(Color.FromRgb(r, g, 0));
            return color;
        }

        private void BtnMatch_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            IsMatch = true;
            Close();
        }

        public bool IsMatch { get; private set; }

        private void BtnFail_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            IsMatch = false;
            Close();
        }


        private void MatchDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_args.LocalFile.Path.FullName);
        }
    }
}