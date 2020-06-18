using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RazorEngineCore
{
    public class SimpleTask : ContextIsolatedTask
    {
        //When implementing the ITask interface, it is necessary to
        //implement a BuildEngine property of type
        //Microsoft.Build.Framework.IBuildEngine. This is done for
        //you if you derive from the Task class.
        //public IBuildEngine BuildEngine { get; set; }

        // When implementing the ITask interface, it is necessary to
        // implement a HostObject property of type object.
        // This is done for you if you derive from the Task class.
        //public ITaskHost HostObject { get; set; }

        [Required] public string ExtensionsAssembly { get; set; }

        [Required] public string TargetDir { get; set; }

        [Required] public string TargetFileName { get; set; }

        protected override bool ExecuteIsolated()
        {
            /*TaskEventArgs taskEvent;
            taskEvent = new TaskEventArgs(BuildEventCategory.Custom,
                BuildEventImportance.High, "Important Message",
                "SimpleTask");*/
            ;

            Log.LogWarning("messageResource1", "1", "2", "3");
            Log.LogWarning("messageResource2");
            Log.LogWarning(BuildEngine.ProjectFileOfTaskNode);

            var outputFile = Path.Combine(this.TargetDir, this.TargetFileName);
            Log.LogWarning(outputFile);

            RazorEngine razorEngine = new RazorEngine();

            Log.LogWarning(Assembly.GetExecutingAssembly().FullName);

            foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                //this.ctxt.LoadFromAssemblyName(assemblyName);
                Log.LogWarning(assemblyName.Name);
            }
            
            var ass =  Assembly.LoadFile(outputFile);
            //var ass1 = typeof(PrecompiledTemplate).Assembly;
            //Log.LogError(ass1.Location);
            //Assembly.Load(ass1.GetName());

            var managedDllDirectory =
                Path.GetDirectoryName(new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath);
            Log.LogWarning(managedDllDirectory);

            ass.GetCustomAttributes();

            var template = razorEngine.Compile<RazorEngineCorePageModel>("");

            return true;
        }
        
    }

    
    partial class ContextIsolatedTask : Task
    {
        /// <summary>
        /// The context the inner task is loaded within.
        /// </summary>
        protected AssemblyLoadContext ctxt;

        /// <inheritdoc />
        public sealed override bool Execute()
        {
            try
            {
                string taskAssemblyPath = new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath;
                
                
                
                this.ctxt = new CustomAssemblyLoader(this);
                Assembly inContextAssembly = this.ctxt.LoadFromAssemblyPath(taskAssemblyPath);
                
                Log.LogWarning(inContextAssembly.FullName);
                
                Type innerTaskType = inContextAssembly.GetType(this.GetType().FullName);

                object innerTask = Activator.CreateInstance(innerTaskType);
                return this.ExecuteInnerTask(innerTask);
            }
            catch (OperationCanceledException)
            {
                this.Log.LogMessage(MessageImportance.High, "Canceled.");
                return false;
            }
        }

        /// <summary>
        /// Loads the assembly at the specified path within the isolated context.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly to be loaded.</param>
        /// <returns>The loaded assembly.</returns>
        protected Assembly LoadAssemblyByPath(string assemblyPath)
        {
            if (this.ctxt == null)
            {
                throw new InvalidOperationException("AssemblyLoadContext must be set first.");
            }

            return this.ctxt.LoadFromAssemblyPath(assemblyPath);
        }

        private bool ExecuteInnerTask(object innerTask)
        {
            Type innerTaskType = innerTask.GetType();
            Type innerTaskBaseType = innerTaskType;
            while (innerTaskBaseType.FullName != typeof(ContextIsolatedTask).FullName)
            {
                innerTaskBaseType = innerTaskBaseType.GetTypeInfo().BaseType;
            }

            var outerProperties = this.GetType().GetRuntimeProperties().ToDictionary(i => i.Name);
            var innerProperties = innerTaskType.GetRuntimeProperties().ToDictionary(i => i.Name);
            var propertiesDiscovery = from outerProperty in outerProperties.Values
                                      where outerProperty.SetMethod != null && outerProperty.GetMethod != null
                                      let innerProperty = innerProperties[outerProperty.Name]
                                      select new { outerProperty, innerProperty };
            var propertiesMap = propertiesDiscovery.ToArray();
            var outputPropertiesMap = propertiesMap.Where(pair => pair.outerProperty.GetCustomAttribute<OutputAttribute>() != null).ToArray();

            foreach (var propertyPair in propertiesMap)
            {
                object outerPropertyValue = propertyPair.outerProperty.GetValue(this);
                propertyPair.innerProperty.SetValue(innerTask, outerPropertyValue);
            }

            // Forward any cancellation requests
            MethodInfo innerCancelMethod = innerTaskType.GetMethod(nameof(Cancel));
            using (this.CancellationToken.Register(() => innerCancelMethod.Invoke(innerTask, new object[0])))
            {
                this.CancellationToken.ThrowIfCancellationRequested();

                // Execute the inner task.
                var executeInnerMethod = innerTaskType.GetMethod(nameof(ExecuteIsolated), BindingFlags.Instance | BindingFlags.NonPublic);
                bool result = (bool)executeInnerMethod.Invoke(innerTask, new object[0]);

                // Retrieve any output properties.
                foreach (var propertyPair in outputPropertiesMap)
                {
                    propertyPair.outerProperty.SetValue(this, propertyPair.innerProperty.GetValue(innerTask));
                }

                return result;
            }
        }

        private class CustomAssemblyLoader : AssemblyLoadContext
        {
            private readonly ContextIsolatedTask loaderTask;

            internal CustomAssemblyLoader(ContextIsolatedTask loaderTask)
            {
                this.loaderTask = loaderTask;
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return this.loaderTask.LoadAssemblyByName(assemblyName)
                    ?? Default.LoadFromAssemblyName(assemblyName);
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string unmanagedDllPath = Directory.EnumerateFiles(
                    this.loaderTask.UnmanagedDllDirectory,
                    $"{unmanagedDllName}.*").Concat(
                        Directory.EnumerateFiles(
                            this.loaderTask.UnmanagedDllDirectory,
                            $"lib{unmanagedDllName}.*"))
                    .FirstOrDefault();
                if (unmanagedDllPath != null)
                {
                    return this.LoadUnmanagedDllFromPath(unmanagedDllPath);
                }

                return base.LoadUnmanagedDll(unmanagedDllName);
            }
        }
    }
        
    // <summary>
    /// A base class to use for an MSBuild Task that needs to supply its own dependencies
    /// independently of the assemblies that the hosting build engine may be willing to supply.
    /// </summary>
    public abstract partial class ContextIsolatedTask : ICancelableTask
    {
        /// <summary>
        /// The source of the <see cref="CancellationToken" /> that is canceled when
        /// <see cref="ICancelableTask.Cancel" /> is invoked.
        /// </summary>
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>Gets a token that is canceled when MSBuild is requesting that we abort.</summary>
        public CancellationToken CancellationToken => this.cts.Token;

        /// <summary>Gets the path to the directory containing managed dependencies.</summary>
        protected virtual string ManagedDllDirectory =>
            Path.GetDirectoryName(new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath);

        /// <summary>
        /// Gets the path to the directory containing native dependencies.
        /// May be null if no native dependencies are required.
        /// </summary>
        public virtual string UnmanagedDllDirectory => null;

        /// <inheritdoc />
        public void Cancel() => this.cts.Cancel();

        /// <summary>
        /// The body of the Task to execute within the isolation boundary.
        /// </summary>
        protected abstract bool ExecuteIsolated();

        /// <summary>
        /// Loads an assembly with a given name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to load.</param>
        /// <returns>The loaded assembly, if one could be found; otherwise <c>null</c>.</returns>
        /// <remarks>
        /// The default implementation searches the <see cref="ManagedDllDirectory"/> folder for
        /// any assembly with a matching simple name.
        /// Derived types may use <see cref="LoadAssemblyByPath(string)"/> to load an assembly
        /// from a given path once some path is found.
        /// </remarks>
        public virtual Assembly LoadAssemblyByName(AssemblyName assemblyName)
        {
            if (assemblyName.Name.StartsWith("Microsoft.Build", StringComparison.OrdinalIgnoreCase) ||
                assemblyName.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
            {
                // MSBuild and System.* make up our exchange types. So don't load them in this LoadContext.
                // We need to inherit them from the default load context.
                return null;
            }

            string assemblyPath = Path.Combine(this.ManagedDllDirectory, assemblyName.Name) + ".dll";
            if (File.Exists(assemblyPath))
            {
                return this.LoadAssemblyByPath(assemblyPath);
            }

            return null;
        }
    }
}