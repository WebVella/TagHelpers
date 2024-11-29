using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-form")]
	public class WvForm : TagHelper
	{
		public WvForm(IHtmlGenerator generator)
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

		[HtmlAttributeName("antiforgery")]
		public bool Antiforgery { get; set; } = true;

		[HtmlAttributeName("name")]
		public string Name { get; set; } = "form";

		[HtmlAttributeName("action")]
		public string Action { get; set; } = "";

		[HtmlAttributeName("enctype")]
		public string Enctype { get; set; } = "";
		//Specifies how the form-data should be encoded when submitting it to the server (only for method="post")
		//application/x-www-form-urlencoded
		//multipart/form-data
		//text/plain


		[HtmlAttributeName("target")]
		public string Target { get; set; } = "";
		//Specifies where to display the response that is received after submitting the form
		//_blank
		//_self
		//_parent
		//_top

		[HtmlAttributeName("method")]
		public string Method { get; set; } = "post";

		[HtmlAttributeName("novalidate")]
		public bool NoValidate { get; set; } = true;

		[HtmlAttributeName("autocomplete")]
		public bool AutoComplete { get; set; } = true;

		[HtmlAttributeName("accept-charset")]
		public string AcceptCharset { get; set; } = "";

		[HtmlAttributeName("validation-errors")]
		public List<KeyValuePair<string, string>> ValidationErrors { get; set; } = new List<KeyValuePair<string, string>>();

		[HtmlAttributeName("label-mode")]
		public WvLabelRenderMode LabelMode { get; set; } = WvLabelRenderMode.Undefined; //To be inherited

		[HtmlAttributeName("mode")]
		public WvFieldRenderMode Mode { get; set; } = WvFieldRenderMode.Undefined; //To be inherited

		[HtmlAttributeName("size")]
		public WvCssSize Size { get; set; } = WvCssSize.Inherit;

		[HtmlAttributeName("class")]
		public string Class { get; set; } = "";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}
			#region << Init >>

			if (LabelMode == WvLabelRenderMode.Undefined)
			{
				//Check if it is defined in form group
				if (context.Items.ContainsKey(typeof(WvLabelRenderMode)))
				{
					LabelMode = (WvLabelRenderMode)context.Items[typeof(WvLabelRenderMode)];
				}
				else
				{
					LabelMode = WvLabelRenderMode.Stacked;
				}
			}

			if (Mode == WvFieldRenderMode.Undefined)
			{
				//Check if it is defined in form group
				if (context.Items.ContainsKey(typeof(WvFieldRenderMode)))
				{
					Mode = (WvFieldRenderMode)context.Items[typeof(WvFieldRenderMode)];
				}
				else
				{
					Mode = WvFieldRenderMode.Form;
				}
			}

			if (Size == WvCssSize.Inherit)
			{
				//Check if it is defined in form group
				if (context.Items.ContainsKey(typeof(WvCssSize)))
				{
					Size = (WvCssSize)context.Items[typeof(WvCssSize)];
				}
				else
				{
					Size = WvCssSize.Normal;
				}
			}

			#endregion

			if (Mode == WvFieldRenderMode.Form)
			{
				output.TagName = "form";
				if (!String.IsNullOrWhiteSpace(Id))
				{
					output.Attributes.Add("id", Id);
				}

				if (!String.IsNullOrWhiteSpace(Class))
				{
					output.Attributes.Add("class", Class);
				}

				if (!String.IsNullOrWhiteSpace(Name))
				{
					output.Attributes.Add("name", Name);
				}
				if (!String.IsNullOrWhiteSpace(Action))
				{
					output.Attributes.Add("action", Action);
				}

				if (!String.IsNullOrWhiteSpace(Enctype))
				{
					output.Attributes.Add("enctype", Enctype);
				}

				if (!String.IsNullOrWhiteSpace(Target))
				{
					output.Attributes.Add("target", Target);
				}
				switch (Method.ToLowerInvariant())
				{
					case "get":
						output.Attributes.Add("method", Method);
						break;
					default:
						output.Attributes.Add("method", "post");
						break;
				}

				if (NoValidate)
				{
					output.Attributes.Add("novalidate", "novalidate");
				}

				if (AutoComplete)
				{
					output.Attributes.Add("autocomplete", "on");
				}
				else
				{
					output.Attributes.Add("autocomplete", "off");
				}

				if (!String.IsNullOrWhiteSpace(AcceptCharset))
				{
					output.Attributes.Add("accept-charset", AcceptCharset);
				}

				if (Antiforgery && Method.ToLowerInvariant() != "get")
				{
					var antiforgeryTag = Generator.GenerateAntiforgery(ViewContext);
					if (antiforgeryTag != null)
					{
						output.PreContent.AppendHtml(antiforgeryTag);
					}
				}

				if (ValidationErrors.Count > 0)
				{
					context.Items["WvTagHelperValidationErrors"] = ValidationErrors;
				}

			}
			else
			{
				output.TagName = "div";
				output.Attributes.Add("class", "form");
			}

			context.Items[typeof(WvLabelRenderMode)] = LabelMode;
			context.Items[typeof(WvFieldRenderMode)] = Mode;
			context.Items["FromAutocomplete"] = AutoComplete;
			return Task.CompletedTask;
		}
	}
}
