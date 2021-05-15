using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace RazorEngineCore.Content
{
    public class PartialContent : IHtmlContent
    {
        private readonly Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>> _templateTask;
        private readonly object _model;

        public PartialContent(Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>> templateTask, object model)
        {
            _templateTask = templateTask;
            _model = model;
        }
        
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            WriteToAsync(writer: writer, encoder: encoder).GetAwaiter().GetResult();
        }

        public async Task WriteToAsync(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));
            
            var template = await _templateTask;
            
            var output = await template.RunAsync(model: _model);
            
            await writer.WriteAsync(output);
        }
    }
}