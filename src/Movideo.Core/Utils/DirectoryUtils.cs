using System.IO;
using System.Linq;

namespace Grappachu.Movideo.Core.Utils
{
    public static class DirectoryUtils
    {
        /// <summary>
        /// Checks if a directory contains any file or folder
        /// </summary>
        public static bool IsEmpty(this DirectoryInfo dir)
        {
            return !dir.GetFiles().Any() && !dir.GetDirectories().Any();
        }
 
    }
}