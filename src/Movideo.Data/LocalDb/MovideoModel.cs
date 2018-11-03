using System.Data.Entity;
using Grappachu.Movideo.Data.LocalDb.Models;

namespace Grappachu.Movideo.Data.LocalDb
{
    public class MovideoModel : DbContext
    {
        public MovideoModel()
            : base("name=MovideoModel")
        {
        }

        public virtual DbSet<DbMediaFile> MediaFiles { get; set; }
        public virtual DbSet<DbMediaBinding> MediaBindings { get; set; }
        public virtual DbSet<TmdbMovie> TmdbMovies { get; set; }
        public virtual DbSet<TmdbGenere> TmdbGeneres { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TmdbMovie>()
                .HasMany(s => s.Genres)
                .WithMany(c => c.Movies)
                .Map(cs =>
                {
                    cs.MapLeftKey("MovieId");
                    cs.MapRightKey("GenereId");
                    cs.ToTable("MovieGeneres");
                });
        }

    }
}