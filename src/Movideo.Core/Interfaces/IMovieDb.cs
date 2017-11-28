using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Utils;
using Movie = Grappachu.Movideo.Core.Models.Movie;

namespace Grappachu.Movideo.Core.Interfaces
{
    public interface IMovieDb
    {
        bool HasMatch(AnalyzedItem item);
        bool HasHash(FileRef item);
        string GetHashFor(FileRef item);
        int? GetMovieIdFor(AnalyzedItem item);
        void Push(FileRef fref, string hash);
        void Push(Movie movieTaskResult);
        void Push(string hash, int movieId);
    }
}