using System;
using System.Threading.Tasks;
using System.Web;
using RazorEngineCore.Content;
using RazorEngineCore.Writers.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
namespace RazorEngineCore.Writers
{
    /// <inheritdoc cref="IHtmlWriter"/>
    public class HtmlWriter : IHtmlWriter
    {
        private readonly IRazorEngineTemplate _parentTemplate;

        public HtmlWriter(IRazorEngineTemplate parentTemplate)
        {
            _parentTemplate = parentTemplate;
        }
        
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

        public IHtmlContent Partial(string partialViewName, object model = null)
        {
            return PartialAsync(partialViewName: partialViewName, model: model)
                .GetAwaiter()
                .GetResult();
        }

        #region Partial
        
        public Task<IHtmlContent> PartialAsync(
            string partialViewName,
            object model = null/*,
            ViewDataDictionary viewData*/)
        {
            if (string.IsNullOrWhiteSpace(partialViewName))
                throw new ArgumentNullException(nameof(partialViewName));

            return RenderPartialCoreAsync(partialViewName, model);
        }

        protected virtual Task<IHtmlContent> RenderPartialCoreAsync(
            string partialViewName,
            object model = null/*,
            ViewDataDictionary viewData,
            TextWriter writer*/)
        {
            model ??= _parentTemplate?.Model;
            if (ViewsManager.TryGetValue(partialViewName, out var lazyTemplateTask))
            {
                return Task.FromResult<IHtmlContent>(new PartialContent(lazyTemplateTask?.Value, model));
            }
            
            return Task.FromResult<IHtmlContent>(new HtmlString(""));
        }
        
        #endregion
    }

   
}