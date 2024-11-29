using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-autonumber")]
	public class WvFieldAutonumber : WvFieldBase
	{

		[HtmlAttributeName("template")]
		public string Template { get; set; } = "";

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
                var inputGroupEl = new TagBuilder("div");
                inputGroupEl.AddCssClass("input-group");
                //Prepend
                if (PrependHtml.Count > 0)
                {
                    var prependEl = new TagBuilder("span");
                    prependEl.AddCssClass($"input-group-prepend {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
                    foreach (var htmlString in PrependHtml)
                    {
                        prependEl.InnerHtml.AppendHtml(htmlString);
                    }
                    inputGroupEl.InnerHtml.AppendHtml(prependEl);
                }

                var divEl = new TagBuilder("div");
				divEl.Attributes.Add("id", $"input-{FieldId}");
				divEl.AddCssClass("form-control-plaintext erp-autonumber");
				divEl.InnerHtml.Append(templateValue);
                inputGroupEl.InnerHtml.AppendHtml(divEl);
                
				//Append
                if (AppendHtml.Count > 0)
                {
                    var appendEl = new TagBuilder("span");
                    appendEl.AddCssClass($"input-group-append {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");

                    foreach (var htmlString in AppendHtml)
                    {
                        appendEl.InnerHtml.AppendHtml(htmlString);
                    }
                    inputGroupEl.InnerHtml.AppendHtml(appendEl);
                }
                output.Content.AppendHtml(inputGroupEl);
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
