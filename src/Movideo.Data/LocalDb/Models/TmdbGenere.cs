using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grappachu.Movideo.Data.LocalDb.Models
{
    [Table("TmdbGeneres")]
    public class TmdbGenere
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

    }
}