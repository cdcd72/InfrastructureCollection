using System.Text;
using Microsoft.Extensions.Configuration;

namespace Infra.Core.Extensions;

public static class ConfigurationBuilderExtension
{
    public static IConfigurationBuilder AddObject(this IConfigurationBuilder builder, object obj)
    {
        var bytes = Encoding.UTF8.GetBytes(obj.ToJson());

        return builder.AddJsonStream(bytes.ToStream());
    }
}
