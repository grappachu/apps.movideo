using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grappachu.Movideo.Data.LocalDb.Models
{
    [Table("MediaFiles")]
    public class DbMediaFile
    {
        [Key]
        [MaxLength(32)]
        [Column(TypeName = "varchar")]
        public string Key { get; set; }

        [Column(TypeName = "varchar")]
        public string Path { get; set; }

        public long Bytes { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastModifiedUtc { get; set; }

        [MaxLength(32)]
        [Column(TypeName = "varchar")]
        [ForeignKey("Binding")]
        public string Hash { get; set; }

        public DbMediaBinding Binding { get; set; }
    }
}