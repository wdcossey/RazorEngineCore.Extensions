using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RazorEngineCore;

[assembly: PrecompiledTemplate("sample", typeof(RazorEngineCorePageModel), SampleApp.Content.SampleContent)]

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

            RazorEngine razorEngine = new RazorEngine();

            //Warm up CSharpCompilation, ensure fairness when doing a Stopwatch comparison
            var mocktemplate =
                await razorEngine.CompileAsync<RazorEngineCorePageModel>("");

            var runs = 10;
            
            var compileResults = TimeSpan.Zero;
            var precompileResults = TimeSpan.Zero;
            
            for (int i = 0; i < runs; i++)
            {
                var sw = new Stopwatch();

                sw.Start();
                
                var template =
                    await razorEngine.CompileAsync<RazorEngineCorePageModel>(SampleApp.Content.SampleContent);
                
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

                await template.RunAsync(model: model);
                
                sw.Stop();

                compileResults = compileResults.Add(sw.Elapsed);
                
                Console.WriteLine($"Compile :: {sw.Elapsed}");

                sw.Restart();
                
                var resourceTemplate = await PrecompiledTemplate.LoadAsync("sample");

                await resourceTemplate.RunAsync(model: model);
                
                sw.Stop();

                precompileResults = precompileResults.Add(sw.Elapsed);
                
                Console.WriteLine($"Precompile :: {sw.Elapsed}");
            }

            Console.WriteLine($"Results of {runs} runs |");
            Console.WriteLine($"\tCompile    | total: {compileResults}; average: {TimeSpan.FromMilliseconds(compileResults.TotalMilliseconds / runs)}");
            
            Console.WriteLine($"\tPrecompile | total: {precompileResults}; average: {TimeSpan.FromMilliseconds(precompileResults.TotalMilliseconds / runs)}");
            
            Console.ReadKey();
        }
    }
}