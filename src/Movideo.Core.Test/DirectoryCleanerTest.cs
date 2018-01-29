using System;
using System.IO;
using Grappachu.Movideo.Core.Components.MediaOrganizer;
using SharpTestsEx;
using Xunit;

namespace Grappachu.Movideo.Core.Test
{
    public class DirectoryCleanerTest
    {
        private readonly string _testRoot;

        public DirectoryCleanerTest()
        {
            _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRoot);
        }

        ~DirectoryCleanerTest()
        {
            Directory.Delete(_testRoot, true);
        }

        [Fact]
        public void CleanUp_should_recusively_delete_all_empty_parents()
        {
            var dir1 = Path.Combine(_testRoot, "A");
            var dir2 = Path.Combine(dir1, "B");
            var dir3 = Path.Combine(dir2, "C");
            var dir4 = Path.Combine(dir3, "D");
            var dir5 = Path.Combine(dir4, "E");
            Directory.CreateDirectory(dir5);
            var childFile = Path.Combine(dir1, "aFile");
            File.WriteAllText(childFile, string.Empty);

            int deletes = DirectoryCleaner.CleanUp(dir5);

            Directory.Exists(dir1).Should().Be.True();
            Directory.Exists(dir2).Should().Be.False();
            deletes.Should().Be.EqualTo(4);
        }
    }
}