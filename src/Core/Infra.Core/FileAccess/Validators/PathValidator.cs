namespace Infra.Core.FileAccess.Validators
{
    public class PathValidator
    {
        private readonly string[] _rootPaths;
        private readonly string[] _violationPathPatterns =
            new string[] { $"..{Path.AltDirectorySeparatorChar}", $"..{Path.DirectorySeparatorChar}" };

        public PathValidator(params string[] rootPaths) => _rootPaths = rootPaths;

        public bool IsValidPath(string path)
        {
            if (_violationPathPatterns.Any(violationPathPattern => path.Contains(violationPathPattern)))
                return false;

            // Path error count
            var errorCount = 0;

            foreach (var rootPath in _rootPaths)
            {
                var rootSegements = GetSegements(new FileInfo(rootPath).FullName);
                var pathSegements = GetSegements(new FileInfo(path).FullName);

                if (rootSegements.Count() > pathSegements.Count())
                {
                    errorCount++;
                    continue;
                }

                var verifySegements = pathSegements.Take(rootSegements.Count());

                if (!rootSegements.SequenceEqual(verifySegements))
                    errorCount++;
            }

            // Path error count less than root paths, mean at least one efficient path.
            return errorCount != _rootPaths.Length;
        }

        public string GetValidPath(string path)
        {
            if (!IsValidPath(path))
                throw new IOException($"Try to access error path! {path}");

            return path;
        }

        #region Private Method

        internal static IEnumerable<string> GetSegements(string path)
        {
            var fi = new FileInfo(path);

            if (fi.Directory == null)
                return new string[] { fi.FullName };

            // Is directory
            if (string.IsNullOrEmpty(fi.Name))
                return GetSegements(fi.Directory.FullName);

            return GetSegements(fi.Directory.FullName).Concat(new string[] { fi.Name });
        }

        #endregion
    }
}
