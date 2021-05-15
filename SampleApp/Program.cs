using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using RazorEngineCore;

[assembly: RazorEngineCoreTemplate("Pages/ContentTemplate", typeof(RazorEngineCorePageModel), fileName: "Pages\\ContentTemplate.cshtml", content: SampleApp.Content.SampleContent)]

namespace SampleApp
{

    public static class Content
    {
        public const string SampleContent = @"
@{
    /* Layout = ""_Layout""; */

    var jsonObject = new { Title = ""My Title"", Description = ""This is a description"", Null = (object)null };

    Json.WriteIndented(true)
        .IgnoreNullValues(true);
}

@*@RenderBody()*@

Hello @Model.Name

@foreach(var item in @Model.Items)
{
    <div>- @item</div>
}

<div data-name=""@Html.AttributeEncode(Model.Attribute)""></div>

<div data-name=""@(Model.Attribute)""></div>

<img href='@(""test"")'>

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

@(await Html.PartialAsync(""StaticPartialTemplate"", new { Name = ""Partial Template""}))
        
";

        public const string PartialContent = @"
Hello <b>@Model.Name</b>

@(await Html.PartialAsync(""Shared/_StaticSharedTemplate"", new { Name = ""Shared Template""}))
";

        public const string SharedContent = @"
Hello <b>@Model.Name</b>
";

        public const string LayoutContent = @"
<!DOCTYPE html>
<html lang=""en"">
<head>

</head>
<body>
<header>
    
</header>
<div>
    <main>
        @*@RenderBody()*@
        @(await Html.PartialAsync(""StaticBody""))
    </main>
</div>

<footer>
    
</footer>

</body>
</html>
";
    }

    public class Program
    {
        static async Task Main(string[] args)
        {
            
            //ResourceReader res = new ResourceReader(@".\RazorEngineCore.templates");
            //res.GetResourceData("razorenginecore", out var resourceType, out var resourceData);

            //var x = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(
            //    System.Text.Encoding.Unicode.GetString(resourceData, 4, resourceData.Length - 4));
                
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
                    await razorEngine.CompileAsync<RazorEngineCorePageModel>(SampleApp.Content.LayoutContent);
                
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

                ViewsManager.TryAdd("Pages/StaticBody", SampleApp.Content.SampleContent);
                ViewsManager.TryAdd("Pages/StaticPartialTemplate", SampleApp.Content.PartialContent);
                ViewsManager.TryAdd("Pages/Shared/_StaticSharedTemplate", SampleApp.Content.SharedContent);
                    
                var output = await template.RunAsync(model: model);
                
                sw.Stop();

                compileResults = compileResults.Add(sw.Elapsed);
                
                Console.WriteLine($"Runtime Compile :: {sw.Elapsed}");

                sw.Restart();
                
                var resourceTemplate = await PrecompiledTemplate.LoadAsync("Template", Assembly.GetExecutingAssembly());

                output = await resourceTemplate.RunAsync(model: model);
                
                //Console.WriteLine($"Precompiled :: \n\n{output}");
                
                sw.Stop();

                precompileResults = precompileResults.Add(sw.Elapsed);
                
                Console.WriteLine($"Precompiled :: {sw.Elapsed}");
            }

            Console.WriteLine($"Results of {runs} runs |");
            Console.WriteLine($"\tCompile    | total: {compileResults}; average: {TimeSpan.FromMilliseconds(compileResults.TotalMilliseconds / runs)}");
            
            Console.WriteLine($"\tPrecompile | total: {precompileResults}; average: {TimeSpan.FromMilliseconds(precompileResults.TotalMilliseconds / runs)}");
            
            Console.ReadKey();
        }
    }
}