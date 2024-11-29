﻿using HtmlAgilityPack;
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
	[HtmlTargetElement("wv-field-date")]
	[RestrictChildren("wv-field-prepend", "wv-field-append")]
	public class WvFieldDate : WvFieldBase
	{
		[HtmlAttributeName("timezone-name")]
		public string TimezoneName { get; set; } = "FLE Standard Time";

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
			#endregion

			#region << Render >>

			if (Mode == WvFieldRenderMode.Form)
			{
				var inputReadOnlyValue = "";
				var inputEditValue = "";
				if (Value != null)
				{
					inputReadOnlyValue = (Value as DateTime? ?? (DateTime?)DateTime.Now).WvClearKind().Value.ToString("dd MMM yyyy", Culture);
					inputEditValue = (Value ?? DateTime.Now).ToString("yyyy-MM-ddT00:00:00.000");
				}


				var inputGroupEl = new TagBuilder("div");
				inputGroupEl.AddCssClass("input-group");

                if (Size == WvCssSize.Large)
                    inputGroupEl.AddCssClass("input-group-lg");
                if (Size == WvCssSize.Small)
                    inputGroupEl.AddCssClass("input-group-sm");

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
				inputElCssClassList.Add("form-control erp-date");

				inputEl.Attributes.Add("type", "text");
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

					inputEl.Attributes.Add("value", inputEditValue);
				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					inputEl.Attributes.Add("value", inputReadOnlyValue);
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

				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					#region << Init Libraries >>
					var wvLibraryInitialized = false;
					var libraryItemsKey = "WebVella-" + "flatpickr";
					if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
						wvLibraryInitialized = tagHelperContext.Initialized;
					}

					if (!wvLibraryInitialized)
					{
						var libCssEl = new TagBuilder("link");
						libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/flatpickr/flatpickr.min.css");
						libCssEl.Attributes.Add("type", "text/css");
						libCssEl.Attributes.Add("rel", "stylesheet");
						output.PostContent.AppendHtml(libCssEl);
						output.PostContent.AppendHtml("\r\n\t");

						var libJsEl = new TagBuilder("script");
						libJsEl.Attributes.Add("type", "text/javascript");
						libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/flatpickr/flatpickr.min.js");
						output.PostContent.AppendHtml(libJsEl);
						output.PostContent.AppendHtml("\r\n\t");

						ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
						{
							Initialized = true
						};
					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldDate) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldDate) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldDate", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldDate) + "-form"] = new WvTagHelperContext()
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
							InitFlatPickrDate(""{{FieldId}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));

					var fieldConfig = new WvFieldDateConfig()
					{
						IsRequired = Required
					};
					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion
				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				if (Value != null)
				{
					var inputReadOnlyValue = (Value as DateTime? ?? (DateTime?)DateTime.Now).WvClearKind().Value.ToString("dd MMM yyyy", Culture);

					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-date");
                    if (Size == WvCssSize.Large)
                        divEl.AddCssClass("form-control-lg");
                    if (Size == WvCssSize.Small)
                        divEl.AddCssClass("form-control-sm");
                    divEl.InnerHtml.Append(inputReadOnlyValue);
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
					var inputReadOnlyValue = (Value as DateTime? ?? (DateTime?)DateTime.Now).WvClearKind().Value.ToString("dd MMM yyyy", Culture);
					output.Content.AppendHtml(inputReadOnlyValue);
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

					var editInpuValue = "";
					var readOnlyInputValue = "";
					if (Value != null)
					{
						readOnlyInputValue = (Value as DateTime? ?? (DateTime?)DateTime.Now).WvClearKind().Value.ToString("dd MMM yyyy", Culture);
						editInpuValue = (Value ?? DateTime.Now).ToString("yyyy-MM-ddT00:00:00.000");
					}

					#region << View Wrapper >>
					{
						var viewWrapperEl = new TagBuilder("div");
						viewWrapperEl.AddCssClass("input-group view-wrapper");
						viewWrapperEl.Attributes.Add("title", "double click to edit");
						viewWrapperEl.Attributes.Add("id", $"view-{FieldId}");
                        if (Size == WvCssSize.Large)
                            viewWrapperEl.AddCssClass("input-group-lg");
                        if (Size == WvCssSize.Small)
                            viewWrapperEl.AddCssClass("input-group-sm");
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
						viewFormControlEl.AddCssClass("form-control erp-date");
						viewFormControlEl.InnerHtml.Append(readOnlyInputValue);
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
						editWrapperEl.Attributes.Add("data-default-date", editInpuValue); //There is a problem with double initializing flatpickr which this solves
						editWrapperEl.AddCssClass("edit-wrapper");

						var editInputGroupEl = new TagBuilder("div");
						editInputGroupEl.AddCssClass("input-group");
                        if (Size == WvCssSize.Large)
                            editInputGroupEl.AddCssClass("input-group-lg");
                        if (Size == WvCssSize.Small)
                            editInputGroupEl.AddCssClass("input-group-sm");
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
						editInputEl.AddCssClass("form-control erp-date");
						editInputEl.Attributes.Add("type", "text");
						if (Required)
						{
							editInputEl.Attributes.Add("required", null);
						}
						if (!String.IsNullOrWhiteSpace(Placeholder))
						{
							editInputEl.Attributes.Add("placeholder", Placeholder);
						}

						editInputEl.Attributes.Add("value", editInpuValue);
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

						output.Content.AppendHtml(editWrapperEl);
					}
					#endregion

					#region << Init Libraries >>
					var wvLibraryInitialized = false;
					var libraryItemsKey = "WebVella-" + "flatpickr";
					if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
						wvLibraryInitialized = tagHelperContext.Initialized;
					}

					if (!wvLibraryInitialized)
					{
						var libCssEl = new TagBuilder("link");
						libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/flatpickr/flatpickr.min.css");
						libCssEl.Attributes.Add("type", "text/css");
						libCssEl.Attributes.Add("rel", "stylesheet");
						output.PostContent.AppendHtml(libCssEl);
						output.PostContent.AppendHtml("\r\n\t");

						var libJsEl = new TagBuilder("script");
						libJsEl.Attributes.Add("type", "text/javascript");
						libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/flatpickr/flatpickr.min.js");
						output.PostContent.AppendHtml(libJsEl);
						output.PostContent.AppendHtml("\r\n\t");

						ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
						{
							Initialized = true
						};
					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldDate) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldDate) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldDate", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldDate) + "-inline-edit"] = new WvTagHelperContext()
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
							DateInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldDateConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						IsRequired = Required
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion
				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					var readOnlyInputValue = "";
					if (Value != null)
					{
						readOnlyInputValue = (Value as DateTime? ?? (DateTime?)DateTime.Now).WvClearKind().Value.ToString("dd MMM yyyy", Culture);
					}

					var divEl = new TagBuilder("div");
					divEl.AddCssClass("input-group");
                    if (Size == WvCssSize.Large)
                        divEl.AddCssClass("input-group-lg");
                    if (Size == WvCssSize.Small)
                        divEl.AddCssClass("input-group-sm");
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
					inputEl.AddCssClass("form-control erp-date");
					inputEl.Attributes.Add("type", "text");
					inputEl.Attributes.Add("value", readOnlyInputValue);
					inputEl.Attributes.Add("readonly", null);
					divEl.InnerHtml.AppendHtml(inputEl);
					//Append
					var appendActionSpan = new TagBuilder("span");
					appendActionSpan.AddCssClass("input-group-append action");
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
