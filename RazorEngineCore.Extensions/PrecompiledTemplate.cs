using System.IO;
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
        /// <param name="resourceName">Name of the template resource</param>
        /// <param name="fileName">Overrides the default file name of the resource file</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown if the resource cannot be found in the resource file</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the resource file cannot be found</exception>
        public static async Task<IRazorEngineCompiledTemplate> LoadAsync(string resourceName, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var entryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                fileName = Path.Combine(entryPath, "RazorEngineCore.templates");
            }

            var resourceReader = new ResourceReader(fileName);
            
            resourceReader.GetResourceData(resourceName: resourceName, out _, out var outData);

            using var memoryStream = new MemoryStream(outData, false);
            memoryStream.Seek(4, SeekOrigin.Begin);
            return await RazorEngineCompiledTemplate.LoadFromStreamAsync(memoryStream);
        }
        
        /// <summary>
        /// Loads a <see cref="IRazorEngineCompiledTemplate"/> from a resource.
        /// </summary>
        /// <param name="resourceName">Name of the template resource</param>
        /// <param name="fileName">Overrides the default file name of the resource file</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown if the resource cannot be found in the resource file</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the resource file cannot be found</exception>
        public static IRazorEngineCompiledTemplate Load(string resourceName, string fileName = null)
        {
            return LoadAsync(resourceName: resourceName, fileName: fileName)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}