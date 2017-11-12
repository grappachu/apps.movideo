using System.Collections.Generic;
using System.IO;

namespace Grappachu.Movideo.Core.Interfaces
{
    public interface IFileScanner
    {
        IEnumerable<FileInfo> Scan();
    }
}