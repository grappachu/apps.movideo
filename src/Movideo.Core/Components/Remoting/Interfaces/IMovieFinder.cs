using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.Remoting.Models;

namespace Grappachu.Movideo.Core.Components.Remoting.Interfaces
{
    public interface IMovieFinder
    {
        MovieMatch FindMatch(AnalyzedItem item);
    }
}