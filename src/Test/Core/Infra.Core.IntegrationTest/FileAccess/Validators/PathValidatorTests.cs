using System.IO;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.FileAccess.Validators
{
    public class PathValidatorTests
    {
        private const string NON_UNC_PATTERN = @"\\?\";

        private readonly string _folderPath;

        public PathValidatorTests() =>
            _folderPath = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TestData\Files");

        [Test]
        public void IsValidPath()
            => PathIsValidShouldBe(_folderPath, Path.Combine(_folderPath, "1.jpg"), true);

        [Test]
        public void IsInvalidPathWithViolationPathPattern()
            => PathIsValidShouldBe(_folderPath, Path.Combine(_folderPath, "..", "1.jpg"), false);

        [Test]
        public void IsInvalidPathWithNotAccordingToRoot()
            => PathIsValidShouldBe(_folderPath, Path.Combine("C:\\", "Test", "1.jpg"), false);

        [Test]
        public void GetNonUncValidPath()
        {
            var validNonUncPath = Path.Combine(_folderPath, "1.jpg");

            PathShouldBe(_folderPath, validNonUncPath, NON_UNC_PATTERN + validNonUncPath);
        }

        [Test]
        public void GetUncValidPath()
        {
            var rootPath = Path.Combine(@"\\Server", "Test");
            var validUncPath = Path.Combine(@"\\Server", "Test", "1.jpg");

            PathShouldBe(rootPath, validUncPath, validUncPath);
        }

        [Test]
        public void GetValidPathFail()
        {
            var validNonUncPath = Path.Combine(_folderPath, "1.jpg");
            var invalidNonUncPath = Path.Combine(_folderPath, "..", "1.jpg");

            Assert.Throws<IOException>(()
                => PathShouldBe(_folderPath, invalidNonUncPath, NON_UNC_PATTERN + validNonUncPath));
        }

        #region Private Method

        private static void PathIsValidShouldBe(string rootPath, string targetPath, bool expected)
        {
            var pathValidator = new PathValidator(rootPath);

            Assert.AreEqual(expected, pathValidator.IsValidPath(targetPath));
        }

        private static void PathShouldBe(string rootPath, string targetPath, string expected)
        {
            var pathValidator = new PathValidator(rootPath);

            Assert.AreEqual(expected, pathValidator.GetValidPath(targetPath));
        }

        #endregion
    }
}
