using System.IO;
using Grappachu.Movideo.Core.Models;

namespace Grappachu.Movideo.Core.Components.MediaOrganizer
{
    public interface IFileOrganizer
    {
        string Organize(FileInfo itemPath, Movie movie);
    }
}