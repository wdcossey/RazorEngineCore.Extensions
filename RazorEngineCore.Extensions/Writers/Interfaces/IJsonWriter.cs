using System;
using System.Text.Json;

namespace RazorEngineCore.Writers.Interfaces
{
    /// <summary>
    /// Lightweight implementation of the IJsonHelper from ASP.Net Core.
    /// The purpose of this file is to allow use of @Json tags in RazorEngineCore.
    /// </summary>
    public interface IJsonWriter
    {
        // ReSharper disable MethodOverloadWithOptionalParameter
        
        IHtmlContent Serialize(object value);

        IHtmlContent Serialize(
            object value,
            bool? allowTrailingCommas = null,
            int? defaultBufferSize = null,
            bool? ignoreNullValues = null,
            bool? ignoreReadOnlyProperties = null,
            int? maxDepth = null,
            bool? propertyNameCaseInsensitive = null,
            bool? writeIndented = null);

        IJsonWriter WriteIndented(bool value);
        
        IJsonWriter IgnoreNullValues(bool value);
        
        IJsonWriter MaxDepth(int value);
        
        IJsonWriter AllowTrailingCommas(bool value);
        
        IJsonWriter IgnoreReadOnlyProperties(bool value);
        
        IJsonWriter PropertyNameCaseInsensitive(bool value);
        
        IJsonWriter DefaultBufferSize(int value);

        // ReSharper enable MethodOverloadWithOptionalParameter
    }
}