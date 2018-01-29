using System;
using System.IO;
using Grappachu.Movideo.Core.Utils;
using SharpTestsEx;
using Xunit;

namespace Grappachu.Movideo.Core.Test
{
    public class DirectoryInfoUtilsTest
    {
        private readonly string _testRoot;

        public DirectoryInfoUtilsTest()
        {
            _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRoot);
        }

        ~DirectoryInfoUtilsTest()
        {
            Directory.Delete(_testRoot, true);
        }

        [Fact]
        public void Should_check_if_dir_is_empty()
        {
            var dir1 = Path.Combine(_testRoot, "dir1");
            var sut = new DirectoryInfo(dir1);

            Executing.This(() => sut.IsEmpty()).Should().Throw<DirectoryNotFoundException>();

            Directory.CreateDirectory(dir1);
            sut.IsEmpty().Should().Be.True();

            var child1 = Path.Combine(dir1, "childDir");
            Directory.CreateDirectory(child1);
            sut.IsEmpty().Should().Be.False();

            var child2 = Path.Combine(dir1, "childFile");
            File.WriteAllText(child2, string.Empty);
            sut.IsEmpty().Should().Be.False();

            Directory.Delete(child1);
            sut.IsEmpty().Should().Be.False();

            File.SetAttributes(child2, FileAttributes.Hidden);
            sut.IsEmpty().Should().Be.False();

            File.Delete(child2);
            sut.IsEmpty().Should().Be.True();
        }
    }
}