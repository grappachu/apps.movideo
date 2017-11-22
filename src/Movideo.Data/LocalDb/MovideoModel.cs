using System.Data.Entity;
using Grappachu.Movideo.Core.Data.Model;

namespace Grappachu.Movideo.Data.LocalDb
{
    public class MovideoModel : DbContext
    {
        public MovideoModel()
            : base("name=MovideoModel")
        {
        }

        // Aggiungere DbSet per ogni tipo di entità che si desidera includere nel modello. Per ulteriori informazioni 
        // sulla configurazione e sull'utilizzo di un modello Code, vedere http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MediaFile> MediaFiles { get; set; }

        public virtual DbSet<MediaBinding> MediaBindings { get; set; }

        public virtual DbSet<TmdbMovie> TmdbMovies { get; set; }
        public virtual DbSet<TmdbGenere> TmdbGeneres { get; set; }
    }
}