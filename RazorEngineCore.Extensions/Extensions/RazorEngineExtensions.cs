using System;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace RazorEngineCore
{
    public static class RazorEngineExtensions
    {
        // ReSharper disable MemberCanBePrivate.Global
        
        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static IRazorEngineCompiledTemplate<T> CompileFromFile<T>(
            this RazorEngine razorEngine, string fileName,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : class, IRazorEngineTemplate
        {
            var content = GetFileContent(fileName);
            return razorEngine.Compile<T>(content: content, builderAction: builderAction);
        }

        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static async Task<IRazorEngineCompiledTemplate<T>> CompileFromFileAsync<T>(
            this RazorEngine razorEngine, 
            string fileName,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : class, IRazorEngineTemplate
        {
            var content = await GetFileContentAsync(fileName);
            return await razorEngine.CompileAsync<T>(content: content, builderAction: builderAction);
        }
        
        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static IRazorEngineCompiledTemplate<T> CompileFromStream<T>(
            this RazorEngine razorEngine, 
            StreamReader streamReader,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : class, IRazorEngineTemplate
        {
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            var content = streamReader.ReadToEnd();
            return razorEngine.Compile<T>(content: content, builderAction: builderAction);
        }

        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static async Task<IRazorEngineCompiledTemplate<T>> CompileFromStreamAsync<T>(
            this RazorEngine razorEngine, 
            StreamReader streamReader,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : class, IRazorEngineTemplate
        {
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            var content = await streamReader.ReadToEndAsync();
            return await razorEngine.CompileAsync<T>(content: content, builderAction: builderAction);
        }
        
        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static IRazorEngineCompiledTemplate CompileFromFile(
            this RazorEngine razorEngine, 
            string fileName, 
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            var content = GetFileContent(fileName);
            return razorEngine.Compile(content: content, builderAction: builderAction);
        }

        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static async Task<IRazorEngineCompiledTemplate> CompileFromFileAsync(
            this RazorEngine razorEngine, 
            string fileName, 
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            var content = await GetFileContentAsync(fileName);
            return await razorEngine.CompileAsync(content: content, builderAction: builderAction);
        }

        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static IRazorEngineCompiledTemplate CompileFromStream(
            this RazorEngine razorEngine, 
            StreamReader streamReader, 
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            return razorEngine.Compile(content: streamReader.ReadToEnd(), builderAction: builderAction);
        }
        
        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static async Task<IRazorEngineCompiledTemplate> CompileFromStreamAsync(
            this RazorEngine razorEngine, 
            StreamReader streamReader, 
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            return await razorEngine.CompileAsync(content: await streamReader.ReadToEndAsync(), builderAction: builderAction);
        }

        // ReSharper restore MemberCanBePrivate.Global
        
        #region Private Methods

        private static async Task<string> GetFileContentAsync(string fileName)
        {
            CheckFile(fileName: fileName);
            using var reader = new StreamReader(fileName);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
        
        private static string GetFileContent(string fileName)
        {
            CheckFile(fileName: fileName);
            using var reader = new StreamReader(fileName);
            return reader.ReadToEnd();
        }

        private static void CheckFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
        }

        #endregion
    }
}