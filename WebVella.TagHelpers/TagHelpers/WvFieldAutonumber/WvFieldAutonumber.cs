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
			var templateValue = (Value ?? "").ToString();
			if (!String.IsNullOrWhiteSpace(Template))
			{
				templateValue = String.Format(Template, (Value ?? "").ToString());
			}

			if (Mode == WvFieldRenderMode.Form || Mode == WvFieldRenderMode.InlineEdit)
			{
				//Hidden -> with the real value
				var hiddenInputEl = new TagBuilder("input");
				hiddenInputEl.Attributes.Add("type", "hidden");
				hiddenInputEl.Attributes.Add("value", (Value ?? "").ToString());
				hiddenInputEl.Attributes.Add("id", $"input-{FieldId}");
				hiddenInputEl.Attributes.Add("name", Name);
				output.Content.AppendHtml(hiddenInputEl);

				var inputEl = new TagBuilder("input");
				var inputElCssClassList = new List<string>();
				inputElCssClassList.Add("form-control erp-autonumber");
				inputEl.Attributes.Add("type", "text");
				inputEl.Attributes.Add("value", templateValue);
				inputEl.Attributes.Add("id", $"fake-{FieldId}");
				
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
				divEl.InnerHtml.Append(templateValue);
				output.Content.AppendHtml(divEl);
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				output.Content.AppendHtml(templateValue);
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
