using System.IO;

namespace Grappachu.Movideo.Core.Components.MediaAnalyzer
{
    public interface IFileAnalyzer
    {
        AnalyzedItem Analyze(FileInfo file);
    }
}