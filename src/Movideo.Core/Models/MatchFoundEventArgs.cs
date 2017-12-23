using System;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;

namespace Grappachu.Movideo.Core.Models
{
    public class MatchFoundEventArgs : EventArgs
    {
        private readonly AnalyzedItem _item;
        private readonly Movie _res;
        private readonly float _accuracy;

        public MatchFoundEventArgs(AnalyzedItem item, Movie res, float accuracy)
        {
            _item = item;
            _res = res;
            _accuracy = accuracy;
        }

        public bool? IsMatch { get; set; }

        public AnalyzedItem LocalFile { get { return _item; } }

        public Movie Movie { get { return _res; } }
        public double MatchAccuracy { get { return _accuracy; } }
        public bool Cancel { get; set; }
        public int Progress { get; set; }
        public int Total { get; set; }
    }
}