using System.Text.Encodings.Web;
using System.Text.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace RazorEngineCore
{
    /// <summary>
    /// Lightweight implementation of the IJsonHelper from ASP.Net Core.
    /// The purpose of this file is to allow use of some @Json. tags in RazorEngineCore.
    /// </summary>
    public class RazorEngineCoreJsonWriter
    {
        private JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Default
        };
        
        public IHtmlContent Serialize(object value)
        {
            var json = JsonSerializer.Serialize(value, SerializerOptions);
            return new HtmlString(json);
        }

        public RazorEngineCoreJsonWriter WriteIndented(bool value)
        {
            SerializerOptions.WriteIndented = value;
            return this;
        }

        public RazorEngineCoreJsonWriter IgnoreNullValues(bool value)
        {
            SerializerOptions.IgnoreNullValues = value;
            return this;
        }

        public RazorEngineCoreJsonWriter MaxDepth(int value)
        {
            SerializerOptions.MaxDepth = value;
            return this;
        }

        public RazorEngineCoreJsonWriter AllowTrailingCommas(bool value)
        {
            SerializerOptions.AllowTrailingCommas = value;
            return this;
        }

        public RazorEngineCoreJsonWriter IgnoreReadOnlyProperties(bool value)
        {
            SerializerOptions.IgnoreReadOnlyProperties = value;
            return this;
        }
    }
}