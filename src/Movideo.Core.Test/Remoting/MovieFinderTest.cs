using System;
using System.Globalization;
using System.IO;
using Grappachu.Movideo.Core.Components.MediaAnalyzer;
using Grappachu.Movideo.Core.Components.Movies;
using Grappachu.Movideo.Core.Components.Remoting;
using Grappachu.Movideo.Core.Components.Remoting.Interfaces;
using Grappachu.Movideo.Core.Interfaces;
using Grappachu.Movideo.Data.Remoting;
using Moq;
using SharpTestsEx;
using TMDbLib.Objects.Movies;
using Xunit;

namespace Grappachu.Movideo.Core.Test.Remoting
{
    public class MovieFinderTest
    {
        IMovieFinder _sut;
        private readonly Mock<ITmdbClientFactory> _clientFactoryMock;
        private Mock<IConfigReader> _configReaderMock;

        public MovieFinderTest()
        {
            _clientFactoryMock = new Mock<ITmdbClientFactory>();
            _configReaderMock = new Mock<IConfigReader>();
            _configReaderMock.Setup(x => x.GetApiSettings()).Returns(new ApiSettings()
            {
                ApiCulture = new CultureInfo("it-IT"),
                ApiKey = "8eb2eaed41168191a9c48786b6b2c3ff"
            });
            var factory = new TmdbClientFactory(_configReaderMock.Object);

            _sut = new MovieFinder(factory);

            
        }

        [Fact]
        public void Test_matrix()
        {
            var item1 = new AnalyzedItem(new FileInfo("."))
            {
                Title = "The Matrix Reloaded",
                SubTitle = "",
                Duration = TimeSpan.FromMinutes(138),
                Year = 2003
            };

            var res1 = _sut.FindMatch(item1);

            res1.Movie.OriginalTitle.Should().Be("The Matrix Reloaded");
            res1.MatchAccuracy.Should().Be.EqualTo(1);
        }

        [Fact]
        public void Perfect_match_should_get_1()
        {
            var movie = new Movie
            {
                OriginalTitle = "The Matrix Reloaded",
                Title = "Matrix Reloaded", 
                Runtime = 138,
                Popularity = 21.950722,
                ReleaseDate = new DateTime(2003, 3, 25)
            };

            var item =  new AnalyzedItem(null)
            {
                Title = "The Matrix Reloaded",
                SubTitle = "",
                Duration = TimeSpan.FromMinutes(138),
                Year = 2003 
            };

            var res = MovieFinder.GetMatch(movie, item, 21.950722);

            res.Should().Be.IncludedIn(0.999, 1);
        }

        [Fact]
        public void No_match_should_get_0()
        {
            var movie = new Movie
            {
                OriginalTitle = "Fantozzi",
                Title = "Fantozzi", 
                Runtime = 92,
                Popularity = 11.950722,
                ReleaseDate = new DateTime(1972, 3, 25)
            };

            var item =  new AnalyzedItem(null)
            {
                Title = "The Matrix Reloaded",
                SubTitle = "",
                Duration = TimeSpan.FromMinutes(138),
                Year = 2003
            };

            var res = MovieFinder.GetMatch(movie, item, 11.950722);

            res.Should().Be.IncludedIn(0.0, 0.1);
        }

    }


}
