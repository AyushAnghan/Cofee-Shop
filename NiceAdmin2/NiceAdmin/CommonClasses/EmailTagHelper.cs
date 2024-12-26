using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace AuthoringTagHelpers.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        public string MailTo { get; set; }
        public string EmailDomain { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Ensure tag is set to <a>
            output.TagName = "a";
            output.Attributes.SetAttribute("href", MailTo + "@" + EmailDomain);
            // Combine MailTo and EmailDomain to form a full email address
            /*var address = MailTo + "@" + EmailDomain;*/
            output.Content.SetContent("Click");
            output.Attributes.SetAttribute("Style", "color:green");
/*
            // Set the href attribute
            output.Attributes.SetAttribute("href", "mailto:" + address);

            // If there is no content inside the tag, set the content to the email address
            if (output.Content.IsEmptyOrWhiteSpace)
            {
                output.Content.SetContent(address);
            }*/
        }
    }
}