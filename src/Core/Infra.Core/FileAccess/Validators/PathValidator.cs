using System.IO;

namespace Infra.Core.FileAccess.Validators
{
    public static class PathValidator
    {
        private static readonly string[] violationPathPatterns =
            new string[] { $"..{Path.AltDirectorySeparatorChar}", $"..{Path.DirectorySeparatorChar}" };

        public static bool IsValidPath(string path)
        {
            foreach (var violationPathPattern in violationPathPatterns)
            {
                if (path.Contains(violationPathPattern))
                    return false;
            }

            return true;
        }

        public static string GetValidPath(string path)
        {
            foreach (var violationPathPattern in violationPathPatterns)
            {
                path = path.Replace(violationPathPattern, string.Empty);
            }

            return path;
        }
    }
}
