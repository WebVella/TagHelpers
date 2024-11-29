using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-alert")]
	public class WvAlert : TagHelper
	{
		public WvAlert(IHtmlGenerator generator)
		{
			Generator = generator;
		}

		protected IHtmlGenerator Generator { get; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }


		[HtmlAttributeName("type")]
		public WvAlertType Type { get; set; } = WvAlertType.Warning;

		[HtmlAttributeName("content")]
		public string Content { get; set; } = "";

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";

		[HtmlAttributeName("icon-class")]
		public string IconClass { get; set; } = "";

		[HtmlAttributeName("title")]
		public string Title { get; set; } = "";

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (String.IsNullOrWhiteSpace(Title) && String.IsNullOrWhiteSpace(Content))
			{
				output.SuppressOutput();
				return;
			}

			output.TagName = "div";
			output.Attributes.Add("class", $"alert alert-{ModelExtensions.GetLabel<WvAlertType>(Type)} wv-alert {(String.IsNullOrWhiteSpace(IconClass) ? "" : " alert-icon ")} {Class}");

			//>> Icon
			if (!String.IsNullOrWhiteSpace(IconClass))
			{
				var iconEl = new TagBuilder("div");
				iconEl.AddCssClass("icon");
				iconEl.InnerHtml.AppendHtml($"<i class='{IconClass}'></i>");
				output.Content.AppendHtml(iconEl);
			}
			//>> content
			var contentEl = new TagBuilder("div");
			contentEl.AddCssClass("content");

			if (!String.IsNullOrWhiteSpace(Title))
			{
				//>> content >> title
				var titleEl = new TagBuilder("div");
				titleEl.AddCssClass("title");
				titleEl.InnerHtml.AppendHtml(Title);
				contentEl.InnerHtml.AppendHtml(titleEl);
			}

			if (!String.IsNullOrWhiteSpace(Content))
			{
				//>> content >> text
				var textEl = new TagBuilder("div");
				textEl.AddCssClass("text");
				textEl.InnerHtml.AppendHtml(Content);
				contentEl.InnerHtml.AppendHtml(textEl);
			}

			output.Content.AppendHtml(contentEl);





		}
	}
}
