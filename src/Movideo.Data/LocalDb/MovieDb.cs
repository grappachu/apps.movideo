using System.Collections.Generic;
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
            _ctx.TmdbMovies.Count();
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
            //Verific l'esistenza di un binding
            var bind = _ctx.MediaBindings.SingleOrDefault(x => x.Hash == hash);
            if (bind == null)
            {
                bind = _ctx.MediaBindings.Create();
                bind.Hash = hash;
                _ctx.MediaBindings.Add(bind);
            }

            // Verifica l'esistenda di un file
            var key = GetKey(fref);
            var file = _ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
            if (file == null)
            {
                file = _ctx.MediaFiles.Create();
                file.Key = key;
                _ctx.MediaFiles.Add(file);
            }

            // Aggiorna il file
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

        public IEnumerable<Movie> GetMovies()
        {
            return _ctx.MediaBindings
                .Include(nameof(MediaBinding.Movie))
                .Where(mb => mb.Movie != null)
                .Select(mb => mb.Movie)
                .ToArray()
                .Select(Mapper.Map<Movie>)
                .ToArray();
        }



        public void Push(Movie movie)
        {
            TmdbMovie dbMovie = Mapper.Instance.Map<TmdbMovie>(movie);

            var mov = _ctx.TmdbMovies
                // .Include(nameof(TmdbMovie.Genres))
                .SingleOrDefault(x => x.ImdbId == dbMovie.ImdbId); 

            if (mov != null)
            {
                // Update Values
                //   Mapper.Instance.Map(dbMovie, mov);  
                mov.Adult = dbMovie.Adult;
                mov.Collection = dbMovie.Collection;
                mov.ImageUri = dbMovie.ImageUri;

                mov.ImdbId = mov.ImdbId;
                mov.Duration = mov.Duration;
                mov.OriginalLanguage = dbMovie.OriginalLanguage;
                mov.OriginalTitle = dbMovie.OriginalTitle;
                mov.Overview = dbMovie.Overview;
                mov.Popularity = dbMovie.Popularity;
                mov.PosterPath = dbMovie.PosterPath;
                mov.ReleaseDate = dbMovie.ReleaseDate;
                mov.Title = dbMovie.Title;
                mov.VoteAverage = dbMovie.VoteAverage;
                mov.VoteCount = dbMovie.VoteCount;

                ////  mov.Genres
                //foreach (var dbMovieGenre in dbMovie.Genres)
                //{
                //    var origGenre = mov.Genres.SingleOrDefault(x => x.Id == dbMovieGenre.Id);
                //    if (origGenre != null)
                //    {
                //        origGenre.Name = dbMovieGenre.Name;
                //    }
                //    else
                //    {

                //    }   
                //}
                
                movie.Id = mov.Id;
                _ctx.SaveChanges(); 
            }
            else
            {
                _ctx.TmdbMovies.Add(dbMovie);
                _ctx.SaveChanges();
                movie.Id = dbMovie.Id;
            }

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