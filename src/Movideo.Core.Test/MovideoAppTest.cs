using System.IO;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Interfaces;
using Moq;
using SharpTestsEx;
using Xunit;

namespace Grappachu.MoVideo.Test
{
    public class MovideoAppTest
    {
        public MovideoAppTest()
        {
            _fileScannerMock = new Mock<IFileScanner>();
            _dbMock = new Mock<IMovieDb>();
            _configMock = new Mock<IConfigReader>();
            _analyzerMock = new Mock<IFileAnalyzer>();

            _sut = new MovideoApp(_configMock.Object, _fileScannerMock.Object, _analyzerMock.Object, _dbMock.Object);
        }

        private readonly MovideoApp _sut;
        private readonly Mock<IFileScanner> _fileScannerMock;
        private readonly Mock<IMovieDb> _dbMock;
        private readonly Mock<IConfigReader> _configMock;
        private readonly Mock<IFileAnalyzer> _analyzerMock;

        [Fact]
        public void App_flow_Test()
        {
            _fileScannerMock.Setup(x => x.Scan()).Returns(new[]
            {
                new FileInfo("Z:\\_IMPORTED\\_Movies\\Il primo dei bugiardi (2009) [Mux by Little-Boy].mkv"),
                new FileInfo("Z:\\_IMPORTED\\_Movies\\The Raven (2012) BDRip 720p x265 Ita Eng Ac3 Sub ManoNera.mkv"),
                new FileInfo("Z:\\_IMPORTED\\_Movies\\Fuck You, Prof! (2013) BDRip 720p HEVC ITA GER AC3 Multi Sub PirateMKV.mkv")
            });


            var res =   _sut.ScanAsync(new MovideoSettings());
            res.Wait();

            res.Result.Should().Be(3);
        }
    }
}