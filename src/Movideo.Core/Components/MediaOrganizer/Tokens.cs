using TMDbLib.Objects.TvShows;

namespace Grappachu.Movideo.Core.Components.MediaOrganizer
{
    public static class Tokens
    {
        public const string Title = "%title%";
        public const string Year = "%year%";
        public const string Extension = "%ext%";
        public const string Collection = "%collection%";
        public const string Genre = "%genre%";
        public const string AllGenres = "%genres%";
        public const string Rating = "%rating%";
        public const string Popularity = "%popularity%";

        public static readonly string[] List =
        {
            Title, Extension, Year, Collection, Genre, AllGenres, Rating, Popularity
        };
    }
}