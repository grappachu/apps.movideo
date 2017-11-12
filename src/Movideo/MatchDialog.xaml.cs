using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Grappachu.Movideo.Core.Dtos;

namespace SistemafilmTest
{
    /// <summary>
    ///     Logica di interazione per MatchDialog.xaml
    /// </summary>
    public partial class MatchDialog : Window
    {
        public MatchDialog()
        {
            InitializeComponent();
        }


        public static void Prompt(MatchFoundEventArgs args)
        {
            var dlg = new MatchDialog();

            dlg.LblMatchScore.Text = string.Format("{0:P0}", args.MatchAccuracy);
            dlg.BadgeScore.Background = GetColor(args.MatchAccuracy);
            dlg.ImgPoster.Source = new BitmapImage(new Uri(args.Movie.ImageUri));

            dlg.TxtMtitle.Text = string.Format("Titolo: {0}", args.Movie.Title);
            dlg.TxtOriginalTitle.Text = string.Format("Titolo Originale: {0}", args.Movie.OriginalTitle);
            dlg.TxtYear.Text = string.Format("Anno: {0}", args.Movie.Year);
            dlg.TxtDuration.Text = string.Format("Durata: {0:N0} mins.", args.Movie.Duration.TotalMinutes);

            dlg.TxtFname.Text = string.Format("Nome File: {0}", args.LocalFile.Path.Name);
            dlg.TxtFduration.Text = string.Format("Durata: {0:N0} mins.", args.LocalFile.Duration.TotalMinutes);

            args.IsMatch = dlg.ShowDialog();
        }

        private static Brush GetColor(double argsMatchAccuracy)
        {
            var g = (byte) (argsMatchAccuracy * 255);
            var r = (byte) (255 - g);
            Brush color = new SolidColorBrush(Color.FromRgb(r, g, 0));
            return color;
        }

        private void BtnMatch_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnFail_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}