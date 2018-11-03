using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.MediaScanner;
using Grappachu.Movideo.Core.Components.Remoting;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Data.LocalDb;
using Grappachu.Movideo.Data.Remoting;

namespace Grappachu.Apps.Movideo.Config
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
                var finder = new MovieFinder(new TmdbClientFactory(configReader));

                _app = new MovideoApp(configReader, scanner, analyzer, db, finder);
            }
            return _app;
        }
    }
}