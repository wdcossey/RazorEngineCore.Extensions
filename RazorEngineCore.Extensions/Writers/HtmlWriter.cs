using System.Web;
using RazorEngineCore.Writers.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
namespace RazorEngineCore.Writers
{
    /// <inheritdoc cref="IHtmlWriter"/>
    public class HtmlWriter : IHtmlWriter
    { 
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public string AttributeEncode(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.HtmlAttributeEncode(value);
        }
        
        public string AttributeEncode(object value)
        {
            return AttributeEncode(value?.ToString());
        }
        
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public IHtmlContent Raw(string value)
        {
            return new HtmlString(value);
        }

        public IHtmlContent Raw(object value)
        {
            return Raw(value?.ToString());
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public string Encode(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.HtmlEncode(value);
        }
        
        public string Encode(object value)
        {
            return Encode(value?.ToString());
        }
    }

   
}