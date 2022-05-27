using System.IO;
using System.Reflection;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.FileAccess.Validators
{
    public class PathValidatorTests
    {
        #region Properties

        private static string RootPath =>
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "TestData", "Files");

        #endregion

        [Test]
        public void IsValidPath()
            => PathIsValidShouldBe(RootPath, Path.Combine(RootPath, "1.jpg"), true);

        [Test]
        public void IsInvalidPathWithViolationPathPattern()
            => PathIsValidShouldBe(RootPath, Path.Combine(RootPath, "..", "1.jpg"), false);

        [Test]
        public void IsInvalidPathWithNotAccordingToRoot()
            => PathIsValidShouldBe(RootPath, Path.Combine("C:\\", "Test", "1.jpg"), false);

        [Test]
        public void GetNonUncValidPath()
        {
            var validNonUncPath = Path.Combine(RootPath, "1.jpg");

            PathShouldBe(RootPath, validNonUncPath, validNonUncPath);
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
            var validNonUncPath = Path.Combine(RootPath, "1.jpg");
            var invalidNonUncPath = Path.Combine(RootPath, "..", "1.jpg");

            Assert.Throws<IOException>(()
                => PathShouldBe(RootPath, invalidNonUncPath, validNonUncPath));
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
