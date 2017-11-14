using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grappachu.Core.Lang.Text;
using Grappachu.Core.Preview.IO;
using Grappachu.Movideo.Core.Data.Model;
using Grappachu.Movideo.Core.Dtos;
using Grappachu.Movideo.Core.Interfaces;
using log4net;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace Grappachu.Movideo.Core
{
    public sealed class MovideoApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MovideoApp));
        private readonly IFileAnalyzer _analyzer;
        private readonly TMDbClient _apiClient;

        private readonly IMovieDb _db;
        private readonly IFileScanner _fileScanner;

        public MovideoApp(IConfigReader configReader, IFileScanner fileScanner, IFileAnalyzer analyzer, IMovieDb db)
        {
            _fileScanner = fileScanner;
            _analyzer = analyzer;
            _db = db;

            ApiSettings apiSettings = configReader.GetApiSettings();
            if (apiSettings == null)
            {
                throw new ArgumentNullException("Impossibile proseguire. API non configurata.");
            }
            _apiClient = new TMDbClient(apiSettings.ApiKey)
            {
                DefaultCountry = "IT",
                DefaultLanguage = "it"
            };
        }

        public event EventHandler<MatchFoundEventArgs> MatchFound;

        public Task<int> ScanAsync(MovideoSettings settings)
        {
            var tf = new TaskFactory<int>();
            return tf.StartNew(() => Scan(settings));
        }

        public int Scan(MovideoSettings settings)
        {
            var count = 0;

            var files = _fileScanner.Scan();
            foreach (var file in files)
            {
                var item = _analyzer.Analyze(file);
                if (!item.IsKnown)
                {
                    float accuracy;
                    var res = TryIdentify(item, out accuracy);
                    if (res != null)
                    {
                        var args = new MatchFoundEventArgs(item, res, accuracy);
                        OnMatchFound(args);

                        if (args.IsMatch == true)
                        {
                            DoRename(args, settings);

                            UpdateItem(item, res);
                            count++;
                        }
                    }
                }
                else
                {
                    float accuracy;
                    var res = TryIdentify(item, out accuracy);
                    var args = new MatchFoundEventArgs(item, res, 1);
                    OnMatchFound(args);

                    if (args.IsMatch == true)
                    {
                        DoRename(args, settings);
                    }
                }
            }

            return count;
        }

        private void DoRename(MatchFoundEventArgs args, MovideoSettings settings)
        {
            if (settings.Reorganize)
            {
                var fname =GetRenamedPath(args.LocalFile, args.Movie);
                var target = Path.Combine(settings.TargetPath, fname);
                var targetPath = SafeAddSuffix(target);

                FilesystemTools.SafeCreateDirectory(Path.GetDirectoryName(targetPath));

                File.Move(args.LocalFile.Path.FullName, targetPath);
                Log.InfoFormat("Match Saved: {0} ==> {1}", args.LocalFile.Path.Name, targetPath);
            }
        }

        private static string SafeAddSuffix(string fullPath)
        {
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }


        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        private TmdbMovie TryIdentify(AnalyzedItem item, out float matchAccuracy)
        {
            Log.DebugFormat("Querying remote: {0} ({1})", item.Title, item.Year);
            IEnumerable<string> tokens = new[] { item.Title };
            var matches = new List<Movie>();
            foreach (var token in tokens)
            {
                var results = _apiClient.SearchMovieAsync(token).Result;

                Log.Debug($"Got {results.Results.Count:N0} of {results.TotalResults:N0} results");
                foreach (var result in results.Results)
                {
                    Log.DebugFormat(" => {0}| {1} / {2} ({3})", result.Id, result.Title, result.OriginalTitle,
                        result.ReleaseDate.GetValueOrDefault().Year);


                    var movieTask = _apiClient.GetMovieAsync(result.Id);
                    movieTask.Wait();
                    var movie = movieTask.Result;

                    matches.Add(movie);
                }
            }


            var resChart = matches
                .Select(x => new { Movie = MapDbItem(x), Match = GetMatch(x, item) })
                .OrderByDescending(z => z.Match).ToArray();



            var candidate = resChart.FirstOrDefault(x => x.Match >= 0.2f);
            if (candidate != null)
            {
                matchAccuracy = candidate.Match;

                _apiClient.GetConfig();
                candidate.Movie.ImageUri = _apiClient.GetImageUrl("w185", candidate.Movie.PosterPath).ToString();

                var movie = candidate.Movie;
                _db.Push(movie);

                _db.Push(item.Hash, movie.Id);

                return movie;
            }
            else
            {
                matchAccuracy = 0f;
            }


            return null;
        }

        public string GetRenamedPath(AnalyzedItem item, TmdbMovie movie)
        {
            var frenamed = CleanFileName(Path.ChangeExtension(movie.Title +
                                                (movie.Year.HasValue ? " (" + movie.Year.Value + ")" : string.Empty)
                                                + ".ext", item.Path.Extension));
            if (!String.IsNullOrEmpty(movie.Collection))
            {
                frenamed = Path.Combine(movie.Collection, frenamed);
            }
            return frenamed;
        }


        private static float GetMatch(Movie movie, AnalyzedItem item)
        {
            float res = 0;

            var dtt = LevenshteinDistance.ComputeIgnoreCase(item.Title, movie.Title);
            var dto = LevenshteinDistance.ComputeIgnoreCase(item.Title, movie.OriginalTitle);
            float score1 = Math.Max(0, 5 - Math.Min(dtt, dto));
            res += score1;

            //if (string.Equals(item.Title, movie.Title, StringComparison.OrdinalIgnoreCase)
            //    || string.Equals(item.Title, movie.OriginalTitle, StringComparison.OrdinalIgnoreCase))
            //{
            //    res += 3;
            //}

            if (!string.IsNullOrEmpty(item.SubTitle))
            {
                var dst = LevenshteinDistance.ComputeIgnoreCase(item.SubTitle, movie.Title);
                var dso = LevenshteinDistance.ComputeIgnoreCase(item.SubTitle, movie.OriginalTitle);
                float score2 = Math.Max(0, 3 - Math.Min(dst, dso));
                res += score2;
            }

            //if (string.Equals(item.SubTitle, movie.Title, StringComparison.OrdinalIgnoreCase)
            //    || string.Equals(item.SubTitle, movie.OriginalTitle, StringComparison.OrdinalIgnoreCase))
            //{
            //    res += 2;
            //}

            if (LooksLike(item.Year, movie.ReleaseDate))
            {
                res += 3;
            }


            if (item.Duration > TimeSpan.Zero && movie.Runtime.HasValue)
            {
                var mtime = TimeSpan.FromMinutes(movie.Runtime.Value);
                var diff = Math.Abs(item.Duration.Subtract(mtime).TotalMinutes);
                var score = (int)Math.Max(5 - diff, 0);
                {
                    res += score;
                }
            }
            else
            {
                res += 1;
            }

            return res / 16;
        }

        private static TmdbMovie MapDbItem(Movie match)
        {
            var res = new TmdbMovie
            {
                Id = match.Id,
                Title = match.Title,
                OriginalTitle = match.OriginalTitle,
                ReleaseDate = match.ReleaseDate,
                Overview = match.Overview,
                Duration = TimeSpan.FromMinutes(match.Runtime.GetValueOrDefault()),
                Adult = match.Adult,
                Genres = match.Genres.Select(g => new TmdbGenere { Id = g.Id, Name = g.Name }).ToArray(),
                ImdbId = match.ImdbId,
                OriginalLanguage = match.OriginalLanguage,
                Popularity = match.Popularity,
                PosterPath = match.PosterPath,
                VoteAverage = match.VoteAverage,
                VoteCount = match.VoteCount,
                Collection = match.BelongsToCollection?.Name.Replace(" - Collezione", string.Empty)

            };


            ////   match.Similar;
            //  // match.Videos;
            return res;
        }

        private static bool LooksLike(int? itemYear, DateTime? matchReleaseDate)
        {
            if (itemYear.HasValue && matchReleaseDate.HasValue)
            {
                var yd = Math.Abs(itemYear.Value - matchReleaseDate.Value.Year);
                return yd <= 1;
            }
            return false;
        }


        public void UpdateItem(AnalyzedItem item, TmdbMovie res)
        {
        }

        private void OnMatchFound(MatchFoundEventArgs e)
        {
            MatchFound?.Invoke(this, e);
        }


    }
}