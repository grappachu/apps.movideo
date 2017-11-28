namespace Grappachu.Movideo.Core.Models
{
    public class MediaBinding
    {
        public string Hash { get; set; }

     
        public int? MovieId { get; set; }
         

        public Movie Movie { get; set; }
    }


}