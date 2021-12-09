using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Infra.FileAccess.Physical.Common
{
    public class Env
    {
        private const string SectionName = "File";

        public string[] Roots { get; set; }

        #region Constructor

        public Env(IConfiguration config) => InitEnv(config);

        #endregion

        #region Private Method

        private void InitEnv(IConfiguration config)
        {
            Roots = config.GetValue<string>($"{SectionName}:{nameof(Roots)}").Split(",");

            if (Roots is null || (Roots != null && (Roots.Length is 0 || Roots.Any(path => path is null || path is ""))))
                throw new ArgumentNullException(nameof(Roots));
        }

        #endregion
    }
}
