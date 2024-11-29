using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-append")]
	public class WvFieldAppend : TagHelper
	{
		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			return Task.CompletedTask;
		}

	}
}
