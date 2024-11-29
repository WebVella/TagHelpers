using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-code")]
	public class WvFieldCode : WvFieldBase
	{
		[HtmlAttributeName("height")]
		public string Height { get; set; } = "160px";

		[HtmlAttributeName("language")]
		public string Language { get; set; } = "razor";

		[HtmlAttributeName("theme")]
		public string Theme { get; set; } = "cobalt";

		[HtmlAttributeName("is-encoded")]
		public bool IsEncoded { get; set; } = false;

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
				if (IsEncoded)
				{
					Value = HttpUtility.HtmlDecode(Value);
				}
			}
			else
			{
				Value = "";
			}


			var editorWrapperEl = new TagBuilder("div");
			var wrapperCssClassList = new List<string>();
			wrapperCssClassList.Add("form-control-plaintext erp-code");
			if (ValidationErrors.Count > 0)
			{
				wrapperCssClassList.Add("is-invalid");
			}
			editorWrapperEl.Attributes.Add("class", String.Join(' ', wrapperCssClassList));

			var editorWrapper = new TagBuilder("div");
			editorWrapper.Attributes.Add("id", $"ace-{FieldId}");
			editorWrapper.Attributes.Add("style", $"height:{Height}");
			editorWrapperEl.InnerHtml.AppendHtml(editorWrapper);

			var hiddenInput = new TagBuilder("input");
			hiddenInput.Attributes.Add("type", "hidden");
			hiddenInput.Attributes.Add("id", $"input-{FieldId}");
			hiddenInput.Attributes.Add("name", Name);
			hiddenInput.Attributes.Add("value", Value.ToString());
			editorWrapperEl.InnerHtml.AppendHtml(hiddenInput);

			output.Content.AppendHtml(editorWrapperEl);


			#region << Add Ace lib >>
			{
				var tagHelperInitialized = false;
				var fileName = "ace.js";
				if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldCode) + fileName))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldCode) + fileName];
					tagHelperInitialized = tagHelperContext.Initialized;
				}
				if (!tagHelperInitialized)
				{
					var scriptEl = new TagBuilder("script");
					scriptEl.Attributes.Add("type", "text/javascript");
					scriptEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/ace/ace.js");
					output.PostContent.AppendHtml(scriptEl);

					ViewContext.HttpContext.Items[typeof(WvFieldCode) + fileName] = new WvTagHelperContext()
					{
						Initialized = true
					};

				}
			}
			#endregion

			#region << Init Ace script >>
			{
				var tagHelperInitialized = false;
				var fileName = "form.js";
				if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldCode) + fileName))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldCode) + fileName];
					tagHelperInitialized = tagHelperContext.Initialized;
				}
				if (!tagHelperInitialized)
				{
					var scriptContent = WvHelpers.GetEmbeddedTextResource(fileName, "WebVella.TagHelpers.TagHelpers.WvFieldCode", "WebVella.TagHelpers");
					var scriptEl = new TagBuilder("script");
					scriptEl.Attributes.Add("type", "text/javascript");
					scriptEl.InnerHtml.AppendHtml(scriptContent);
					//scriptEl.InnerHtml.AppendHtml(scriptContent);
					output.PostContent.AppendHtml(scriptEl);

					ViewContext.HttpContext.Items[typeof(WvFieldCode) + fileName] = new WvTagHelperContext()
					{
						Initialized = true
					};

				}
			}
			#endregion

			#region << Add Inline Init Script for this instance >>
			var initScript = new TagBuilder("script");
			initScript.Attributes.Add("type", "text/javascript");
			var scriptTemplate = @"
					$(function(){
						CodeFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
					});";

			scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
			scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

			var fieldConfig = new WvFieldCodeConfig()
			{
				ApiUrl = ApiUrl,
				CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
				Language = Language,
				Theme = Theme,
				ReadOnly = Mode == WvFieldRenderMode.Display
			};
			scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

			initScript.InnerHtml.AppendHtml(scriptTemplate);

			output.PostContent.AppendHtml(initScript);
			#endregion


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
