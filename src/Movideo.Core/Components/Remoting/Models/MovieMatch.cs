using Grappachu.Movideo.Core.Models;

namespace Grappachu.Movideo.Core.Components.Remoting.Models
{
    public class MovieMatch
    {
        public Movie Movie { get; set; }
        public float MatchAccuracy { get; set; }
    }
}