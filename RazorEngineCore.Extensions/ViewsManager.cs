using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public static class ViewsManager
    {
        private static ConcurrentDictionary<ViewKey, Lazy<Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>>>>
            Partials { get; } =
            new ConcurrentDictionary<ViewKey, Lazy<Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>>>>();

        private static HashSet<Assembly> RegisteredAssemblies = new HashSet<Assembly>();
        
        public static bool TryAdd(string key, string value)
        {
            var lazyEngineTask = new Lazy<Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>>>(() =>
            {
                var razorEngine = new RazorEngine();
                return razorEngine.CompileAsync<RazorEngineCorePageModel>(value);
            });

            return Partials.TryAdd(key: new ViewKey(key), value: lazyEngineTask);
        }
        
        public static bool TryAdd(string key, IRazorEngineCompiledTemplate<RazorEngineCorePageModel> value)
        {
            var lazyEngineTask = new Lazy<Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>>>(() => Task.FromResult(value));
            return Partials.TryAdd(key: new ViewKey(key), value: lazyEngineTask);
        }
        
        internal static void RegisterAssembly(Assembly assembly)
        {
            RegisteredAssemblies.Add(assembly);
        }
        
        internal static bool IsAssemblyRegistered(Assembly assembly)
        {
            return RegisteredAssemblies.Contains(assembly);
        }

        /*public static bool ContainsKey(string key)
        {
            return Partials.ContainsKey(key: key);
        }
        
        public static bool TryRemove(string key, out string value)
        {
            return Partials.TryRemove(key: key, value: out value);
        }
        
        public static bool TryRemove(string key, string newValue, string comparisonValue)
        {
            return Partials.TryUpdate(key: key, newValue: newValue, comparisonValue: comparisonValue);
        }*/
        
        public static bool TryGetValue(string key, out Lazy<Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>>> value)
        {
            
            var partials = Partials.Where(w => 
                w.Key.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) || 
                w.Key.Segments.Last().Equals(key) ||
                string.Join("/", w.Key.Segments.Skip(1)).Equals(key)).ToList();

            if (partials.Any())
            {
                if (partials.Count == 1)
                {
                    value = partials.Single().Value;
                    return true;
                }
               
                value = partials.FirstOrDefault().Value;
                return true;
            }
            
            //if (Partials.TryGetValue(key: "key", value: out value))
            //    return true;

            value = null;
            return false;
                
            //Template
            //Pages/Template
            //Pages/Shared/Template
            //Shared/Template
            
            /*var resourceAssembly = Assembly.GetEntryAssembly();
            
            var resourceStream = resourceAssembly.GetManifestResourceStream("RazorEngineCore.Templates");

            if (resourceStream == null)
                throw new InvalidOperationException($"Could not find the requested resource (RazorEngineCore.Templates) in the specified assembly ({resourceAssembly.FullName})");
            
            var resourceReader = new ResourceReader(resourceStream);
            
            resourceReader.GetResourceData("razorenginecore", out var resourceType, out var resourceData);
            
            var templateDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(
                System.Text.Encoding.Unicode.GetString(resourceData, 4, resourceData.Length - 4));

            var templateKeyPair = templateDictionary.FirstOrDefault(fd => fd.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            
            resourceReader.GetResourceData(resourceName: templateKeyPair.Value, out _, out var outData);
            
            using var memoryStream = new MemoryStream(outData, false);
            memoryStream.Seek(4, SeekOrigin.Begin);
            
            var templateTask = RazorEngineCompiledTemplate.LoadFromStreamAsync<RazorEngineCorePageModel>(memoryStream);
                
            value = new Lazy<Task<IRazorEngineCompiledTemplate<RazorEngineCorePageModel>>>(() => templateTask);

            Partials.TryAdd(key: key, value: value);
                
            return true;*/

        }
    }
}