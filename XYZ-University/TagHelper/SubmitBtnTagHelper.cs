using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XYZ_University.TagHelpers
{
    [HtmlTargetElement("submit-btn")]
    public class SubmitBtnTagHelper : TagHelper
    {
        public string Text { get; set; } = "Submit";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            output.Attributes.SetAttribute("type", "submit");
            output.Attributes.SetAttribute("class", "btn btn-primary fw-bold px-4");
            output.Content.SetContent(Text);
        }
    }
}