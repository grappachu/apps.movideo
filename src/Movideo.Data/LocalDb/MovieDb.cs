using System.Collections.Generic;
using System.Diagnostics;
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

        private static MovideoModel CreateContext()
        {
            return new MovideoModel();
        }



        public bool HasMatch(AnalyzedItem item)
        {
            using (var ctx = CreateContext())
            {
                return ctx.MediaBindings.Any(x => x.Hash == item.Hash && x.MovieId.HasValue);

            }
        }

        public bool HasHash(FileRef fref)
        {
            using (var ctx = CreateContext())
            {
                var key = GetKey(fref);
                var mf = ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
                if (!string.IsNullOrEmpty(mf?.Hash) && mf.Bytes == fref.Bytes &&
                    mf.LastModifiedUtc == fref.LastModifiedUtc)
                    return true;
                return false;
            }
        }

        public string GetHashFor(FileRef fref)
        {
            using (var ctx = CreateContext())
            {
                var key = GetKey(fref);
                var mf = ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
                if (!string.IsNullOrEmpty(mf?.Hash) && mf.Bytes == fref.Bytes &&
                    mf.LastModifiedUtc == fref.LastModifiedUtc)
                    return mf.Hash;
                return null;
            }
        }

        public int? GetMovieIdFor(AnalyzedItem item)
        {
            using (var ctx = CreateContext())
            {
                return ctx.MediaBindings.First(x => x.Hash == item.Hash).MovieId;
            }
        }

        public void Push(FileRef fref, string hash)
        {
            //Verific l'esistenza di un binding
            using (var ctx = CreateContext())
            {
                var bind = ctx.MediaBindings.SingleOrDefault(x => x.Hash == hash);
                if (bind == null)
                {
                    bind = ctx.MediaBindings.Create();
                    bind.Hash = hash;
                    ctx.MediaBindings.Add(bind);
                }

                // Verifica l'esistenda di un file
                var key = GetKey(fref);
                var file = ctx.MediaFiles.SingleOrDefault(x => x.Key == key);
                if (file == null)
                {
                    file = ctx.MediaFiles.Create();
                    file.Key = key;
                    ctx.MediaFiles.Add(file);
                }

                // Aggiorna il file
                file.Path = fref.Path;
                file.Bytes = fref.Bytes;
                file.Hash = hash;
                file.LastModifiedUtc = fref.LastModifiedUtc;

                ctx.SaveChanges();
            }
        }


        public Movie GetMovie(int movieId)
        {
            using (var ctx = CreateContext())
            {
                var dbMovie = ctx.TmdbMovies.SingleOrDefault(x => x.Id == movieId);

                var movie = Mapper.Instance.Map<Movie>(dbMovie);

                return movie;
            }
        }

        public IEnumerable<Movie> GetMovies()
        {
            using (var ctx = CreateContext())
            {
                var dbItems = ctx.MediaBindings
                    .Include(nameof(DbMediaBinding.Movie))
                    .Include(nameof(DbMediaBinding.Movie) + "." + nameof(TmdbMovie.Genres))
                    .Where(mb => mb.Movie != null)
                    .Select(mb => mb.Movie)
                    .ToArray();
                var mappedItems = dbItems.Select(Mapper.Map<Movie>)
                    .ToArray();
                return mappedItems;
            }
        }



        public void Push(Movie movie)
        {
            Debug.Assert(!string.IsNullOrEmpty(movie.ImdbId));

            using (var ctx = CreateContext())
            {
                var mov = ctx.TmdbMovies
                    .Include(nameof(TmdbMovie.Genres))
                    .SingleOrDefault(x => x.ImdbId == movie.ImdbId);

                if (mov == null)
                {
                    mov = ctx.TmdbMovies.Create();
                    ctx.TmdbMovies.Add(mov);
                }

                mov.Adult = movie.Adult;
                mov.Collection = movie.Collection;
                mov.ImageUri = movie.ImageUri;
                mov.ImdbId = movie.ImdbId;
                mov.Duration = movie.Duration;
                mov.OriginalLanguage = movie.OriginalLanguage;
                mov.OriginalTitle = movie.OriginalTitle;
                mov.Overview = movie.Overview;
                mov.Popularity = movie.Popularity;
                mov.PosterPath = movie.PosterPath;
                mov.ReleaseDate = movie.ReleaseDate;
                mov.Title = movie.Title;
                mov.VoteAverage = movie.VoteAverage;
                mov.VoteCount = movie.VoteCount;

                //  mov.Genres   
                var genList = new List<TmdbGenere>();
                foreach (var genre in movie.Genres)
                {
                    var dbGenre = GetOrCreateGenre(ctx, genre);
                    genList.Add(dbGenre);
                }
                mov.Genres = genList;

                ctx.SaveChanges();

                movie.Id = mov.Id;
            }
        }

        private static TmdbGenere GetOrCreateGenre(MovideoModel ctx, MovieGenere genre)
        {
            var dbGenre = ctx.TmdbGeneres.SingleOrDefault(x => x.Id == genre.Id);
            if (dbGenre == null)
            {
                dbGenre = ctx.TmdbGeneres.Add(new TmdbGenere { Id = genre.Id, Name = genre.Name });
            }
            return dbGenre;
        }


        public void Push(string hash, int movieId)
        {
            using (var ctx = CreateContext())
            {
                var binding = ctx.MediaBindings.SingleOrDefault(x => x.Hash == hash);
                if (binding == null)
                {
                    binding = ctx.MediaBindings.Create();
                    binding.Hash = hash;
                    ctx.MediaBindings.Add(binding);
                }

                binding.MovieId = movieId;
                ctx.SaveChanges();
            }
        }


        private static string GetKey(FileRef fref)
        {
            return MD5.HashString(fref.Path).ToString(BinaryRepresentation.Hex);
        }
    }
}