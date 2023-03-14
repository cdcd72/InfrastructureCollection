using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infra.Core.Binders;

public class BaseModelBinder : IModelBinder
{
    public virtual async Task BindModelAsync(ModelBindingContext bindingContext) => await Task.CompletedTask;

    protected static string GetJsonPropertyName(PropertyInfo propInfo)
    {
        var customAttributeData =
            propInfo.CustomAttributes.First(attr => attr.AttributeType == typeof(System.Text.Json.Serialization.JsonPropertyNameAttribute));

        var arguments = customAttributeData.ConstructorArguments;

        return arguments.Count > 0 ? $"{arguments[0].Value}" : null;
    }

    protected static async Task<Dictionary<string, object>> GetValueDictionaryAsync(Stream body)
    {
        string json;

        using (var reader = new StreamReader(body))
        {
            json = await reader.ReadToEndAsync();
        }

        return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    }
}
