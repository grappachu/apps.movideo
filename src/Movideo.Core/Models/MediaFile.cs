using System;

namespace Grappachu.Movideo.Core.Models
{
    public class MediaFile
    {
        public string Key { get; set; }

        public string Path { get; set; }

        public long Bytes { get; set; }

       public DateTime LastModifiedUtc { get; set; }

        public string Hash { get; set; }

        public MediaBinding Binding { get; set; }
    }
}