using System;
using System.Globalization;
using System.Web;

// ReSharper disable MemberCanBePrivate.Global
namespace RazorEngineCore
{
    /// <summary>
    /// Lightweight implementation of the IHtmlHelper from ASP.Net Core.
    /// The purpose of this file is to allow use of some @Html tags in RazorEngineCore.
    /// i.e. @Html.Raw(), @Html.AttributeEncode(), @Html.Encode(), etc.
    /// </summary>
    public class RazorEngineCoreHtmlWriter
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