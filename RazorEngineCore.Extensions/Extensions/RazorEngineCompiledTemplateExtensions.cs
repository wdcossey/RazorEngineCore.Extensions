using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace RazorEngineCore
{
    public static class RazorEngineCompiledTemplateExtensions
    {
        private const string TemplateType = "templateType";

        private static Type GetReflectedTemplateType(this object @object)
        {
            // 'templateType' is private, need to get the value via refection.
            var fieldInfo = @object
                .GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(w => w.Name.Equals(TemplateType, StringComparison.InvariantCultureIgnoreCase));

            var templateType = fieldInfo?.GetValue(@object) as Type;
            
            if (fieldInfo == null || templateType == null)
            {
                throw new InvalidOperationException($"Could not find {TemplateType} value for {nameof(RazorEngineCompiledTemplate)}");
            }

            return templateType;
        }

        private static Type GetReflectedTemplateType<TTemplate>(
            this RazorEngineCompiledTemplate<TTemplate> compiledTemplate)
            where TTemplate : IRazorEngineTemplate
        {
            return ((object)compiledTemplate).GetReflectedTemplateType();
        }
        
        // ReSharper disable MemberCanBePrivate.Global
        
        public static string Run<TTemplate>(
            this RazorEngineCompiledTemplate<TTemplate> compiledTemplate, 
            object model = null)
            where TTemplate : class, IRazorEngineTemplate
        {
            return compiledTemplate.RunAsync<TTemplate>(model).GetAwaiter().GetResult();
        }
        
        public static async Task<string> RunAsync<TTemplate>(
            this RazorEngineCompiledTemplate<TTemplate> compiledTemplate, 
            object model = null)
            where TTemplate : class, IRazorEngineTemplate
        {
            if (model?.IsAnonymous() == true)
            {
                model = new AnonymousTypeWrapper(model);
            }
            
            var templateType = compiledTemplate.GetReflectedTemplateType();

            IRazorEngineTemplate instance = (IRazorEngineTemplate)Activator.CreateInstance(templateType);
            instance.Model = model;
            await instance.ExecuteAsync();
            return instance.Result();
        }
        
        public static string Run<TTemplate, TModel>(
            this RazorEngineCompiledTemplate<TTemplate> compiledTemplate,
            TModel model = default)
            where TModel : class
            where TTemplate : class, IRazorEngineTemplate
        {
            return compiledTemplate.RunAsync<TTemplate, TModel>(model).GetAwaiter().GetResult();
        }

        public static async Task<string> RunAsync<TTemplate, TModel>(
            this RazorEngineCompiledTemplate<TTemplate> compiledTemplate, 
            TModel model = default)
            where TModel : class
            where TTemplate : class, IRazorEngineTemplate
        {

            if (compiledTemplate == null)
            {
                throw new ArgumentNullException(nameof(compiledTemplate));
            }

            if (model?.IsAnonymous() == true)
            {
                return await compiledTemplate.RunAsync(model:(object)model);
            }
            
            var templateType = compiledTemplate.GetReflectedTemplateType();
            
            IRazorEngineTemplate instance = (IRazorEngineTemplate)Activator.CreateInstance(templateType);
            
            // Find the correct property to update via reflection.
            // As IRazorEngineTemplateBase<T> inherits from IRazorEngineTemplateBase and the both have `Model`
            var propertyInfo = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                .FirstOrDefault(fd => fd.Name.Equals(nameof(IRazorEngineTemplate.Model)) && fd.PropertyType == typeof(TModel));

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(instance, model);
            }
            else
            {
                instance.Model = model;
            }
            
            await instance.ExecuteAsync();

            return instance.Result();
        }
        
        // ReSharper restore MemberCanBePrivate.Global
    }
}