using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-image-card")]
	public class WvImageCard : TagHelper
	{
		public WvImageCard(IHtmlGenerator generator)
		{
			Generator = generator;
		}

		protected IHtmlGenerator Generator { get; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName("is-visible")]
		public bool isVisible { get; set; } = true;

		[HtmlAttributeName("id")]
		public string Id { get; set; } = "";

		[HtmlAttributeName("title")]
		public string Title { get; set; } = "";

		[HtmlAttributeName("description")]
		public string Description { get; set; } = "";

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";

		[HtmlAttributeName("image-url")]
		public string ImageUrl { get; set; } = "";

		[HtmlAttributeName("is-card")]
		public bool IsCard { get; set; } = true;

		[HtmlAttributeName("is-image-right")]
		public bool IsImageRight { get; set; } = false;

		[HtmlAttributeName("has-shadow")]
		public bool HasShadow { get; set; } = true;

		[HtmlAttributeName("is-clickable")]
		public bool IsClickable { get; set; } = true;

		[HtmlAttributeName("body-link")]
		public string BodyLink { get; set; } = "";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
			}
			else
			{
				var embeddedContent = await output.GetChildContentAsync();

				output.TagName = "div";
				output.AddCssClass("image-card-body");
				if (!IsCard)
				{
					output.AddCssClass(Class);
				}


				#region << Init wrappers >>
				var cardWrapperEl = new TagBuilder("div");
				cardWrapperEl.AddCssClass("card image-card");
				if (IsClickable)
				{
					cardWrapperEl.AddCssClass("clickable");
				}

				cardWrapperEl.AddCssClass(Class);
				if (HasShadow)
				{
					cardWrapperEl.AddCssClass("shadow-sm");
				}
				if (!String.IsNullOrWhiteSpace(Id))
					cardWrapperEl.Attributes.Add("id", Id);


				var cardBodyWrapperEl = new TagBuilder("div");
				cardBodyWrapperEl.AddCssClass("card-body p-2");

				if (IsCard)
				{
					output.PreElement.AppendHtml(cardWrapperEl.RenderStartTag());
					output.PreElement.AppendHtml(cardBodyWrapperEl.RenderStartTag());

					output.PostElement.AppendHtml(cardBodyWrapperEl.RenderEndTag());

					output.PostElement.AppendHtml(embeddedContent);
					output.PostElement.AppendHtml(cardWrapperEl.RenderEndTag());
				}

				#endregion


				var ImageEl = new TagBuilder("img");
				ImageEl.AddCssClass("image");
				ImageEl.Attributes.Add("src",ImageUrl);

				if(!IsImageRight)
					output.Content.AppendHtml(ImageEl);

				var metaEl = new TagBuilder("div");
				metaEl.AddCssClass("meta");

				var metaTitleEl = new TagBuilder("div");
				metaTitleEl.AddCssClass("title");
				metaTitleEl.InnerHtml.AppendHtml(Title);
				metaEl.InnerHtml.AppendHtml(metaTitleEl);

				var metaDescriptionEl = new TagBuilder("div");
				metaDescriptionEl.AddCssClass("description");
				metaDescriptionEl.InnerHtml.AppendHtml(Description);
				metaEl.InnerHtml.AppendHtml(metaDescriptionEl);

				output.Content.AppendHtml(metaEl);

				if(IsImageRight)
					output.Content.AppendHtml(ImageEl);

				if (!String.IsNullOrWhiteSpace(BodyLink))
				{
					var linkEl = new TagBuilder("a");
					linkEl.AddCssClass("link");
					linkEl.Attributes.Add("href", BodyLink);
					linkEl.InnerHtml.AppendHtml("<em></em>");
					output.Content.AppendHtml(linkEl);
				}
			}
		}
	}
}
