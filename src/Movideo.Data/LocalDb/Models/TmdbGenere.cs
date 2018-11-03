using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Grappachu.Movideo.Data.LocalDb.Models
{
    [Table("TmdbGeneres")]
    public class TmdbGenere
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        [Index("UK_Genere_Name", IsUnique = true)]
        public string Name { get; set; }
        
        public virtual ICollection<TmdbMovie> Movies { get; set; }
    }
}