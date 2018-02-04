using System.Collections.Generic;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Movie = Grappachu.Movideo.Core.Models.Movie;

namespace Grappachu.Movideo.Core.Interfaces
{
    public interface IMovieDb
    {
        bool HasMatch(AnalyzedItem item);
        bool HasHash(FileRef item);
        string GetHashFor(FileRef item);
        int? GetMovieIdFor(AnalyzedItem item);
        Movie GetMovie(int movieId);
        void Push(FileRef fref, string hash);
        void Push(Movie movieTaskResult);
        void Push(string hash, int movieId);
        IEnumerable<Movie> GetMovies();
    }
}