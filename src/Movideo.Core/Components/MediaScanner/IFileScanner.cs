using System.Collections.Generic;
using System.IO;

namespace Grappachu.Movideo.Core.Components.MediaScanner
{
    public interface IFileScanner
    {
        IEnumerable<FileInfo> Scan();
    }
}