using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-currency")]
	public class WvFieldCurrency : WvFieldBase
	{
		[HtmlAttributeName("currency-code")]
		public string CurrencyCode { get; set; } = "USD";

		[HtmlAttributeName("min")]
		public decimal? Min { get; set; } = null;

		[HtmlAttributeName("max")]
		public decimal? Max { get; set; } = null;

		[HtmlAttributeName("step")]
		public decimal? Step { get; set; } = null;

		private WvCurrencyType Currency { get; set; } = null;

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return;
			}
			#region << Init >>
			var initSuccess = InitField(context, output);

			if (!initSuccess)
			{
				return;
			}

			if (Currency == null)
			{
				Currency = WvHelpers.GetCurrencyType(CurrencyCode);
				if (Currency == null)
					throw new Exception($"Currency with code '{CurrencyCode}' is not found.");
			}
			//Apply currency decimal places
			if (Value != null)
			{
				var decimalPlaces = Convert.ToInt32(Currency.DecimalDigits);
				Value = Math.Round(Convert.ToDecimal(Value), decimalPlaces);
			}

			#endregion

			#region << Init Prepend and Append >>
			var content = await output.GetChildContentAsync();
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(content.GetContent());
			var prependTaghelper = htmlDoc.DocumentNode.Descendants("wv-field-prepend");
			var appendTagHelper = htmlDoc.DocumentNode.Descendants("wv-field-append");

			foreach (var node in prependTaghelper)
			{
				PrependHtml.Add(node.InnerHtml.ToString());
			}

			foreach (var node in appendTagHelper)
			{
				AppendHtml.Add(node.InnerHtml.ToString());
			}

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
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

				var inputEl = new TagBuilder("input");
				var inputElCssClassList = new List<string>();
				inputElCssClassList.Add("form-control erp-currency");

				inputEl.Attributes.Add("type", "number");
				inputEl.Attributes.Add("value", (Value ?? "").ToString());
				inputEl.Attributes.Add("id", $"input-{FieldId}");
				inputEl.Attributes.Add("name", Name);

				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					if (Required)
					{
						inputEl.Attributes.Add("required", null);
					}
					if (!String.IsNullOrWhiteSpace(Placeholder))
					{
						inputEl.Attributes.Add("placeholder", Placeholder);
					}
					if (Min != null)
					{
						inputEl.Attributes.Add("min", Min.ToString());
					}
					if (Max != null)
					{
						inputEl.Attributes.Add("max", Max.ToString());
					}
					if (Step != null)
					{
						inputEl.Attributes.Add("step", Step.ToString());
					}
				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					inputEl.Attributes.Add("readonly", null);
				}

				if (ValidationErrors.Count > 0)
				{
					inputElCssClassList.Add("is-invalid");
				}

				inputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

				inputGroupEl.InnerHtml.AppendHtml(inputEl);

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
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (Value != null)
				{
					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-currency");
					var valueString = Value.ToString("N" + Currency.DecimalDigits, Culture);
					if (Currency.SymbolPlacement == CurrencySymbolPlacement.After)
					{
						divEl.InnerHtml.Append($"{valueString} {Currency.SymbolNative}");
					}
					else
					{
						divEl.InnerHtml.Append($"{Currency.SymbolNative} {valueString}");
					}
					output.Content.AppendHtml(divEl);
				}
				else
				{
					output.Content.AppendHtml(EmptyValEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				if (Value != null)
				{
					output.SuppressOutput();
					var valueString = Value.ToString("N" + Currency.DecimalDigits, Culture);
					if (Currency.SymbolPlacement == CurrencySymbolPlacement.After)
					{
						output.Content.AppendHtml($"{valueString} {Currency.SymbolNative}");
					}
					else
					{
						output.Content.AppendHtml($"{Currency.SymbolNative} {valueString}");
					}
				}
				else
				{
					output.SuppressOutput();
					output.Content.AppendHtml("");
				}
				return;
			}
			else if (Mode == WvFieldRenderMode.InlineEdit)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					#region << View Wrapper >>
					{
						var viewWrapperEl = new TagBuilder("div");
						viewWrapperEl.AddCssClass("input-group view-wrapper");
						viewWrapperEl.Attributes.Add("title", "double click to edit");
						viewWrapperEl.Attributes.Add("id", $"view-{FieldId}");

						//Prepend
						if (PrependHtml.Count > 0)
						{
							var viewInputPrepend = new TagBuilder("span");
							viewInputPrepend.AddCssClass("input-group-prepend");
							foreach (var htmlString in PrependHtml)
							{
								viewInputPrepend.InnerHtml.AppendHtml(htmlString);
							}
							viewWrapperEl.InnerHtml.AppendHtml(viewInputPrepend);
						}

						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass("form-control erp-currency");
						viewFormControlEl.InnerHtml.Append((Value ?? "").ToString());
						viewWrapperEl.InnerHtml.AppendHtml(viewFormControlEl);

						var viewInputActionEl = new TagBuilder("span");
						viewInputActionEl.AddCssClass("input-group-append action");
						viewInputActionEl.Attributes.Add("title", "edit");

						var viewInputActionLinkEl = new TagBuilder("button");
						viewInputActionLinkEl.Attributes.Add("type", "button");
						viewInputActionLinkEl.AddCssClass("btn btn-white");

						var viewInputActionIconEl = new TagBuilder("span");
						viewInputActionIconEl.AddCssClass("fa fa-fw fa-pencil-alt");
						viewInputActionLinkEl.InnerHtml.AppendHtml(viewInputActionIconEl);
						viewInputActionEl.InnerHtml.AppendHtml(viewInputActionLinkEl);
						viewWrapperEl.InnerHtml.AppendHtml(viewInputActionEl);

						output.Content.AppendHtml(viewWrapperEl);
					}
					#endregion

					#region << Edit Wrapper>>
					{
						var editWrapperEl = new TagBuilder("div");
						editWrapperEl.Attributes.Add("id", $"edit-{FieldId}");
						editWrapperEl.Attributes.Add("style", $"display:none;");
						editWrapperEl.AddCssClass("edit-wrapper");

						var editInputGroupEl = new TagBuilder("div");
						editInputGroupEl.AddCssClass("input-group");

						//Prepend
						if (PrependHtml.Count > 0)
						{
							var editInputPrepend = new TagBuilder("span");
							editInputPrepend.AddCssClass("input-group-prepend");
							foreach (var htmlString in PrependHtml)
							{
								editInputPrepend.InnerHtml.AppendHtml(htmlString);
							}
							editInputGroupEl.InnerHtml.AppendHtml(editInputPrepend);
						}


						var editInputEl = new TagBuilder("input");
						editInputEl.AddCssClass("form-control erp-currency");
						editInputEl.Attributes.Add("type", "number");
						if (Required)
						{
							editInputEl.Attributes.Add("required", null);
						}
						if (!String.IsNullOrWhiteSpace(Placeholder))
						{
							editInputEl.Attributes.Add("placeholder", Placeholder);
						}
						if (Min != null)
						{
							editInputEl.Attributes.Add("min", Min.ToString());
						}
						if (Max != null)
						{
							editInputEl.Attributes.Add("max", Max.ToString());
						}
						if (Step != null)
						{
							editInputEl.Attributes.Add("step", Step.ToString());
						}

						editInputEl.Attributes.Add("value", (Value ?? "").ToString());
						editInputGroupEl.InnerHtml.AppendHtml(editInputEl);

						var editInputGroupAppendEl = new TagBuilder("span");
						editInputGroupAppendEl.AddCssClass("input-group-append");

						var editSaveBtnEl = new TagBuilder("button");
						editSaveBtnEl.Attributes.Add("type", "submit");
						editSaveBtnEl.AddCssClass("btn btn-white save");
						editSaveBtnEl.Attributes.Add("title", "save");

						var editSaveIconEl = new TagBuilder("span");
						editSaveIconEl.AddCssClass("fa fa-fw fa-check go-green");
						editSaveBtnEl.InnerHtml.AppendHtml(editSaveIconEl);
						editInputGroupAppendEl.InnerHtml.AppendHtml(editSaveBtnEl);

						var editCancelBtnEl = new TagBuilder("button");
						editCancelBtnEl.Attributes.Add("type", "submit");
						editCancelBtnEl.AddCssClass("btn btn-white cancel");
						editCancelBtnEl.Attributes.Add("title", "cancel");

						var editCancelIconEl = new TagBuilder("span");
						editCancelIconEl.AddCssClass("fa fa-fw fa-times go-gray");
						editCancelBtnEl.InnerHtml.AppendHtml(editCancelIconEl);
						editInputGroupAppendEl.InnerHtml.AppendHtml(editCancelBtnEl);

						editInputGroupEl.InnerHtml.AppendHtml(editInputGroupAppendEl);
						editWrapperEl.InnerHtml.AppendHtml(editInputGroupEl);

						output.Content.AppendHtml(editWrapperEl);
					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldCurrency) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldCurrency) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldCurrency", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldCurrency) + "-inline-edit"] = new WvTagHelperContext()
						{
							Initialized = true
						};

					}
					#endregion

					#region << Add Inline Init Script for this instance >>
					var initScript = new TagBuilder("script");
					initScript.Attributes.Add("type", "text/javascript");
					var scriptTemplate = @"
						$(function(){
							CurrencyInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldCurrencyConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					//initScript.InnerHtml.AppendHtml(jsCompressor.Compress(scriptTemplate));
					initScript.InnerHtml.AppendHtml(scriptTemplate);
					output.PostContent.AppendHtml(initScript);
					#endregion
				}
				else if (Access == WvFieldAccess.ReadOnly)
				{

					var divEl = new TagBuilder("div");
					divEl.AddCssClass("input-group");

					//Prepend
					if (PrependHtml.Count > 0)
					{
						var viewInputPrepend = new TagBuilder("span");
						viewInputPrepend.AddCssClass("input-group-prepend");
						foreach (var htmlString in PrependHtml)
						{
							viewInputPrepend.InnerHtml.AppendHtml(htmlString);
						}
						divEl.InnerHtml.AppendHtml(viewInputPrepend);
					}


					var inputEl = new TagBuilder("input");
					inputEl.AddCssClass("form-control erp-currency");
					inputEl.Attributes.Add("type", "text");
					inputEl.Attributes.Add("value", (Value ?? "").ToString());
					inputEl.Attributes.Add("readonly", null);

					//Append
					var appendActionSpan = new TagBuilder("span");
					appendActionSpan.AddCssClass("input-group-append action");
					foreach (var htmlString in AppendHtml)
					{
						appendActionSpan.InnerHtml.AppendHtml(htmlString);
					}

					var appendIconSpan = new TagBuilder("span");
					appendIconSpan.AddCssClass("fa fa-fw fa-lock");

					appendActionSpan.InnerHtml.AppendHtml(appendIconSpan);


					divEl.InnerHtml.AppendHtml(inputEl);
					divEl.InnerHtml.AppendHtml(appendActionSpan);
					output.Content.AppendHtml(divEl);
				}
			}
			#endregion


			//Finally
			if (SubInputEl != null)
			{
				output.PostContent.AppendHtml(SubInputEl);
			}

			return;
		}

	}
}
