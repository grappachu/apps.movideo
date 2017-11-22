using System.Globalization;
using System.IO;
using Grappachu.Movideo.Core.Dtos;
using Grappachu.Movideo.Core.Interfaces;
using Moq;
using SharpTestsEx;
using Xunit;

namespace Grappachu.Movideo.Core.Test
{
    public class MovideoAppTest
    {
        public MovideoAppTest()
        {
            _fileScannerMock = new Mock<IFileScanner>();
            _dbMock = new Mock<IMovieDb>();
            _configMock = new Mock<IConfigReader>();
            _configMock.Setup(x => x.GetApiSettings()).Returns(new ApiSettings
            {
                ApiCulture = new CultureInfo("it-IT"),
                ApiKey = "1234567890"
            });


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
            _analyzerMock.Setup(x => x.Analyze(It.IsAny<FileInfo>())).Returns(new AnalyzedItem(new FileInfo(".")));


            var res =   _sut.ScanAsync(new MovideoSettings());
            res.Wait();

            res.Result.Should().Be(3);
        }
    }
}