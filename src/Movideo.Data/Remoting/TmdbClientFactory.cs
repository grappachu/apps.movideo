using System;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Components.Movies;
using Grappachu.Movideo.Core.Interfaces;
using TMDbLib.Client;

namespace Grappachu.Movideo.Data.Remoting
{
    public class TmdbClientFactory : ITmdbClientFactory
    {
        private readonly IConfigReader _configReader;

        public TmdbClientFactory(IConfigReader configReader)
        {
            _configReader = configReader;
        }


        public TMDbClient CreateClient()
        {
            ApiSettings apiSettings = _configReader.GetApiSettings();
            if (apiSettings == null)
            {
                throw new ArgumentNullException("Impossibile proseguire. API non configurata.");
            }

            return new TMDbClient(apiSettings.ApiKey)
            {
                DefaultCountry = "IT",
                DefaultLanguage = apiSettings.ApiCulture.TwoLetterISOLanguageName
            };
        }
    }
}