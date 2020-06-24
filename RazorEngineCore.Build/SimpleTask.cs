using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using RazorEngineCore;

namespace RazorEngineCore.Build
{
    public class SimpleTask : Task
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
        
        [Required] 
        public string ExtensionsAssembly { get; set; }
        
        [Required] 
        public string TargetDir { get; set; }
        
        [Required] 
        public string TargetFileName { get; set; }
        

        public override bool Execute()
        {
            
            /*TaskEventArgs taskEvent;
            taskEvent = new TaskEventArgs(BuildEventCategory.Custom,
                BuildEventImportance.High, "Important Message",
                "SimpleTask");*/
            ;
            
            Log.LogError("messageResource1", "1", "2", "3");
            Log.LogError("messageResource2");
            Log.LogError(BuildEngine.ProjectFileOfTaskNode);

            var outputFile = Path.Combine(this.TargetDir, this.TargetFileName);
            Log.LogError(outputFile);
            
            RazorEngine razorEngine = new RazorEngine();
            
            var ass = Assembly.LoadFile(outputFile);
            var ass1 = typeof(PrecompiledTemplate).Assembly;
            Log.LogError(ass1.Location);
            Assembly.Load(ass1.GetName());

            //ass.GetCustomAttributes<PrecompiledTemplate>();

            
            var template = razorEngine.Compile<RazorEngineCorePageModel>("");
            
            return true;
        }
    }
}