using System;
using System.Collections.Generic;
using System.Linq;
using Grappachu.Core.Lang.Text;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.Movies;
using Grappachu.Movideo.Core.Components.Remoting.Interfaces;
using Grappachu.Movideo.Core.Components.Remoting.Models;
using Grappachu.Movideo.Core.Models;
using log4net;
using Movie = TMDbLib.Objects.Movies.Movie;

namespace Grappachu.Movideo.Core.Components.Remoting
{
    public class MovieFinder : IMovieFinder
    {
        private readonly ITmdbClientFactory _apiClientFactory;
        private static readonly ILog Log = LogManager.GetLogger(typeof(MovieFinder));
        private static float _titlePound = 1f;
        private static float _origTitlePound = 1f;
        private static float _yearPound = 1f;
        private static float _lengthPound = 1f;

        public MovieFinder(ITmdbClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        public MovieMatch FindMatch(AnalyzedItem item)
        {
            Log.DebugFormat("Querying remote: {0} ({1})", item.Title, item.Year);
            var apiClient = _apiClientFactory.CreateClient();

            IEnumerable<string> tokens = new[] { string.Format("{0} {1}", item.Title, item.SubTitle), item.Title, item.SubTitle }
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(x => x.Trim())
                .Distinct();

            var matches = new List<Movie>();
            foreach (var token in tokens)
            {
                var results = apiClient.SearchMovieAsync(token).Result;

                Log.Debug($"Got {results.Results.Count:N0} of {results.TotalResults:N0} results");
                foreach (var result in results.Results)
                {
                    Log.DebugFormat(" => {0}| {1} / {2} ({3})", result.Id, result.Title, result.OriginalTitle,
                        result.ReleaseDate.GetValueOrDefault().Year);

                    var movieTask = apiClient.GetMovieAsync(result.Id);
                    movieTask.Wait();
                    var movie = movieTask.Result;

                    matches.Add(movie);
                }
            }

            if (matches.Any())
            { 
                var maxPop = matches.Max(x => x.Popularity);
                var resChart = matches
                    .Select(x => new MovieMatch { Movie = MapDbItem(x), MatchAccuracy = GetMatch(x, item, maxPop) })
                    .OrderByDescending(z => z.MatchAccuracy).ToArray();

                var candidate = resChart.FirstOrDefault(x => x.MatchAccuracy >= 0.2f);

                if (candidate != null)
                {
                    apiClient.GetConfig();
                    candidate.Movie.ImageUri = apiClient.GetImageUrl("w185", candidate.Movie.PosterPath).ToString();
                }

                return candidate;
            }

            return null;
        }


        public static float GetMatch(Movie movie, AnalyzedItem item, double maxPop)
        {
            float res = 0;
            int parts = 0;

            var titleMatch = MatchTitle(item, movie);
            if (titleMatch.HasValue)
            {
                res += titleMatch.Value * _titlePound;
                parts++;
            }

            var origTitleMatch = MatchSubTitle(item, movie);
            if (origTitleMatch.HasValue)
            {
                res += origTitleMatch.Value * _origTitlePound;
                parts++;
            }

            var yearMatch = MatchYear(item, movie);
            if (yearMatch.HasValue)
            {
                res += yearMatch.Value * _yearPound;
                parts++;
            }
            else
            {
                res += 0.5f;
                parts++;
            }

            var lengthMatch = MatchLength(item, movie);
            if (lengthMatch.HasValue)
            {
                res += lengthMatch.Value * _lengthPound;
                parts++;
            }
            else
            {
                res += 0.2f;
                parts++;
            }


            res += (float)movie.Popularity / (float)maxPop;
            parts++;

            var score = res / parts;
            return score;
        }




        private static float? MatchTitle(AnalyzedItem item, Movie movie)
        {
            var lScore = LevenshteinDistance.ComputeIgnoreCase(item.Title, movie.Title);
            var oScore = LevenshteinDistance.ComputeIgnoreCase(item.Title, movie.OriginalTitle);

            var lp = (float)lScore / Math.Max(item.Title.Length, movie.Title.Length);
            var op = (float)oScore / Math.Max(item.Title.Length, movie.OriginalTitle.Length);

            float best = 1 - Math.Min(lp, op);
            return best;
        }

        private static float? MatchSubTitle(AnalyzedItem item, Movie movie)
        {
            if (!string.IsNullOrEmpty(item.SubTitle))
            {
                var dst = LevenshteinDistance.ComputeIgnoreCase(item.SubTitle, movie.Title);
                var dso = LevenshteinDistance.ComputeIgnoreCase(item.SubTitle, movie.OriginalTitle);
                //  var fst = LevenshteinDistance.ComputeIgnoreCase(item.Title + " " + item.SubTitle, movie.Title);
                //  var fso = LevenshteinDistance.ComputeIgnoreCase(item.Title + " " + item.SubTitle, movie.OriginalTitle);

                var lp = (float)dst / Math.Max(item.Title.Length, movie.Title.Length);
                var op = (float)dso / Math.Max(item.Title.Length, movie.OriginalTitle.Length);

                float best = 1 - Math.Min(lp, op);
                return best;
            }

            return null;
        }

        private static float? MatchYear(AnalyzedItem item, Movie movie)
        {
            const int k = 10;
            if (item.Year.HasValue && movie.ReleaseDate.HasValue)
            {
                var yd = Math.Abs(item.Year.Value - movie.ReleaseDate.Value.Year);
                var square = (float)k - Math.Min(yd * yd, k);
                float perc = square / k;
                return perc;
            }
            return null;
        }

        private static float? MatchLength(AnalyzedItem item, Movie movie)
        {
            const int k = 20;
            if (item.Duration > TimeSpan.Zero && movie.Runtime.HasValue)
            {
                var mtime = TimeSpan.FromMinutes(movie.Runtime.Value);
                var diff = Math.Abs(item.Duration.Subtract(mtime).TotalMinutes);

                var square = k - (float)Math.Min(diff, k);
                float perc = square / k;

                return perc;
            }

            return null;
        }

        private static Core.Models.Movie MapDbItem(Movie match)
        {
            var res = new Core.Models.Movie
            {
                Id = match.Id,
                Title = match.Title,
                OriginalTitle = match.OriginalTitle,
                ReleaseDate = match.ReleaseDate,
                Overview = match.Overview,
                Duration = TimeSpan.FromMinutes(match.Runtime.GetValueOrDefault()),
                Adult = match.Adult,
                Genres = match.Genres.Select(g => new MovieGenere { Id = g.Id, Name = g.Name }).ToArray(),
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



    }
}