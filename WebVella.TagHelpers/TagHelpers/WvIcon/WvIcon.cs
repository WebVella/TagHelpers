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

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{

			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			var cssList = new List<string>();
			cssList.Add("wv-icon");
			cssList.Add(Class);
			var typeDescription = IconTypeService.GetDescriptionByIconType(Type);
			if (Type != WvIconType.NoIcon)
			{
				if (typeDescription.StartsWith("bs"))
				{
					cssList.Add($" wvp-icon--bs ");
				}
				else if (typeDescription.StartsWith("mdf"))
				{
					cssList.Add($" wvp-icon--mdf ");
				}
				else{}
			}

			output.Attributes.Add("class", string.Join(" ", cssList));
			output.Content.AppendHtml(IconTypeService.GetSVGContentForIconType(Type));
		}
	}
}
