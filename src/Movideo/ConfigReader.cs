using System;
using System.Globalization;
using Grappachu.Apps.Movideo.Properties;
using Grappachu.Apps.Movideo.UI.Dialogs;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Interfaces;

namespace Grappachu.Apps.Movideo
{
    public class ConfigReader : IConfigReader
    {
        public ApiSettings GetApiSettings()
        {
            var settings = new ApiSettings();

            if (string.IsNullOrEmpty(Settings.Default.TmdbApiKey))
            {
                if (ApiKeyDialog.Prompt(settings))
                {
                    Settings.Default.TmdbApiKey = settings.ApiKey;
                    Settings.Default.TmdbApiCulture = settings.ApiCulture.ToString();
                    Settings.Default.Save();
                }
                else
                {
                    throw new ArgumentNullException("Api Key not set. Cannot continue");
                }
            }
            else
            {
                settings.ApiKey = Settings.Default.TmdbApiKey;
                settings.ApiCulture = CultureInfo.GetCultureInfo( Settings.Default.TmdbApiCulture);
            }

            return settings;
           
        }
    }
}