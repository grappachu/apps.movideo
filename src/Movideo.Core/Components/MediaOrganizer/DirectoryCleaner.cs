using System.IO;
using Grappachu.Movideo.Core.Utils;

namespace Grappachu.Movideo.Core.Components.MediaOrganizer
{
    public static class DirectoryCleaner
    {
        /// <summary>
        ///     Removes a empty directory deleting recursively all parents.
        /// </summary>
        /// <returns>number of deleted folders</returns>
        public static int CleanUp(DirectoryInfo directory)
        { 
            var deleted = 0;
            if (directory.Exists)
            {
                var toDelete = directory;
                var parent = directory.Parent;
                while (toDelete.IsEmpty())
                {
                    toDelete.Delete();
                    deleted++;
                    toDelete = parent;
                    if (parent == null) break;
                    parent = toDelete.Parent;
                }
            }

            return deleted;
        }

        public static int CleanUp(string directory)
        {
            return CleanUp(new DirectoryInfo(directory));
        }
    }
}