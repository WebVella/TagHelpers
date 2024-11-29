using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using System.Web;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-code-highlight")]
	public class WvCodeHighlight : TagHelper
	{

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName("is-visible")]
		public bool isVisible { get; set; } = true;

		[HtmlAttributeName("language")]
		public string Language { get; set; } = "language-html";

		[HtmlAttributeName("code")]
		public string Code { get; set; } = "<span>code string not set</span>";

		[HtmlAttributeName("is-encoded")]
		public bool IsEncoded { get; set; } = false;
		private class WvCodeHighlightContext
		{
			public bool Initialized { get; set; } = false;
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			if (!String.IsNullOrWhiteSpace(Code))
			{
				if (IsEncoded)
				{
					Code = HttpUtility.HtmlDecode(Code);
				}
			}
			else
			{
				Code = "";
			}

			var prependTemplate = $@"<code class='{Language}'>";

			var appendTemplate = $@"</code>";

			output.TagName = null;
			var preEl = new TagBuilder("pre");
			preEl.InnerHtml.AppendHtml(prependTemplate);
			preEl.InnerHtml.AppendHtml(Code);
			preEl.InnerHtml.AppendHtml(appendTemplate);
			output.PreContent.AppendHtml(preEl);


			var moduleAdded = false;
			if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvCodeHighlight)))
			{
				var tagHelperContext = (WvCodeHighlightContext)ViewContext.HttpContext.Items[typeof(WvCodeHighlight)];
				moduleAdded = tagHelperContext.Initialized;
			}
			if (!moduleAdded)
			{
				var scriptEl = new TagBuilder("script");
				scriptEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/prism/prism.js");
				scriptEl.Attributes.Add("type", "text/javascript");
				output.PostContent.AppendHtml(scriptEl);

				var linkEl = new TagBuilder("link");
				linkEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/prism/prism.css");
				linkEl.Attributes.Add("rel", "stylesheet");
				linkEl.Attributes.Add("type", "text/css");
				output.PostContent.AppendHtml(linkEl);

				ViewContext.HttpContext.Items[typeof(WvCodeHighlight)] = new WvCodeHighlightContext()
				{
					Initialized = true
				};
			}



			return Task.CompletedTask;

		}


	}


}
