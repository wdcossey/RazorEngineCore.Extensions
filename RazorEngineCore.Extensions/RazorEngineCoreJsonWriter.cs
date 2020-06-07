using System;
using System.Text.Encodings.Web;
using System.Text.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace RazorEngineCore
{
    /// <summary>
    /// Lightweight implementation of the IJsonHelper from ASP.Net Core.
    /// The purpose of this file is to allow use of @Json tags in RazorEngineCore.
    /// </summary>
    public class RazorEngineCoreJsonWriter
    {
        internal const int BufferSizeDefault = 16 * 1024;
        internal const int DefaultMaxDepth = 64;
        
        private JsonSerializerOptions GlobalSerializerOptions { get; } = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Default
        };
        
        private static IHtmlContent Serialize(object value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value, options);
            return new HtmlString(json);
        }

        private static int SafeMaxDepth(int value)
        {
            return (value <= 0) ? DefaultMaxDepth : value;
        }
        
        private static int SafeBufferSize(int value)
        {
            return (value <= 0) ? 1 : value;
        }
        
        public IHtmlContent Serialize(object value)
        {
            return Serialize(value, GlobalSerializerOptions);
        }
        
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public IHtmlContent Serialize(
            object value, 
            bool? allowTrailingCommas = null,
            int? defaultBufferSize = null,
            bool? ignoreNullValues = null,
            bool? ignoreReadOnlyProperties = null,
            int? maxDepth = null,
            bool? propertyNameCaseInsensitive = null,
            bool? writeIndented = null)
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Default,
                DefaultBufferSize = SafeBufferSize(defaultBufferSize ?? GlobalSerializerOptions.DefaultBufferSize),
                WriteIndented = writeIndented ?? GlobalSerializerOptions.WriteIndented,
                AllowTrailingCommas = allowTrailingCommas ?? GlobalSerializerOptions.AllowTrailingCommas,
                IgnoreNullValues = ignoreNullValues ?? GlobalSerializerOptions.IgnoreNullValues,
                MaxDepth =  SafeMaxDepth(maxDepth ?? GlobalSerializerOptions.MaxDepth),
                IgnoreReadOnlyProperties = ignoreReadOnlyProperties ?? GlobalSerializerOptions.IgnoreReadOnlyProperties,
                PropertyNameCaseInsensitive = propertyNameCaseInsensitive ?? GlobalSerializerOptions.PropertyNameCaseInsensitive,
            };
            
            return Serialize(value, serializerOptions);
        }

        public RazorEngineCoreJsonWriter WriteIndented(bool value)
        {
            GlobalSerializerOptions.WriteIndented = value;
            return this;
        }

        public RazorEngineCoreJsonWriter IgnoreNullValues(bool value)
        {
            GlobalSerializerOptions.IgnoreNullValues = value;
            return this;
        }

        public RazorEngineCoreJsonWriter MaxDepth(int value)
        {
            GlobalSerializerOptions.MaxDepth = SafeMaxDepth(value);
            return this;
        }

        public RazorEngineCoreJsonWriter AllowTrailingCommas(bool value)
        {
            GlobalSerializerOptions.AllowTrailingCommas = value;
            return this;
        }

        public RazorEngineCoreJsonWriter IgnoreReadOnlyProperties(bool value)
        {
            GlobalSerializerOptions.IgnoreReadOnlyProperties = value;
            return this;
        }
        
        public RazorEngineCoreJsonWriter PropertyNameCaseInsensitive(bool value)
        {
            GlobalSerializerOptions.PropertyNameCaseInsensitive = value;
            return this;
        }
        
        public RazorEngineCoreJsonWriter DefaultBufferSize(int value)
        {
            GlobalSerializerOptions.DefaultBufferSize = SafeBufferSize(value);
            return this;
        }
    }
}