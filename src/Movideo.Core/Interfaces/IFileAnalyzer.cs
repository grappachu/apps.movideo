using System.IO;
using Grappachu.Movideo.Core.Dtos;

namespace Grappachu.Movideo.Core
{
    public interface IFileAnalyzer
    {
        AnalyzedItem Analyze(FileInfo file);
    }
}