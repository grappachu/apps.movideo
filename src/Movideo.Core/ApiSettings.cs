using System.Globalization;

namespace Grappachu.Movideo.Core
{
    public class ApiSettings
    {
        public string ApiKey { get; set; }
        public CultureInfo ApiCulture { get; set; }

        public ApiSettings()
        {
            ApiCulture = CultureInfo.CurrentUICulture;
        }
    }
}