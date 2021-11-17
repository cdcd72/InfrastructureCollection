using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.FileAccess.Validators
{
    public class PathValidatorTests
    {
        private readonly string _validPath = @"C:\Test\";
        private readonly string _invalidPath = @"C:\Test\..\";

        [Test]
        public void IsValidPath() => Assert.IsTrue(PathValidator.IsValidPath(_validPath));

        [Test]
        public void IsInvalidPath() => Assert.IsFalse(PathValidator.IsValidPath(_invalidPath));

        [Test]
        public void GetPathWithValidPath() => PathShouldBe(_validPath, _validPath);

        [Test]
        public void GetPathWithInvalidPath() => PathShouldBe(_invalidPath, _validPath);

        #region Private Method

        private static void PathShouldBe(string path, string expected)
            => Assert.AreEqual(expected, PathValidator.GetValidPath(path));

        #endregion
    }
}
