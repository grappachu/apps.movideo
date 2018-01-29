using System.IO;
using log4net;

namespace Grappachu.Movideo.Core.Components.MediaOrganizer
{
    public interface IFolderCleaner
    {
        void Clean(DirectoryInfo directory);
    }

    public class FolderCleaner : IFolderCleaner
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FolderCleaner));
        public void Clean(DirectoryInfo directory)
        {
            var deletes = DirectoryCleaner.CleanUp(directory);
            Log.DebugFormat("Directory Cleaned: {0} ({1} parents deleted)", directory, deletes - 1);
        }
    }
}