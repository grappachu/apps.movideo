using System;
using System.IO;
using Grappachu.Core.Lang.Extensions;
using Grappachu.Core.Lang.Text;
using Grappachu.Core.Security.Hashing;
using Grappachu.Movideo.Core.Components.TitleCleaner;
using Grappachu.Movideo.Core.Interfaces;
using log4net;

namespace Grappachu.Movideo.Core.Components.MediaAnalyzer
{
    public class FileAnalyzer : IFileAnalyzer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileAnalyzer));
        private readonly IMovieDb _db;

        public FileAnalyzer(IMovieDb db)
        {
            _db = db;

        }

        public AnalyzedItem Analyze(FileInfo file)
        {
            Log.DebugFormat("Analyzing: {0}", file.Name);

            var item = new AnalyzedItem(file);

            var fname = item.Path.FullName;
            var cleaned = MovieTitleCleaner.Clean(fname);
            item.Title = cleaned.Title;
            item.SubTitle = cleaned.SubTitle;
            item.Year = cleaned.Year;

            var mf = new MediaInfoDotNet.MediaFile(file.FullName);
            var duration = TimeSpan.FromMilliseconds(mf.General.Duration);
            item.Duration = duration;

            var fref = new FileRef(file);
            if (_db.HasHash(fref))
            {
                var hash = _db.GetHashFor(fref);
                Log.DebugFormat("Hash Found: {0}", hash);

                item.Hash = hash;
            }
            else
            {
                var hash = QuickHash(file);
                Log.DebugFormat("Hash Computed: {0}", hash);

                _db.Push(fref, hash);
                item.Hash = hash;
            }

            if (_db.HasMatch(item))
            {
                item.IsKnown = true;
                item.MovieId = _db.GetMovieIdFor(item);

                Log.DebugFormat("Match Found: {0} (MovieId)", item.MovieId);
            }

            return item;
        }

        private static string QuickHash(FileInfo finfo)
        {
            var bufferSize = Math.Min(1024 * 4096, finfo.Length); // 4Mb;
            var buffer = new byte[bufferSize];

            using (var fs = new FileStream(finfo.FullName, FileMode.Open, FileAccess.Read))
            {
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }

            var temp = MD5.HashBytes(buffer).ToString(BinaryRepresentation.Base64) + finfo.Length;
            var hash = MD5.HashString(temp).ToString(BinaryRepresentation.Hex);

            return hash;
        }
    }
}