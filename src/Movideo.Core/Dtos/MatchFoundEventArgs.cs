using System;
using Grappachu.Movideo.Core.Data.Model;

namespace Grappachu.Movideo.Core.Dtos
{
    public class MatchFoundEventArgs : EventArgs
    {
        private readonly AnalyzedItem _item;
        private readonly TmdbMovie _res;
        private readonly float _accuracy;

        public MatchFoundEventArgs(AnalyzedItem item, TmdbMovie res, float accuracy)
        {
            _item = item;
            _res = res;
            _accuracy = accuracy;
        }

        public bool? IsMatch { get; set; }

        public AnalyzedItem LocalFile { get { return _item; } }

        public TmdbMovie Movie { get { return _res; } }
        public double MatchAccuracy { get { return _accuracy; } }
    }
}