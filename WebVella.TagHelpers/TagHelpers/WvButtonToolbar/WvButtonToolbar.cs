using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-button-toolbar")]
	public class WvButtonToolbar : TagHelper
	{
		public WvButtonToolbar(IHtmlGenerator generator)
		{
			Generator = generator;
		}

		protected IHtmlGenerator Generator { get; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName("is-visible")]
		public bool isVisible { get; set; } = true;

		[HtmlAttributeName("size")]
		public WvCssSize Size { get; set; } = WvCssSize.Small;

		[HtmlAttributeName("is-vertical")]
		public bool IsVertical { get; set; } = false;

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";

		[HtmlAttributeName("id")]
		public string Id { get; set; } = "";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			var classList = new List<string>();

			output.TagName = "div";
			classList.Add("btn-toolbar");

			#region << Id >>
			if (!String.IsNullOrWhiteSpace(Id))
			{
				output.Attributes.SetAttribute("id", Id);
			}
			#endregion

			#region << Class >>
			if (!String.IsNullOrWhiteSpace(Class))
			{
				classList.Add(Class);
			}
			#endregion

			output.Attributes.SetAttribute("class", String.Join(" ", classList));

			context.Items[typeof(WvCssSize)] = Size;
			context.Items["IsVertical"] = IsVertical;
			return Task.CompletedTask;
		}
	}
}
