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
	[HtmlTargetElement("wv-field-percent")]
	[RestrictChildren("wv-field-prepend", "wv-field-append")]
	public class WvFieldPercent : WvFieldBase
	{
		[HtmlAttributeName("min")]
		public decimal? Min { get; set; } = null;

		[HtmlAttributeName("max")]
		public decimal? Max { get; set; } = null;

		[HtmlAttributeName("step")]
		public decimal? Step { get; set; } = null;

		[HtmlAttributeName("decimal-digits")]
		public int DecimalDigits { get; set; } = 2;

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

			//Apply currency dec places
			if (Value != null)
			{
				Value = Math.Round(Convert.ToDecimal(Value), DecimalDigits + 2);
			}

			if (Step == null)
			{
				Step = (decimal)0.01;
			}

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
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
					//Control
					var inputEl = new TagBuilder("input");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-percent");
					inputEl.Attributes.Add("type", "number");
					var fakeValueString = "";
					if (Value != null)
					{
						fakeValueString = (((decimal)Value) * 100).ToString("N" + DecimalDigits);
					}
					inputEl.Attributes.Add("value", fakeValueString);
					inputEl.Attributes.Add("id", $"fake-{FieldId}");
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

					var hiddenInputEl = new TagBuilder("input");
					hiddenInputEl.Attributes.Add("name", Name);
					hiddenInputEl.Attributes.Add("id", $"input-{FieldId}");
					hiddenInputEl.Attributes.Add("type", "hidden");
					var hiddenValueString = "";
					if (Value != null)
					{
						hiddenValueString = ((decimal)Value).ToString("N" + (DecimalDigits + 2));
					}
					hiddenInputEl.Attributes.Add("value", hiddenValueString);
					output.Content.AppendHtml(hiddenInputEl);

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldPercent) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldPercent) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldPercent", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldPercent) + "-form"] = new WvTagHelperContext()
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
							PercentFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldPercentConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						DecimalDigits = DecimalDigits
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion
				}
				else if (Access == WvFieldAccess.ReadOnly)
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
					//Control
					var inputEl = new TagBuilder("input");
					inputEl.AddCssClass("form-control erp-percent");
					inputEl.Attributes.Add("type", "number");
					inputEl.Attributes.Add("readonly", null);
					var fakeValueString = "";
					if (Value != null)
					{
						fakeValueString = (((decimal)Value) * 100).ToString("N" + DecimalDigits);
					}
					inputEl.Attributes.Add("value", fakeValueString);
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
			}
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (Value != null)
				{
					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-percent");
					var fakeValueString = "";
					if (Value != null)
					{
						fakeValueString = ((decimal)Value).ToString("P" + DecimalDigits, Culture);
					}
					divEl.InnerHtml.Append(fakeValueString);
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
					var fakeValueString = "";
					if (Value != null)
					{
						fakeValueString = ((decimal)Value).ToString("P" + DecimalDigits, Culture);
					}
					output.Content.Append(fakeValueString);
				}
				else
				{
					output.SuppressOutput();
					output.Content.Append("");
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
						//Control
						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass("form-control erp-percent");
						var fakeValueString = "";
						if (Value != null)
						{
							fakeValueString = (((decimal)Value) * 100).ToString("N" + DecimalDigits);
						}
						viewFormControlEl.InnerHtml.Append(fakeValueString);
						viewWrapperEl.InnerHtml.AppendHtml(viewFormControlEl);

						//Append
						var viewInputActionEl = new TagBuilder("span");
						viewInputActionEl.AddCssClass("input-group-append action");
						foreach (var htmlString in AppendHtml)
						{
							viewInputActionEl.InnerHtml.AppendHtml(htmlString);
						}
						viewInputActionEl.InnerHtml.AppendHtml("<button type=\"button\" class='btn btn-white' title='edit'><i class='fa fa-fw fa-pencil-alt'></i></button>");
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
						//Control
						var editInputEl = new TagBuilder("input");
						editInputEl.AddCssClass("form-control erp-percent");
						editInputEl.Attributes.Add("id", $"fake-{FieldId}");
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
						var fakeValueString = "";
						if (Value != null)
						{
							fakeValueString = (((decimal)Value) * 100).ToString("N" + DecimalDigits);
						}
						editInputEl.Attributes.Add("value", fakeValueString);
						editInputGroupEl.InnerHtml.AppendHtml(editInputEl);

						//Append
						var editInputGroupAppendEl = new TagBuilder("span");
						editInputGroupAppendEl.AddCssClass("input-group-append");

						foreach (var htmlString in AppendHtml)
						{
							editInputGroupAppendEl.InnerHtml.AppendHtml(htmlString);
						}
						editInputGroupAppendEl.InnerHtml.AppendHtml("<button type=\"button\" class='btn btn-white save' title='save'><i class='fa fa-fw fa-check go-green'></i></button>");
						editInputGroupAppendEl.InnerHtml.AppendHtml("<button type=\"button\" class='btn btn-white cancel' title='cancel'><i class='fa fa-fw fa-times go-gray'></i></button>");
						editInputGroupEl.InnerHtml.AppendHtml(editInputGroupAppendEl);

						editWrapperEl.InnerHtml.AppendHtml(editInputGroupEl);

						var hiddenInputEl = new TagBuilder("input");
						hiddenInputEl.Attributes.Add("id", $"input-{FieldId}");
						hiddenInputEl.Attributes.Add("type", "hidden");
						var hiddenValueString = "";
						if (Value != null)
						{
							hiddenValueString = ((decimal)Value).ToString("N" + (DecimalDigits + 2));
						}
						hiddenInputEl.Attributes.Add("value", hiddenValueString);
						hiddenInputEl.Attributes.Add("data-original-value", hiddenValueString);
						editWrapperEl.InnerHtml.AppendHtml(hiddenInputEl);

						output.Content.AppendHtml(editWrapperEl);
					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldPercent) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldPercent) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldPercent", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldPercent) + "-inline-edit"] = new WvTagHelperContext()
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
							PercentInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldPercentConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						DecimalDigits = DecimalDigits
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

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
					//Control
					var inputEl = new TagBuilder("input");
					inputEl.AddCssClass("form-control erp-percent");
					inputEl.Attributes.Add("type", "text");
					var fakeValueString = "";
					if (Value != null)
					{
						fakeValueString = (((decimal)Value) * 100).ToString("N" + DecimalDigits);
					}
					inputEl.Attributes.Add("value", fakeValueString);
					inputEl.Attributes.Add("readonly", null);
					divEl.InnerHtml.AppendHtml(inputEl);
					//Append
					var appendActionSpan = new TagBuilder("span");
					appendActionSpan.AddCssClass("input-group-append");
					foreach (var htmlString in AppendHtml)
					{
						appendActionSpan.InnerHtml.AppendHtml(htmlString);
					}
					appendActionSpan.InnerHtml.AppendHtml("<button type=\"button\" disabled class='btn btn-white' title='locked'><i class='fa fa-fw fa-lock'></i></button>");
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
