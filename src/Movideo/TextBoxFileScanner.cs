using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grappachu.Movideo.Core.Interfaces;

namespace Grappachu.Apps.Movideo
{
    public class TextBoxFileScanner : IFileScanner
    {
        public string Path { get; set; }


        public IEnumerable<FileInfo> Scan()
        {
            string str = Path;
             

            if (File.Exists(str))
                return new[] { new FileInfo(str) };
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