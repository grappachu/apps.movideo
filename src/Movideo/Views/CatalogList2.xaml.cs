using System;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;

namespace Grappachu.Apps.Movideo.Views
{
    /// <summary>
    /// Logica di interazione per CatalogList.xaml
    /// </summary>
    public partial class CatalogList2 : UserControl
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CatalogList2));

        public CatalogList2()
        {
            InitializeComponent();
        }

        private void CommandDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandDelete_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = e.Parameter;
            if (parameter != null)
            {
                var movieId = Convert.ToInt32(parameter);



            }
            else
            {
                Log.Warn("Delete was called but no movie was supplied");
            }

        }
    }
}
