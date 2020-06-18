using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RazorEngineCore;

[assembly: PrecompiledTemplate(typeof(RazorEngineCorePageModel), SampleApp.Content.SampleContent)]
//RazorEngineCorePageModel

namespace SampleApp
{

    public static class Content
    {
        public const string SampleContent = @"      

@{
    var jsonObject = new { Title = ""My Title"", Description = ""This is a description"", Null = (object)null };

    Json.WriteIndented(true)
        .IgnoreNullValues(true);
}

@RenderBody(Model)

Hello @Model.Name

@foreach(var item in @Model.Items)
{
    <div>- @item</div>
}

<div data-name=""@Html.AttributeEncode(Model.Attribute)""></div>

<div data-name=""@(Model.Attribute)""></div>

<div style=""margin: 16px"">
    @@()
    <code style=""display: block"">
        @(""<div>string</div>"")    
    </code>
</div>

<div style=""margin: 16px"">
    @@Html.Encode()
    <code style=""display: block"">
        @Html.Encode(""<div>string</div>"")    
    </code>
</div>

<div style=""margin: 16px"">
    @@Html.AttributeEncode()
    <code style=""display: block"">
        @Html.AttributeEncode(""<div>string</div>"")
    </code>
</div>

<div style=""margin: 16px"">
    @@Html.Raw()
    <code style=""display: block"">
        @Html.Raw(""<div>string</div>"")    
    </code>
</div>

<div style=""margin: 16px"">
    @@Json.Serialize()
    <code style=""display: block"">
        @Json.Serialize(jsonObject)
    </code>
</div>

<div style=""margin: 16px"">
    @@Json.Serialize()
    <code style=""display: block"">
        @Json.Serialize(jsonObject, 
            writeIndented: false,
            ignoreNullValues: false)
    </code>
</div>

<area>
    @{ RecursionTest(3); }
</area>

@{
	void RecursionTest(int level){
		if (level <= 0)
		{
			return;
		}
			
		<div>LEVEL: @level</div>
		@{ RecursionTest(level - 1); }
	}
}
";
    }

    public class Program
    {
        
        
        
        static async Task Main(string[] args)
        {

            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            
            
            var assEx = Assembly.LoadFile($"{dir}{Path.DirectorySeparatorChar}RazorEngineCore.Extensions.dll");
            var ass = Assembly.LoadFile($"{dir}{Path.DirectorySeparatorChar}RazorEngineCore.dll");

            ass.GetCustomAttributes<RazorEngineCore.PrecompiledTemplate>();
            
            var type = ass.GetType("RazorEngineCore.RazorEngine");
            var templateType = ass.GetType("RazorEngineCore.RazorEngineCompiledTemplate");
            
            var instance = Activator.CreateInstance(type);

            //var method = instance.GetType().GetMethods().Single(w => w.IsGenericMethod && w.Name.Equals("CompileAsync"));//.MakeGenericMethod(typeof(RazorEngineCore.RazorEngineCorePageModel));
            var method = type.GetMethods().Single(w => w.IsGenericMethod && w.Name.Equals("Compile"));//.MakeGenericMethod(typeof(RazorEngineCore.RazorEngineCorePageModel));

            var tempType = typeof(RazorEngineCorePageModel);
            
            var genericArgument = method.GetGenericArguments().FirstOrDefault();
            if (genericArgument != null)
            {
                Type newType = tempType.MakeGenericType(genericArgument);
                
                method = method.MakeGenericMethod(templateType);
            }
            
            //var method = instance.GetType().GetMethod($"CompileAsync<RazorEngineCore.RazorEngineCorePageModel>").MakeGenericMethod(typeof(RazorEngineCore.RazorEngineCorePageModel));


            var x = method.Invoke(instance, new object[] {"Content.SampleContent", null});// as Task);
                
            RazorEngineCore.RazorEngine razorEngine = new RazorEngineCore.RazorEngine();
            
            var template = razorEngine.Compile<RazorEngineCorePageModel>(Content.SampleContent);
            
            //var template = await razorEngine.CompileAsync<RazorEngineCorePageModel>(Content.SampleContent);

            var model = new
            {
                Name = "Alexander",
                Attribute = "<encode me>",
                Items = new List<string>()
                {
                    "item 1",
                    "item 2"
                }
            };
            
            string result = await template.RunAsync(model: model);

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}