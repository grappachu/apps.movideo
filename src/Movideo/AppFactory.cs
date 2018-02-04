using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.MediaScanner;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Data.LocalDb;

namespace Grappachu.Apps.Movideo
{
    internal static class AppFactory
    {
        private static MovideoApp _app;

        public static MovideoApp GetInstance()
        {
            if (_app == null)
            {
                IMovieDb db = new MovieDb();
                var configReader = new ConfigReader();
                var scanner = new BasicFileScanner();
                var analyzer = new FileAnalyzer(db);

                _app = new MovideoApp(configReader, scanner, analyzer, db);
            }
            return _app;
        }
    }
}