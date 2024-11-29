using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;

namespace WebVella.TagHelpers.TagHelpers
{

	//[OutputElementHint("div")]
	[HtmlTargetElement("wv-row")]
	public class WvRow : TagHelper
	{

		[HtmlAttributeName("flex-vertical-alignment")]
		public WvFlexVerticalAlignmentType FlexVerticalAlignment { get; set; } = WvFlexVerticalAlignmentType.None;

		[HtmlAttributeName("flex-horizontal-alignment")]
		public WvFlexHorizontalAlignmentType FlexHorizontalAlignment { get; set; } = WvFlexHorizontalAlignmentType.None;

		[HtmlAttributeName("no-gutters")]
		public bool NoGutters { get; set; } = false;

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";


		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "";
			//output.TagName = "div";
			//var wrapperClassList = new List<string>();
			//wrapperClassList.Add("container-fluid");
			//output.Attributes.SetAttribute("class", String.Join(" ", wrapperClassList));



			//Inner wrapper
			var rowEl = new TagBuilder("div");
			rowEl.AddCssClass("lnr row");
			if (!String.IsNullOrWhiteSpace(Class))
			{
				rowEl.AddCssClass(Class);
			}

			if (FlexVerticalAlignment != WvFlexVerticalAlignmentType.None)
			{
				switch (FlexVerticalAlignment)
				{
					case WvFlexVerticalAlignmentType.Start:
						rowEl.AddCssClass("align-items-start");
						break;
					case WvFlexVerticalAlignmentType.Center:
						rowEl.AddCssClass("align-items-center");
						break;
					case WvFlexVerticalAlignmentType.End:
						rowEl.AddCssClass("align-items-end");
						break;
					default:
						break;
				}

			}
			if (FlexHorizontalAlignment != WvFlexHorizontalAlignmentType.None)
			{
				switch (FlexHorizontalAlignment)
				{
					case WvFlexHorizontalAlignmentType.Start:
						rowEl.AddCssClass("justify-content-start");
						break;
					case WvFlexHorizontalAlignmentType.Center:
						rowEl.AddCssClass("justify-content-center");
						break;
					case WvFlexHorizontalAlignmentType.End:
						rowEl.AddCssClass("justify-content-end");
						break;
					case WvFlexHorizontalAlignmentType.Between:
						rowEl.AddCssClass("justify-content-between");
						break;
					case WvFlexHorizontalAlignmentType.Around:
						rowEl.AddCssClass($"justify-content-around");
						break;
					default:
						break;
				}
			}
			if (NoGutters)
			{
				rowEl.AddCssClass("no-gutters");
			}

			output.PreContent.AppendHtml(rowEl.RenderStartTag());

			output.PostContent.AppendHtml(rowEl.RenderEndTag());

			return Task.CompletedTask;
		}
	}
}
