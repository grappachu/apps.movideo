using System;
using System.IO;
using Grappachu.Core.Preview.IO;
using Grappachu.Movideo.Core.Models;
using log4net;

namespace Grappachu.Movideo.Core.Components.MediaOrganizer
{
    public class FileOrganizer : IFileOrganizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileOrganizer));
        private string _renameTemplate;
        private readonly IFolderCleaner _cleaner;

        internal FileOrganizer(string destinationFolder, string renameTemplate, IFolderCleaner cleaner)
        {
            DestinationFolder = destinationFolder;
            _renameTemplate = renameTemplate;
            _cleaner = cleaner;
        }

        private string DestinationFolder { get; }

        private string RenameTemplate
        {
            get => _renameTemplate;
            set
            {
                if (!TemplateConverter.IsValid(value))
                    throw new ArgumentException("Rename Template is invalid");
                _renameTemplate = value;
            }
        }


        public static string DefaultTemplate => string.Format("{0}\\{1}\\({2}) {1}.{3}",
            Tokens.Collection, Tokens.Title, Tokens.Year, Tokens.Extension);

        public string Organize(FileInfo itemFile, Movie movie)
        {
            var fname = GetRenamedPath(itemFile, movie);
            var target = Path.Combine(DestinationFolder, fname);
            var targetPath = SafeAddSuffix(target);

            FilesystemTools.SafeCreateDirectory(Path.GetDirectoryName(targetPath));

            File.Move(itemFile.FullName, targetPath);
            Log.InfoFormat("Match Saved: {0} ==> {1}", itemFile.Name, targetPath);

            if (_cleaner != null)
            {
                _cleaner.Clean(itemFile.Directory);
            }

            return targetPath;

        }



        private string GetRenamedPath(FileInfo item, Movie movie)
        {
            var template = RenameTemplate;

            var frenamed = TemplateConverter.Convert(template, item, movie);

            return frenamed;
        }





        private static string SafeAddSuffix(string fullPath)
        {
            var count = 1;

            var fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            var extension = Path.GetExtension(fullPath);
            var path = Path.GetDirectoryName(fullPath) ?? string.Empty;
            var newFullPath = fullPath;

            var dupes = false;
            while (File.Exists(newFullPath))
            {
                dupes = true;
                var tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            if (dupes)
            {
                Log.WarnFormat("Seems path {0} contains some duplicated movies", path);
            }
            return newFullPath;
        }
    }
}