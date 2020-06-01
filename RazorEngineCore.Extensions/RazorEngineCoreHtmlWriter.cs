using System;
using System.Globalization;
using System.Web;
using Microsoft.AspNetCore.Html;

// ReSharper disable MemberCanBePrivate.Global
namespace RazorEngineCore
{
    /// <summary>
    /// Lightweight implementation of the IHtmlHelper from ASP.Net Core.
    /// The purpose of this file is to allow use of some @Html. tags in RazorEngineCore.
    /// i.e. @Html.Raw(), @Html.AttributeEncode(), @Html.Encode(), etc.
    /// </summary>
    public class RazorEngineCoreHtmlWriter
    { 
        public string AttributeEncode(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.HtmlAttributeEncode(value);
        }
        
        public string AttributeEncode(object value)
        {
            return this.AttributeEncode(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
        
        public IHtmlContent Raw(string value)
        {
            return new HtmlString(value);
        }

        public IHtmlContent Raw(object value)
        {
            return new HtmlString(value?.ToString());
        }

        public string Encode(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.HtmlEncode(value);
        }
        
        public string Encode(object value)
        {
            return value == null ? string.Empty : HttpUtility.HtmlEncode(value);
        }
    }

   
}