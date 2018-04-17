using TMDbLib.Client;

namespace Grappachu.Movideo.Core.Components.Movies
{
    public interface ITmdbClientFactory
    {
        TMDbClient CreateClient();
    }
}