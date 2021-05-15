using System;
using System.Threading.Tasks;

namespace RazorEngineCore.Writers.Interfaces
{
    /// <summary>
    /// Lightweight implementation of the IHtmlHelper from ASP.Net Core.
    /// The purpose of this file is to allow use of some @Html tags in RazorEngineCore.
    /// i.e. @Html.Raw(), @Html.AttributeEncode(), @Html.Encode(), etc.
    /// </summary>
    public interface IHtmlWriter
    {
        string AttributeEncode(string value);
        
        string AttributeEncode(object value);
        
        IHtmlContent Raw(string value);
        
        IHtmlContent Raw(object value);
        
        string Encode(string value);
        
        string Encode(object value);

        public IHtmlContent Partial(
            string partialViewName,
            object model = null /*,
            ViewDataDictionary viewData*/);

        public Task<IHtmlContent> PartialAsync(
            string partialViewName,
            object model = null /*,
            ViewDataDictionary viewData*/);
    }
}