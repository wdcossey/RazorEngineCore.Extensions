using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;
using RazorEngineCore;

namespace SampleApp
{
    class Program
    {
        
        static string Content = @"Hello @Model.Name";
        /*static string Content = @"      

Hello @Model.Name

@foreach(var item in @Model.Items)
{
    <div>- @item</div>
}

<div data-name=""@Model.Name""></div>

@(""<div>encoded string</div>"")
@Html.Encode(""<div>encoded string</div>"")
@Html.AttributeEncode(""<div>encoded string</div>"")
@Html.Raw(""<div>encoded string</div>"")

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
";*/
        
        static void Main(string[] args)
        {
            //Microsoft.AspNetCore.Razor.Language.razorps
            //RazorPage
            //HtmlHelper x;
            //x.AntiForgeryToken()
            
            RazorEngine razorEngine = new RazorEngine();
            
            var template = razorEngine.Compile<RazorEngineCorePageModel>(Content, builder =>
            {
                builder.AddAssemblyReference(typeof(Microsoft.AspNetCore.Html.IHtmlContent));
            });

            /*
                new
                {
                    Name = "Alexander",
                    Items = new List<string>()
                    {
                        "item 1",
                        "item 2"
                    }
                }
            */
            
            string result = template.Run(model => model.Model = new
            {
                Name = "Alexander",
                Items = new List<string>()
                {
                    "item 1",
                    "item 2"
                }
            });

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}