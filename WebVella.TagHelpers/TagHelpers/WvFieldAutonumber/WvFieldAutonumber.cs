using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using Yahoo.Yui.Compressor;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-autonumber")]
	public class WvFieldAutonumber : WvFieldBase
	{

		[HtmlAttributeName("template")]
		public string Template { get; set; } = "";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
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
			if (Value == null)
				Value = "";
			
			//Apply Value string
			if (!String.IsNullOrWhiteSpace(Template))
			{
				Value = String.Format(Template, (Value ?? "").ToString());
			}
			else
			{
				Value = (Value ?? "").ToString();
			}

			if (Mode == WvFieldRenderMode.Form || Mode == WvFieldRenderMode.InlineEdit)
			{
				var inputEl = new TagBuilder("input");
				var inputElCssClassList = new List<string>();
				inputElCssClassList.Add("form-control erp-autonumber");
				inputEl.Attributes.Add("type", "text");
				inputEl.Attributes.Add("value", Value);



				inputEl.Attributes.Add("id", $"input-{FieldId}");
				inputEl.Attributes.Add("name", Name);
				if (Required)
				{
					inputEl.Attributes.Add("required", null);
				}
				if (!String.IsNullOrWhiteSpace(Placeholder))
				{
					inputEl.Attributes.Add("placeholder", Placeholder);
				}
				inputEl.Attributes.Add("readonly", null);


				if (ValidationErrors.Count > 0)
				{
					inputElCssClassList.Add("is-invalid");
				}

				inputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

				output.Content.AppendHtml(inputEl);
			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				var divEl = new TagBuilder("div");
				divEl.Attributes.Add("id", $"input-{FieldId}");
				divEl.AddCssClass("form-control-plaintext erp-autonumber");
				divEl.InnerHtml.Append(Value);
				output.Content.AppendHtml(divEl);
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				output.Content.AppendHtml(Value);
				return Task.CompletedTask;
			}
			#endregion

			//Finally
			if (SubInputEl != null)
			{
				output.PostContent.AppendHtml(SubInputEl);
			}
			return Task.CompletedTask;
		}

	}
}
