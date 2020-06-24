using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public class PrecompiledTemplateAttribute : Attribute
    {
        public string Name { get; }
        
        public Type TemplateType { get; }
        
        public string Content { get; }

        public PrecompiledTemplateAttribute(string name, Type templateType, string content)
        {
            Name = name;
            TemplateType = templateType;
            Content = content;
        }

        public async Task BuildAsync(Stream stream)
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<RazorEngineCorePageModel>(Content);
            await template.SaveToStreamAsync(stream);
        }
    }
}