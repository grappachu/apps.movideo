using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Grappachu.Movideo.Core.Components.MediaScanner
{
    public class BasicFileScanner : IFileScanner
    {
        public IEnumerable<FileInfo> Scan(string path)
        {
            var str = path;

            if (File.Exists(str))
                return new[] {new FileInfo(str)};
            if (Directory.Exists(str))
            {
                var ff = Directory.GetFiles(str, "*.mkv", SearchOption.AllDirectories)
                    .Union(Directory.GetFiles(str, "*.avi", SearchOption.AllDirectories))
                    .Union(Directory.GetFiles(str, "*.mp4", SearchOption.AllDirectories));
                return ff.Select(x => new FileInfo(x));
            }

            return new FileInfo[0];
        }
    }
}