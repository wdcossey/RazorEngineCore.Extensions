using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using RazorEngineCore;

namespace RazorEngineCoreCompiler.MSBuild
{
    public class RazorEngineCoreCompiler : Task
    {
        private const string ProductName = "RazorEngineCore.Compiler";
        
        [Required]
        public string BuildDirectory { get; set; }
        
        [Required]
        public string TargetFramework { get; set; }
        
        [Required]
        public string ProjectDirectory { get; set; }
        
        [Required]
        public string ViewsDirectory { get; set; }

        //[Required]
        public string OutputDirectory { get; set; }

        //[Required]
        //public string OutputFile { get; set; }

        public bool EmbedResources { get; set; } = true;
        
        public bool Precompile { get; set; } = true;

        public override bool Execute()
        {
            try
            {
                Log.LogMessage("{0} :: v.{1}", ProductName, Assembly.GetExecutingAssembly().GetName().Version.ToString());
 
                Log.LogMessage("{0} :: Started", ProductName);

                //if (args?.Any() != true || !Directory.Exists(args[0]))
                //{
                //    Console.WriteLine($"{ProductName} :: Target directory invalid or not specified.");
                //    return false;
                //}

                var templateDirectory = Path.Combine(ProjectDirectory, ViewsDirectory);

                Log.LogMessage("{0} :: Scanning {1}", ProductName, templateDirectory);

                var fileNames = Directory
                    .EnumerateFiles(templateDirectory, "*.cshtml", SearchOption.AllDirectories);

                var resources = new ConcurrentDictionary<string, byte[]>();

                foreach (var fileName in fileNames)
                {
                    var relativePath = Path.GetRelativePath(ProjectDirectory, fileName);
                    
                    if (Precompile)
                    {
                        Log.LogMessage("{0} :: Pre-compiling \"{1}\"", ProductName, fileName);

                        var engine = new RazorEngine();

                        var methodInfo = typeof(RazorEngine).GetMethod("CreateAndCompileToStream",
                            BindingFlags.NonPublic | BindingFlags.Instance);

                        var compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();

                        compilationOptionsBuilder.AddAssemblyReference(typeof(RazorEngineCorePageModel));
                        compilationOptionsBuilder.AddAssemblyReference(typeof(IRazorEngineTemplate));
                        compilationOptionsBuilder.Inherits(typeof(RazorEngineCorePageModel));

                        using var reader = new StreamReader(fileName);
                        var fileContent = reader.ReadToEnd();
                        reader.Close();

                        var memoryStream = methodInfo?.Invoke(engine,
                            new object[] {fileContent, compilationOptionsBuilder.Options}) as MemoryStream;

                        if (memoryStream == null)
                            continue;
                        
                        Log.LogMessage("{0} :: Relative Path \"{1}\"", ProductName, relativePath);

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        
                        resources.TryAdd(relativePath, memoryStream.ToArray());
                    }
                    else
                    {
                        resources.TryAdd(relativePath, null);
                    }
                }

                /*var templateDictionary = new Dictionary<string, object>();*/
                var attributeList = new List<string>();
                
                var outputDirectory = Path.Combine(ProjectDirectory, BuildDirectory, TargetFramework);
                
                Directory.CreateDirectory(outputDirectory);
                
                foreach (var (relativePath, value) in resources)
                {
                    var originalFileName = relativePath;
                    var compiledFileName = Path.ChangeExtension(relativePath, ".rzhtml");
                    
                    string outputFileName = $"{(Precompile ? compiledFileName : originalFileName)}";

                    if (Precompile && value != null)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(outputDirectory, outputFileName)));
                        
                        //TODO: fix this 
                        //using (var writer = System.IO.File.Create(Path.Combine(templateDirectory, outputFileName)))
                        using var writer = System.IO.File.Create(Path.Combine(outputDirectory, outputFileName));
                        Log.LogMessage("{0} :: Writing File: \"{1}\", Size: {2}", ProductName, outputFileName, value.Length);
                        writer.Write(value);
                        writer.Flush();
                    }

                    //Log.LogMessage("{0} :: Template Path \"{1}\"", ProductName, resourceUri);

                    var templateFileName = EmbedResources
                        ? outputFileName.Replace(Path.DirectorySeparatorChar, '_')
                        : outputFileName;
                    
                    var templateUri = new UriBuilder("razor", "", 0, Path.ChangeExtension(outputFileName, null));

                    attributeList.Add(@$"[assembly: {typeof(RazorEngineCoreTemplateAttribute).FullName}(name: ""{templateUri.Path}"", templateType: typeof({typeof(RazorEngineCorePageModel).FullName}), fileName: @""{templateFileName}"", originalFileName: @""{originalFileName}"", compiled: {Precompile.ToString().ToLowerInvariant()}, embedded: {EmbedResources.ToString().ToLowerInvariant()})]");
                    
                    /*templateDictionary.Add(templateKeyName, new
                    {
                        file_name = (EmbedResources ? outputFileName.Replace(Path.DirectorySeparatorChar, '_') : outputFileName),
                        file_paths = resourcePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries),
                        compiled = Precompile,
                        embedded = EmbedResources
                    });*/
                }

                /*var jsonContent = System.Text.Json.JsonSerializer.Serialize(templateDictionary, new System.Text.Json.JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                using (var writer = System.IO.File.CreateText(Path.Combine(outputDirectory, "RazorEngineCoreTemplates.json")))
                {
                    writer.Write(jsonContent);
                    writer.Flush();
                }*/

                using (var writer = System.IO.File.CreateText(Path.Combine(ProjectDirectory, "RazorEngineCoreTemplates.cs")))
                {
                    writer.WriteLine($"using {nameof(RazorEngineCore)};");
                    
                    foreach (var item in attributeList)
                        writer.WriteLine(item);

                    writer.Flush();
                }

                Log.LogMessage("{0} :: Completed", ProductName);
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e.Demystify(), false);
                return false;
            }
        }
    }
}