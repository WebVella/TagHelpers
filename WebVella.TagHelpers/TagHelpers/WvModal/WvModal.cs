using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{

	//[OutputElementHint("div")]
	[HtmlTargetElement("wv-modal")]
	public class WvModal : TagHelper
	{
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName("is-visible")]
		public bool isVisible { get; set; } = true;

		[HtmlAttributeName("id")]
		public string Id { get; set; } = "";

		[HtmlAttributeName("title")]
		public string Title { get; set; } = "";

		[HtmlAttributeName("position")]
		public WvModalPosition Position { get; set; } = WvModalPosition.Top;

		[HtmlAttributeName("size")]
		public WvModalSize Size { get; set; } = WvModalSize.Normal;

		[HtmlAttributeName("backdrop")]
		public string Backdrop { get; set; } = "true";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			output.TagName = "div";
			output.AddCssClass("modal");
			//output.Attributes.Add("tabindex","-1");
			output.Attributes.Add("role", "modal");

			if (!String.IsNullOrWhiteSpace(Id))
			{
				output.Attributes.Add("id", Id);
			}
			else
			{
				output.Attributes.Add("id", "modal-" + Guid.NewGuid());
			}

			if (!String.IsNullOrWhiteSpace(Backdrop) && Backdrop != "true")
			{
				output.Attributes.Add("data-backdrop", Backdrop);
			}


			var modalDialog = new TagBuilder("div");
			modalDialog.AddCssClass("modal-dialog");
			modalDialog.AddCssClass(Position.GetLabel());
			modalDialog.AddCssClass(Size.GetLabel());
			modalDialog.Attributes.Add("role", "document");

			var modalContent = new TagBuilder("div");
			modalContent.AddCssClass("modal-content");

			var modalHeader = new TagBuilder("div");
			modalHeader.AddCssClass("modal-header");
			var modalHeaderTitle = new TagBuilder("h5");
			modalHeaderTitle.AddCssClass("modal-title");
			modalHeaderTitle.InnerHtml.AppendHtml(Title);
			modalHeader.InnerHtml.AppendHtml(modalHeaderTitle);


			output.PreContent.AppendHtml(modalDialog.RenderStartTag());
			output.PreContent.AppendHtml(modalContent.RenderStartTag());

			if (!String.IsNullOrWhiteSpace(Title))
			{
				output.PreContent.AppendHtml(modalHeader);
			}


			output.PostContent.AppendHtml(modalContent.RenderEndTag());
			output.PostContent.AppendHtml(modalDialog.RenderEndTag());

			return Task.CompletedTask;
		}
	}
}
