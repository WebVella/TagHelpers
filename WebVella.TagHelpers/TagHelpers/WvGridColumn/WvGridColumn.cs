using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-grid-column", ParentTag = "wv-grid-row")]
	public class WvGridColumn : TagHelper
	{
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName("is-visible")]
		public bool isVisible { get; set; } = true;

		[HtmlAttributeName("vertical-align")]
		public WvVerticalAlignmentType VerticalAlign { get; set; } = WvVerticalAlignmentType.None;

		[HtmlAttributeName("horizontal-align")]
		public WvHorizontalAlignmentType HorizontalAlign { get; set; } = WvHorizontalAlignmentType.None;

		[HtmlAttributeName("text-nowrap")]
		public bool TextNoWrap { get; set; } = false;

		[HtmlAttributeName("is-header")]
		public bool IsHeader { get; set; } = false;

		[HtmlAttributeName("class")]
		public string Class { get; set; } = null;

		[HtmlAttributeName("colspan")]
		public int? Colspan { get; set; } = null;

		[HtmlAttributeName("width")]
		public string Width { get; set; } = "";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			#region << Init >>
			if (VerticalAlign == WvVerticalAlignmentType.None && context.Items.ContainsKey(typeof(WvVerticalAlignmentType)))
			{
				VerticalAlign = (WvVerticalAlignmentType)context.Items[typeof(WvVerticalAlignmentType)];
			}
			#endregion

			#region << Render >>

			output.TagName = "td";

			if (IsHeader)
				output.TagName = "th";

			if (!String.IsNullOrWhiteSpace(Class))
			{
				output.AddCssClass(Class);
			}

			var styleList = new List<string>();
			switch (VerticalAlign)
			{
				case WvVerticalAlignmentType.Top:
					styleList.Add("vertical-align:top");
					break;
				case WvVerticalAlignmentType.Middle:
					styleList.Add("vertical-align:middle");
					break;
				case WvVerticalAlignmentType.Bottom:
					styleList.Add("vertical-align:bottom");
					break;
				default:
					break;
			}

			if (TextNoWrap)
			{
				styleList.Add("white-space:nowrap");
			}

			switch (HorizontalAlign)
			{
				case WvHorizontalAlignmentType.Left:
					styleList.Add("text-align:left");
					break;
				case WvHorizontalAlignmentType.Center:
					styleList.Add("text-align:center");
					break;
				case WvHorizontalAlignmentType.Right:
					styleList.Add("text-align:right");
					break;
				default:
					break;
			}

			if (!String.IsNullOrEmpty(Width))
			{
				styleList.Add($"width:{Width}");
			}

			if (styleList.Count > 0)
			{
				output.Attributes.Add("style", String.Join("; ", styleList));
			}

			if (Colspan != null)
			{
				output.Attributes.Add("colspan", Colspan);
			}

			#endregion

			return Task.CompletedTask;
		}
	}
}
