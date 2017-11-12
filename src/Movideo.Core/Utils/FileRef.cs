using System;
using System.IO;

namespace Grappachu.Movideo.Core.Utils
{
    public class FileRef
    {
        public FileRef()
        {
        }

        public FileRef(FileInfo finfo)
        {
            Path = finfo.FullName;
            LastModifiedUtc = finfo.LastWriteTimeUtc;
            Bytes = finfo.Length;
        }

        public string Path { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public long Bytes { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(FileRef other)
        {
            return string.Equals(Path, other.Path) && LastModifiedUtc.Equals(other.LastModifiedUtc) &&
                   Bytes == other.Bytes;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Path?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ LastModifiedUtc.GetHashCode();
                hashCode = (hashCode * 397) ^ Bytes.GetHashCode();
                return hashCode;
            }
        }
    }
}