using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{

	//[OutputElementHint("div")]
	[HtmlTargetElement("wv-modal-body")]
	public class WvModalBody : TagHelper
	{

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			output.AddCssClass("modal-body");

			return Task.CompletedTask;
		}
	}
}
