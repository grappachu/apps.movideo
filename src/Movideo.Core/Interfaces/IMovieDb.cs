using Grappachu.Movideo.Core.Data.Model;
using Grappachu.Movideo.Core.Dtos;
using Grappachu.Movideo.Core.Utils;
using TMDbLib.Objects.Movies;

namespace Grappachu.Movideo.Core.Interfaces
{
    public interface IMovieDb
    {
        bool HasMatch(AnalyzedItem item);
        bool HasHash(FileRef item);
        string GetHashFor(FileRef item);
        int? GetMovieIdFor(AnalyzedItem item);
        void Push(FileRef fref, string hash);
        void Push(TmdbMovie movieTaskResult);
        void Push(string hash, int movieId);
    }
}