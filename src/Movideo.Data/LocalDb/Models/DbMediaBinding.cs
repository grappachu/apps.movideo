using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grappachu.Movideo.Data.LocalDb.Models
{
    [Table("MediaBindings")]
    public class DbMediaBinding
    {
        [MaxLength(32)]
        [Column(Order = 0, TypeName = "varchar")]
        [Key] 
        public string Hash { get; set; }

        
        [ForeignKey("Movie")]
        public int? MovieId { get; set; }
         

        public TmdbMovie Movie { get; set; }
    }


}