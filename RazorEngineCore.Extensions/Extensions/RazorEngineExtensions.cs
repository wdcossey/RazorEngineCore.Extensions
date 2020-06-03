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
        public static RazorEngineCompiledTemplate<T> CompileFromFile<T>(
            this RazorEngine razorEngine, string fileName,
            Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : class, IRazorEngineTemplate
        {
            return razorEngine.CompileFromFileAsync<T>(fileName: fileName, builderAction: builderAction).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static Task<RazorEngineCompiledTemplate<T>> CompileFromFileAsync<T>(
            this RazorEngine razorEngine, 
            string fileName,
            Action<RazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : class, IRazorEngineTemplate
        {
            return razorEngine.CompileFromStreamAsync<T>(streamReader: GetFileStream(fileName), builderAction: builderAction);
        }
        
        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static RazorEngineCompiledTemplate<T> CompileFromStream<T>(
            this RazorEngine razorEngine, 
            StreamReader streamReader,
            Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : class, IRazorEngineTemplate
        {
            return razorEngine.CompileFromStreamAsync<T>(streamReader: streamReader, builderAction: builderAction).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static async Task<RazorEngineCompiledTemplate<T>> CompileFromStreamAsync<T>(
            this RazorEngine razorEngine, 
            StreamReader streamReader,
            Action<RazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : class, IRazorEngineTemplate
        {
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            return await razorEngine.CompileAsync<T>(content: await streamReader.ReadToEndAsync(), builderAction: builderAction);
        }
        
        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static RazorEngineCompiledTemplate CompileFromFile(this RazorEngine razorEngine, string fileName, Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return razorEngine.CompileFromFileAsync(fileName: fileName, builderAction: builderAction).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads the template content from a file using <see cref="StreamReader"/>
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="fileName"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static Task<RazorEngineCompiledTemplate> CompileFromFileAsync(this RazorEngine razorEngine, string fileName, Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return razorEngine.CompileFromStreamAsync(streamReader: GetFileStream(fileName), builderAction: builderAction);
        }

        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static RazorEngineCompiledTemplate CompileFromStream(this RazorEngine razorEngine, StreamReader streamReader, Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return razorEngine.CompileFromStreamAsync(streamReader: streamReader, builderAction: builderAction).GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Read from a <see cref="StreamReader"/>, this will force the <see cref="streamReader"/> Position back to 0
        /// </summary>
        /// <param name="razorEngine"></param>
        /// <param name="streamReader"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static async Task<RazorEngineCompiledTemplate> CompileFromStreamAsync(this RazorEngine razorEngine, StreamReader streamReader, Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            return await razorEngine.CompileAsync(content: await streamReader.ReadToEndAsync(), builderAction: builderAction);
        }
        
        // ReSharper restore MemberCanBePrivate.Global
        
        #region Private Methods

        private static StreamReader GetFileStream(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            return new StreamReader(fileName);
        }

        #endregion
    }
}