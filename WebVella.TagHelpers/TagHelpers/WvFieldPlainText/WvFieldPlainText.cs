using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;


namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-plaintext")]
	public class WvFieldPlaintext : WvFieldBase
	{
		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}
			#region << Init >>
			var initSuccess = InitField(context, output);

			if (!initSuccess)
			{
				return Task.CompletedTask;
			}

			#endregion

			#region << Render >>
			if (!String.IsNullOrWhiteSpace(Value))
			{
				var divEl = new TagBuilder("div");
				divEl.Attributes.Add("id", $"input-{FieldId}");
				divEl.AddCssClass("form-control-plaintext erp-plain-text");
				divEl.InnerHtml.AppendHtml(Value);
				output.Content.AppendHtml(divEl);
			}
			else
			{
				output.Content.AppendHtml(EmptyValEl);
			}

			#endregion

			return Task.CompletedTask;
		}
	}
}
