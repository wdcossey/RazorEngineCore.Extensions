using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace RazorEngineCore.Precompiler
{
    public static class Program
    {
        public static int Main(string[] args)
        {

            var name = $"{nameof(RazorEngineCore)}.{nameof(Precompiler)}";
            
            Console.WriteLine($"{name} :: Started");
            
            if (args?.Any() != true || !Directory.Exists(args[0]))
            {
                Console.WriteLine($"{name} :: Target directory invalid or not specified.");
                return -1;
            }

            var directoryName = args.FirstOrDefault();
            
            Console.WriteLine($"{name} :: Scanning {directoryName}");
            
            var fileNames = Directory
                .EnumerateFiles(directoryName)
                .Where(file => file.ToLowerInvariant().EndsWith("exe") || file.ToLowerInvariant().EndsWith("dll"));

            var resources = new ConcurrentDictionary<string, byte[]>();
            
            foreach (var fileName in fileNames)
            {

                Assembly assembly;
                
                try
                {
                    assembly = Assembly.LoadFile(fileName);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
                
                var attributes = assembly.GetCustomAttributes<RazorEngineCore.PrecompiledTemplateAttribute>()?.ToList();

                if (!attributes?.Any() == true)
                    continue;
                
                foreach (var attribute in attributes)
                {
                    try
                    {
                        Console.WriteLine($"{name} :: Precompiling \"{assembly.GetName().Name}.{attribute.Name}\" Template");
                
                        var engine = new RazorEngine();

                        var methodInfo = typeof(RazorEngine).GetMethod("CreateAndCompileToStream",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                
                        var compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            
                        compilationOptionsBuilder.AddAssemblyReference(attribute.TemplateType);
                        compilationOptionsBuilder.AddAssemblyReference(typeof(IRazorEngineTemplate));
                        compilationOptionsBuilder.Inherits(attribute.TemplateType);

                        var memoryStream = methodInfo?.Invoke(engine, new object[] {attribute.Content, compilationOptionsBuilder.Options}) as MemoryStream;

                        if (memoryStream == null) 
                            continue;
                        
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        resources.TryAdd($"{attribute.Name}", memoryStream.ToArray());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{name} :: Exception {e.Message}");
                        return -1;
                    }
                }
            }

            var resourceWriter = new ResourceWriter(Path.Combine(directoryName, $"{nameof(RazorEngineCore)}.templates"));
                        
            foreach (var resource in resources)
            {
                resourceWriter.AddResource(resource.Key, resource.Value);
            }
            
            resourceWriter.Close();
            
            Console.WriteLine($"{name} :: Completed");
            return 0;
        }
    }
}