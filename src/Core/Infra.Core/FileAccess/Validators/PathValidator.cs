namespace Infra.Core.FileAccess.Validators
{
    public class PathValidator
    {
        private readonly string[] rootPaths;
        private readonly string[] violationPathPatterns = { $"..{Path.AltDirectorySeparatorChar}", $"..{Path.DirectorySeparatorChar}" };

        public PathValidator(params string[] rootPaths) => this.rootPaths = rootPaths;

        public bool IsValidPath(string path)
        {
            if (violationPathPatterns.Any(path.Contains))
                return false;

            // Path error count
            var errorCount = 0;

            foreach (var rootPath in rootPaths)
            {
                var rootSegments = GetSegments(new FileInfo(rootPath).FullName).ToArray();
                var pathSegments = GetSegments(new FileInfo(path).FullName).ToArray();

                if (rootSegments.Length > pathSegments.Length)
                {
                    errorCount++;
                    continue;
                }

                var verifySegments = pathSegments.Take(rootSegments.Length);

                if (!rootSegments.SequenceEqual(verifySegments))
                    errorCount++;
            }

            // Path error count less than root paths, mean at least one efficient path.
            return errorCount != rootPaths.Length;
        }

        public string GetValidPath(string path)
        {
            if (!IsValidPath(path))
                throw new IOException($"Try to access error path! {path}");

            return path;
        }

        #region Private Method

        private static IEnumerable<string> GetSegments(string path)
        {
            var fi = new FileInfo(path);

            if (fi.Directory == null)
                return new[] { fi.FullName };

            // Is directory
            return string.IsNullOrEmpty(fi.Name)
                ? GetSegments(fi.Directory.FullName)
                : GetSegments(fi.Directory.FullName).Concat(new[] { fi.Name });
        }

        #endregion
    }
}
