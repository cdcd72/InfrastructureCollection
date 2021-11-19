using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infra.Core.FileAccess.Validators
{
    public class PathValidator
    {
        private const string NON_UNC_PATTERN = @"\\?\";

        private readonly string[] _rootPaths;
        private readonly string[] _violationPathPatterns =
            new string[] { $"..{Path.AltDirectorySeparatorChar}", $"..{Path.DirectorySeparatorChar}" };

        public PathValidator(params string[] rootPaths) => _rootPaths = rootPaths;

        public bool IsValidPath(string path)
        {
            if (_violationPathPatterns.Any(violationPathPattern => path.Contains(violationPathPattern)))
                return false;

            foreach (var rootPath in _rootPaths)
            {
                var rootSegements = GetSegements(new FileInfo(rootPath).FullName);
                var pathSegements = GetSegements(new FileInfo(path).FullName);

                if (rootSegements.Count() > pathSegements.Count())
                    return false;

                var verifySegements = pathSegements.Take(rootSegements.Count());

                if (!rootSegements.SequenceEqual(verifySegements))
                    return false;
            }

            return true;
        }

        public string GetValidPath(string path)
        {
            if (path.StartsWith(NON_UNC_PATTERN, StringComparison.Ordinal))
                path = path.Replace(NON_UNC_PATTERN, string.Empty);

            if (!IsValidPath(path))
                throw new IOException($"Try to access error path! {path}");

            if (!new Uri(path).IsUnc)
                path = NON_UNC_PATTERN + path;

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
