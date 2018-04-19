using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.MediaOrganizer;
using Grappachu.Movideo.Core.Components.MediaScanner;
using Grappachu.Movideo.Core.Components.Remoting.Interfaces;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Core.Models;
using log4net;

namespace Grappachu.Movideo.Core
{
    public sealed class MovideoApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MovideoApp));

        private readonly IFileAnalyzer _analyzer;
        private readonly IMovieDb _db;
        private readonly IMovieFinder _movieFinder;
        private readonly IConfigReader _configReader;
        private readonly IFileScanner _fileScanner;

        public MovideoApp(IConfigReader configReader, IFileScanner fileScanner, IFileAnalyzer analyzer, IMovieDb db, IMovieFinder movieFinder)
        {
            _configReader = configReader;
            _fileScanner = fileScanner;
            _analyzer = analyzer;
            _db = db;
            _movieFinder = movieFinder;

            ApiSettings apiSettings = _configReader.GetApiSettings();
            if (apiSettings == null)
            {
                throw new ArgumentNullException("Impossibile proseguire. API non configurata.");
            }

            CurrentJob = _configReader.GetJobSettings();
        }

        public event EventHandler<MatchFoundEventArgs> MatchFound;
        public event ProgressChangedEventHandler ProgressChanged;

        public JobSettings CurrentJob { get; }

        public Task<int> ScanAsync(MovideoSettings settings)
        {
            var tf = new TaskFactory<int>();
            return tf.StartNew(() => Scan(settings));
        }

        public int Scan(MovideoSettings settings)
        {
            var count = 0;
            var index = 0;

            var files = _fileScanner.Scan(settings.SourcePath).ToArray();
            var totalItems = files.Length;

            foreach (var file in files)
            {
                index++;
                OnProgressChanged(index, totalItems);

                var item = _analyzer.Analyze(file);
                float accuracy;
                if (!item.IsKnown)
                {
                    var res = TryIdentify(item, out accuracy);
                    if (res != null)
                    {
                        var args = new MatchFoundEventArgs(item, res, accuracy);
                        OnMatchFound(args);

                        if (args.Cancel)
                        {
                            break;
                        }

                        if (args.IsMatch == true)
                        {
                            _db.Push(args.Movie);
                            _db.Push(item.Hash, args.Movie.Id);

                            DoRename(args, settings);

                            UpdateItem(item, res);
                            count++;
                        }
                    }
                }
                else
                {
                    var movieId = _db.GetMovieIdFor(item);
                    var movie = _db.GetMovie(movieId.Value);
                    item.Title = movie.Title;
                    item.Year = movie.Year;

                    var res = TryIdentify(item, out accuracy);
                    var args = new MatchFoundEventArgs(item, res, 1);
                    DoRename(args, settings);
                }
            }

            return count;
        }

        private static void DoRename(MatchFoundEventArgs args, MovideoSettings settings)
        {
            if (settings.Reorganize)
            {
                var sourcePath = args.LocalFile.Path;
                IFolderCleaner cleaner = settings.DeleteEmptyFolders ? new FolderCleaner() : null;
                var organizer = new FileOrganizer(settings.TargetPath, settings.RenameTemplate, cleaner);
                organizer.Organize(sourcePath, args.Movie);
            }
        }

        private Movie TryIdentify(AnalyzedItem item, out float matchAccuracy)
        {
            var candidate = _movieFinder.FindMatch(item);
            if (candidate == null)
            {
                matchAccuracy = 0f;
                return null;
            }

            matchAccuracy = candidate.MatchAccuracy;

            var movie = candidate.Movie; 

            return movie;
        }

        public void UpdateItem(AnalyzedItem item, Models.Movie res)
        {
        }

        private void OnMatchFound(MatchFoundEventArgs e)
        {
            MatchFound?.Invoke(this, e);
        }

        private void OnProgressChanged(int current, int total)
        {
            if (total > 0)
            {
                var p = 100 * current / total;
                ProgressChangedEventArgs e = new ProgressChangedEventArgs(p, null);
                ProgressChanged?.Invoke(this, e);
            }
        }

        public IEnumerable<Movie> GetCatalog()
        {
            return _db.GetMovies();
        }
    }
}