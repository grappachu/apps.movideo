using System;
using System.Collections.Generic;

namespace Grappachu.Movideo.Core.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string OriginalTitle { get; set; }

        public TimeSpan Duration { get; set; }
        public int? Year => ReleaseDate?.Year;

        public string Title { get; set; }

        public bool Adult { get; set; }

        public string ImdbId { get; set; }

        public string OriginalLanguage { get; set; }

        public string Overview { get; set; }
        public double Popularity { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }

        public string PosterPath { get; set; }

        public ICollection<MovieGenere> Genres { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string Collection { get; set; }

        public string ImageUri { get; set; }
    }
}