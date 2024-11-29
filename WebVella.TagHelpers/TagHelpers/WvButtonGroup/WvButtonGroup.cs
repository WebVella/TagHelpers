using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-button-group")]
	public class WvButtonGroup : TagHelper
	{
		public WvButtonGroup(IHtmlGenerator generator)
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
		public WvCssSize Size { get; set; } = WvCssSize.Inherit;

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

			#region << Init >>
			if (Size == WvCssSize.Inherit) {
				if (context.Items.ContainsKey(typeof(WvCssSize)))
				{
					Size = (WvCssSize)context.Items[typeof(WvCssSize)];
				}
				else
				{
					Size = WvCssSize.Small;
				}
			}
			if (context.Items.ContainsKey("IsVertical"))
			{
				IsVertical = (bool)context.Items["IsVertical"];
			}
			#endregion


			var classList = new List<string>();

			output.TagName = "div";
			if (IsVertical)
			{
				classList.Add("btn-group-vertical");
			}
			else
			{
				classList.Add("btn-group");
			}

			#region << Size >>
			if (Size == WvCssSize.Small)
			{
				classList.Add("btn-group-sm");
			}
			else if (Size == WvCssSize.Large) {
				classList.Add("btn-group-lg");
			}
			#endregion

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
			return Task.CompletedTask;
		}
	}
}
