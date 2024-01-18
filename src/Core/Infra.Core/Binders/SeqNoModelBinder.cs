using System.ComponentModel;
using System.Reflection;
using Infra.Core.Binders.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infra.Core.Binders;

public class SeqNoModelBinder : BaseModelBinder
{
    public override async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelType = bindingContext.ModelMetadata.ModelType;
        var model = Activator.CreateInstance(modelType);

        var valueDictionary = await GetValueDictionaryAsync(bindingContext.ActionContext.HttpContext.Request.Body);

        foreach (var propInfo in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propName = GetJsonPropertyName(propInfo);

            valueDictionary.TryGetValue(propName, out var value);

            var propValue = !string.IsNullOrEmpty($"{value}") ? $"{value}" : null;

            if (propInfo.PropertyType == typeof(SeqNo))
            {
                propInfo.SetValue(model, new SeqNo(propValue), null);
            }
            else
            {
                if (string.IsNullOrEmpty(propValue)) continue;

                var converter = TypeDescriptor.GetConverter(propInfo.PropertyType);

                propInfo.SetValue(model, converter.ConvertFromString(propValue), null);
            }
        }

        bindingContext.Model = model;
        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

        await Task.CompletedTask;
    }
}
