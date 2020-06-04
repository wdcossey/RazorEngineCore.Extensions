using System;
using System.Collections.Generic;
using RazorEngineCore;

namespace SampleApp
{
    class Program
    {
        
        static string Content = @"      

@{
    Json.WriteIndented(true)
        .IgnoreNullValues(true);
}

Hello @Model.Name

@foreach(var item in @Model.Items)
{
    <div>- @item</div>
}

<div data-name=""@Html.AttributeEncode(Model.Attribute)""></div>

<div data-name=""@(Model.Attribute)""></div>

@(""<div>string</div>"")
@Html.Encode(""<div>string</div>"")
@Html.AttributeEncode(""<div>string</div>"")
@Html.Raw(""<div>string</div>"")

<code>
@Json.Serialize(new { Title = ""My Title"", Description = ""This is a description"", Null = (object)null })
</code>
        
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
        
        static void Main(string[] args)
        {
            RazorEngine razorEngine = new RazorEngine();
            
            var template = razorEngine.Compile<RazorEngineCorePageModel>(Content);

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
            
            string result = template.Run(model: model);

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}