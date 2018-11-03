using System.Windows;
using Grappachu.Apps.Movideo.Config;
using log4net.Config;

namespace Grappachu.Apps.Movideo
{
    /// <summary>
    ///     Logica di interazione per App.xaml
    /// </summary>
    public partial class App
    {

        public App()
        {
            XmlConfigurator.Configure();
            AutoMapperConfigurator.Configure();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
           
        }
    }
}