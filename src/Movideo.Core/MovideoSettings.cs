namespace Grappachu.Movideo.Core
{
    public class MovideoSettings
    {
        public bool Reorganize { get; set; }
        public string TargetPath { get; set; }

        public string RenameTemplate { get; set; }
        public bool DeleteEmptyFolders { get; set; }
        public string SourcePath { get; set; }
    }
}