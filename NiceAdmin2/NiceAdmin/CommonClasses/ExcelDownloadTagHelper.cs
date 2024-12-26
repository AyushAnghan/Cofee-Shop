using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AuthoringTagHelpers.TagHelpers
{
	public class ExcelDownloadTagHelper : TagHelper
    {
        public string controller { get; set; }
        public string action { get; set; }

        public string CssClass { get; set; } = "btn btn-primary mb-3";  // Default class

        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            
            output.TagName = "a";
            output.Attributes.SetAttribute("href", "/"+controller+"/"+action);
            output.Content.SetContent("Download Excel22");
            output.Attributes.SetAttribute("class", CssClass);
            /*output.Attributes.SetAttribute("Style", "color:green");*/
        }
    }
}