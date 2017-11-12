using System.IO;
using Grappachu.Movideo.Core.Dtos;

namespace Grappachu.Movideo.Core.Interfaces
{
    public interface IFileAnalyzer
    {
        AnalyzedItem Analyze(FileInfo file);
    }
}