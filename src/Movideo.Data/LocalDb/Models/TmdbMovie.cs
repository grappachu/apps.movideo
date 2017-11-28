using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grappachu.Movideo.Data.LocalDb.Models
{
    [Table("TmdbMovies")]
    public class TmdbMovie
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string OriginalTitle { get; set; }

        public TimeSpan Duration { get; set; }
        public int? Year => ReleaseDate?.Year;

        [MaxLength(255)]
        public string Title { get; set; }

        public bool Adult { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "varchar")]
        public string ImdbId { get; set; }

        [MaxLength(2)]
        [Column(TypeName = "varchar")]
        public string OriginalLanguage { get; set; }

        public string Overview { get; set; }
        public double Popularity { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "varchar")]
        public string PosterPath { get; set; }

        public IEnumerable<TmdbGenere> Genres { get; set; }
        public DateTime? ReleaseDate { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string Collection { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImageUri { get; set; }
    }
}