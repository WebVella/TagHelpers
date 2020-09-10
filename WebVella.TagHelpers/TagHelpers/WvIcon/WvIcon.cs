using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Services;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-icon")]
	public class WvIcon : TagHelper
	{
		public WvIcon(IHtmlGenerator generator)
		{
			Generator = generator;
		}

		protected IHtmlGenerator Generator { get; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }


		[HtmlAttributeName("type")]
		public WvIconType Type { get; set; } = WvIconType.BsQuestion;

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{

			output.TagName = "span";
			var cssList = new List<string>();
			cssList.Add("wv-icon");
			cssList.Add(Class);

			var svgEl = new TagBuilder("svg");
			svgEl.Attributes.Add("xmlns", "http://www.w3.org/2000/svg");
			svgEl.Attributes.Add("fill", $"currentColor");


			if (Type != WvIconType.NoIcon)
			{
				var typeDescription = Type.ToDescriptionString();
				if (typeDescription.StartsWith("bs"))
				{
					svgEl.Attributes.Add("viewBox", $"0 0 16 16");
					cssList.Add($" wvp-icon--bs ");
				}
				else if (typeDescription.StartsWith("mdf"))
				{
					svgEl.Attributes.Add("viewBox", $"0 0 24 24");
					cssList.Add($" wvp-icon--mdf ");
				}
				else{
					svgEl.Attributes.Add("viewBox", $"0 0 16 16");
				}

				svgEl.InnerHtml.AppendHtml(IconTypeService.GetSVGContentForIconType(Type));
			}

			output.Attributes.Add("class", string.Join(" ", cssList));
			output.Content.AppendHtml(svgEl);

		}
	}
}
