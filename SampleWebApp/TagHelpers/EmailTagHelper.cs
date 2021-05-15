using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SampleWebApp.TagHelpers
{
    /*[HtmlTargetElement("email", Attributes = "*")]
    public class EmailTagHelper: TagHelper
    {
        private const string EmailDomain = "avengers.mcu";

        // Can be passed via <email mail-to="..." />. 
        // PascalCase gets translated into kebab-case.
        public string MailTo { get; set; }

        // synchronous method, CANNOT call output.GetChildContentAsync();
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 1. Set the HTML element as the tag name to replace it with, e.g. <a>
            output.TagName = "a";

            var address = MailTo + "@" + EmailDomain;

            // 2. Set the href attribute within that HTML element, e.g. href
            output.Attributes.SetAttribute("href", "mailto:" + address);

            // 3. Set HTML Content within the tags.
            output.Content.SetContent(address);
        }

    }
    
    [HtmlTargetElement("async-email", Attributes = "*")]
    public class AsyncEmailTagHelper : TagHelper
    {
        private const string EmailDomain = "avengers.mcu";

        // Can be passed via <email mail-to="..." />. 
        // PascalCase gets translated into kebab-case.
        public string MailTo { get; set; }

        // ASYNC method, REQUIRED to call output.GetChildContentAsync();
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // 1. Set the HTML element as the tag name to replace it with, e.g. <a>
            output.TagName = "a";

            var content = await output.GetChildContentAsync();
            var target = content.GetContent() + "@" + EmailDomain;

            // 2. Set the href attribute within that HTML element, e.g. href
            output.Attributes.SetAttribute("href", "mailto:" + target);

            // 3. Set HTML Content within the tags.
            output.Content.SetContent(target);
        }
    }
    
    [HtmlTargetElement("*", Attributes = "*")]
    public class ResourcePolicyAuthorizationTagHelper : TagHelper
    {
        public ResourcePolicyAuthorizationTagHelper()
        {

        }

        [HtmlAttributeName("asp-authpolicy")]
        public string PolicyName { get; set; }

        [HtmlAttributeName("asp-route-id")]
        public string ResourceId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // ...
        }
    }*/
    
    [HtmlTargetElement("colorlabel")]
    public class ColorLabelTagHelper : TagHelper
    {
        public ColorLabelTagHelper()
        {
            //Microsoft.AspNetCore.Razor.Language.RazorTemplateEngine.
        }
        
        [HtmlAttributeName("color")]
        public string Color { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            
            output.TagName = "coloredlabel";
            var colorStyle = $"color:{Color}";
            output.Attributes.SetAttribute("style", colorStyle);
            if (Color == "red")
                output.Content.SetContent("Text from custom helper");
        }
    }
}