using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public class RazorEngineCoreTemplateAttribute : Attribute
    {
        public string Name { get; }

        public Type TemplateType { get; }
        public string Content { get; }

        public string FileName { get; }
        public string OriginalFileName { get; }

        public bool Compiled { get; }
        
        public bool Embedded { get; }

        public bool IsContentTemplate { get; } = false;

        public RazorEngineCoreTemplateAttribute(
            string name,
            Type templateType, 
            string fileName,
            bool compiled, 
            bool embedded)
        {
            Name = name;
            TemplateType = templateType;
            FileName = fileName;
            Compiled = compiled;
            Embedded = embedded;
        }

        public RazorEngineCoreTemplateAttribute(
            string name,
            Type templateType, 
            string fileName,
            string originalFileName,
            bool compiled, 
            bool embedded)
        {
            Name = name;
            TemplateType = templateType;
            FileName = fileName;
            OriginalFileName = originalFileName;
            Compiled = compiled;
            Embedded = embedded;
        }

        public RazorEngineCoreTemplateAttribute(
            string name,
            Type templateType, 
            string content)
        {
            Name = name;
            TemplateType = templateType;
            Content = content;
            FileName = null;
            OriginalFileName = null;
            Compiled = false;
            Embedded = false;
            IsContentTemplate = true;
        }

        public RazorEngineCoreTemplateAttribute(
            string name,
            Type templateType, 
            string fileName,
            string content)
        {
            Name = name;
            TemplateType = templateType;
            Content = content;
            FileName = fileName;
            OriginalFileName = fileName;
            Compiled = false;
            Embedded = false;
            IsContentTemplate = true;
        }

        /*public async Task BuildAsync(Stream stream)
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<RazorEngineCorePageModel>(Content);
            await template.SaveToStreamAsync(stream);
        }*/
    }
}