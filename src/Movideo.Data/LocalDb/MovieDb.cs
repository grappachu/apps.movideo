using System.Linq;
using AutoMapper;
using Grappachu.Core.Lang.Extensions;
using Grappachu.Core.Lang.Text;
using Grappachu.Core.Security.Hashing;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Core.Models;
using Grappachu.Movideo.Data.LocalDb.Models;

namespace Grappachu.Movideo.Data.LocalDb
{
    public class MovieDb : IMovieDb
    {
        private readonly MovideoModel _ctx;


        public MovieDb()
        {
            _ctx = new MovideoModel();
        }


        public bool HasMatch(AnalyzedItem item)
        {
            return _ctx.MediaBindings.Any(x => x.Hash == item.Hash && x.MovieId.HasValue);

        }

        public bool HasHash(FileRef fref)
        {
            var key = GetKey(fref);
            var mf = _ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
            if (!string.IsNullOrEmpty(mf?.Hash) && mf.Bytes == fref.Bytes && mf.LastModifiedUtc == fref.LastModifiedUtc)
                return true;
            return false;
        }

        public string GetHashFor(FileRef fref)
        {
            var key = GetKey(fref);
            var mf = _ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
            if (!string.IsNullOrEmpty(mf?.Hash) && mf.Bytes == fref.Bytes && mf.LastModifiedUtc == fref.LastModifiedUtc)
                return mf.Hash;
            return null;
        }

        public int? GetMovieIdFor(AnalyzedItem item)
        {
            return _ctx.MediaBindings.First(x => x.Hash == item.Hash).MovieId;
        }

        public void Push(FileRef fref, string hash)
        {
            var bind = _ctx.MediaBindings.SingleOrDefault(x => x.Hash == hash);
            if (bind == null)
            {
                bind = _ctx.MediaBindings.Create();
                bind.Hash = hash;
                _ctx.MediaBindings.Add(bind);
            }

            var key = GetKey(fref);
            var file = _ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
            if (file == null)
            {
                file = _ctx.MediaFiles.Create();
                file.Key = key;
                _ctx.MediaFiles.Add(file);
            }

            file.Path = fref.Path;
            file.Bytes = fref.Bytes;
            file.Hash = hash;
            file.LastModifiedUtc = fref.LastModifiedUtc;

            _ctx.SaveChanges();
        }


        public Movie GetMovie(int movieId)
        {
            var dbMovie = _ctx.TmdbMovies.SingleOrDefault(x => x.Id == movieId);

            var movie = Mapper.Instance.Map<Movie>(dbMovie);

           return movie;
        }

        public void Push(Movie movie)
        {
            TmdbMovie dbMovie = Mapper.Instance.Map<TmdbMovie>(movie);

            var mov = _ctx.TmdbMovies.SingleOrDefault(x => x.Id == dbMovie.Id);
            if (mov != null)
            {
                _ctx.TmdbMovies.Remove(mov);
                _ctx.SaveChanges();
            }

            _ctx.TmdbMovies.Add(dbMovie);
            _ctx.SaveChanges();
            movie.Id = dbMovie.Id;
        }

        public void Push(string hash, int movieId)
        {
            var binding = _ctx.MediaBindings.SingleOrDefault(x => x.Hash == hash);
            if (binding == null)
            {
                binding = _ctx.MediaBindings.Create();
                binding.Hash = hash;
                _ctx.MediaBindings.Add(binding);
            }
            binding.MovieId = movieId;
            _ctx.SaveChanges();
        }

        private static string GetKey(FileRef fref)
        {
            return MD5.HashString(fref.Path).ToString(BinaryRepresentation.Hex);
        }
    }
}