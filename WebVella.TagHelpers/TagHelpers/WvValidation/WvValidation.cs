using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{

	[HtmlTargetElement("wv-validation")]
	public class WvValidation : TagHelper
	{
		[HtmlAttributeName("errors")]
		public List<KeyValuePair<string,string>> Errors { get; set; } = new List<KeyValuePair<string,string>>();

		[HtmlAttributeName("message")]
		public string Message { get; set; } = "";

		[HtmlAttributeName("show-icon")]
		public bool ShowIcon { get; set; } = false;

		[HtmlAttributeName("show-key")]
		public bool ShowKey { get; set; } = false;

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (String.IsNullOrEmpty(Message) && Errors.Count == 0)
			{
				output.SuppressOutput();
				return;
			}

			output.TagName = "div";
			output.AddCssClass($"alert alert-danger wv-alert {(ShowIcon ? " alert-icon " : "")} {Class}");

			//>> Icon
			if (ShowIcon)
			{
				var iconEl = new TagBuilder("div");
				iconEl.AddCssClass("icon");
				iconEl.InnerHtml.AppendHtml($"<i class='fas fa-fw fa-exclamation-circle'></i>");
				output.Content.AppendHtml(iconEl);
			}

			//>> content
			var contentEl = new TagBuilder("div");
			contentEl.AddCssClass("content");

			//>> content >> message
			if (!String.IsNullOrWhiteSpace(Message))
			{
				var titleEl = new TagBuilder("div");
				if(Errors.Count > 0)
					titleEl.AddCssClass("title");

				titleEl.InnerHtml.AppendHtml(Message);
				contentEl.InnerHtml.AppendHtml(titleEl);
			}

			//>> content >> errors
			if(Errors.Count > 0){
				var ulEl = new TagBuilder("ul");
				ulEl.AddCssClass($"list");

				foreach (var error in Errors)
				{
					var liEl = new TagBuilder("li");
					if (!ShowKey)
					{
						liEl.InnerHtml.AppendHtml(error.Value);
					}
					else
					{
						var keyEl = new TagBuilder("strong");
						keyEl.InnerHtml.AppendHtml(error.Key);
						liEl.InnerHtml.AppendHtml(keyEl);
						liEl.InnerHtml.AppendHtml($" - {error.Value}");
					}
					ulEl.InnerHtml.AppendHtml(liEl);
				}
				contentEl.InnerHtml.AppendHtml(ulEl);
			}

			output.Content.AppendHtml(contentEl);

		}
	}
}
