using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public static class PrecompiledTemplate
    {
        /// <summary>
        /// Loads a <see cref="IRazorEngineCompiledTemplate"/> from a resource.
        /// </summary>
        /// <param name="name">Name of the template resource</param>
        /// <param name="resourceAssembly">Specify the assembly containing the resource file</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown if the resource cannot be found in the resource file</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the resource file cannot be found</exception>
        public static async Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>> LoadAsync(string name, Assembly resourceAssembly = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            resourceAssembly ??= Assembly.GetExecutingAssembly();

            var attributes = resourceAssembly.GetCustomAttributes<RazorEngineCoreTemplateAttribute>();

            if (!ViewsManager.IsAssemblyRegistered(resourceAssembly))
            {
                foreach (var attribute in attributes)
                {
                    if (await LoadFromLocalFileAsync(attribute))
                        continue;

                    if (!attribute.IsContentTemplate && attribute.Compiled)
                    {
                        if (attribute.Embedded)
                        {
                            var manifestResourceStream =
                                resourceAssembly.GetManifestResourceStream(
                                    $"{nameof(RazorEngineCore)}.{attribute.FileName}");
                            var compiledTemplate =
                                await RazorEngineCompiledTemplate<RazorEngineCorePageModel>.LoadFromStreamAsync(
                                    manifestResourceStream);
                            ViewsManager.TryAdd(attribute.Name, compiledTemplate);
                        }
                        else
                        {
                            using var fileStream = LoadFromFileStream(attribute);
                            var compiledTemplate =
                                await RazorEngineCompiledTemplate<RazorEngineCorePageModel>.LoadFromStreamAsync(fileStream);
                            ViewsManager.TryAdd(attribute.Name, compiledTemplate);
                        }
                    }
                    else if (!attribute.IsContentTemplate && !attribute.Compiled)
                    {
                        if (attribute.Embedded)
                        {
                            var manifestResourceStream =
                                resourceAssembly.GetManifestResourceStream(
                                    $"{nameof(RazorEngineCore)}.{attribute.FileName}");
                            using var streamReader =
                                new StreamReader(manifestResourceStream ?? throw new InvalidOperationException());
                            ViewsManager.TryAdd(attribute.Name, await streamReader.ReadToEndAsync());
                        }
                        else
                        {
                            using var fileStream = LoadFromFileStream(attribute);
                            using var streamReader = new StreamReader(fileStream);
                            ViewsManager.TryAdd(attribute.Name, await streamReader.ReadToEndAsync());
                        }
                    }
                    else if (attribute.IsContentTemplate && !attribute.Compiled && !attribute.Embedded)
                    {
                        ViewsManager.TryAdd(attribute.Name, attribute.Content);
                    }
                }

                ViewsManager.RegisterAssembly(resourceAssembly);
            }

            if (ViewsManager.TryGetValue(name, out var template))
            {
                return await template.Value;
            }

            return null;
        }
        
        /// <summary>
        /// Loads a <see cref="IRazorEngineCompiledTemplate"/> from a resource.
        /// </summary>
        /// <param name="name">Name of the template resource</param>
        /// <param name="resourceAssembly">Specify the assembly containing the resource file</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown if the resource cannot be found in the resource file</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the resource file cannot be found</exception>
        public static IRazorEngineCompiledTemplate<RazorEngineCorePageModel> Load(string name, Assembly resourceAssembly = null)
        {
            return LoadAsync(name: name, resourceAssembly: resourceAssembly)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task<bool> LoadFromLocalFileAsync(RazorEngineCoreTemplateAttribute attribute)
        {
            if (!attribute.Compiled ||
                string.IsNullOrWhiteSpace(attribute.OriginalFileName) ||
                !File.Exists(attribute.OriginalFileName))
                return false;
            
            using var fileStream = new FileStream(
                path: attribute.OriginalFileName,
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.None,
                bufferSize: 4096,
                useAsync: true);
            
            using var streamReader = new StreamReader(fileStream);
            ViewsManager.TryAdd(attribute.Name, await streamReader.ReadToEndAsync());
            return true;
        }

        private static FileStream LoadFromFileStream(RazorEngineCoreTemplateAttribute attribute)
        {
            return new FileStream(
                path: attribute.FileName,
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.None,
                bufferSize: 4096,
                useAsync: true);
        }
    }
}