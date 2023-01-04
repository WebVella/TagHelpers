﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{

	[HtmlTargetElement("wv-field-color")]
	public class WvFieldColor : WvFieldBase
	{
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
			if (Mode == WvFieldRenderMode.Form)
			{
				var wrapperEl = new TagBuilder("div");
				wrapperEl.AddCssClass("erp-color");
				var inputEl = new TagBuilder("input");
				var inputElCssClassList = new List<string>();
				inputElCssClassList.Add("d-none");

				inputEl.Attributes.Add("type", "text");
				inputEl.Attributes.Add("value", (Value ?? "").ToString());
				inputEl.Attributes.Add("id", $"input-{FieldId}");
				inputEl.Attributes.Add("name", Name);

				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
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

				wrapperEl.InnerHtml.AppendHtml(inputEl);

				output.Content.AppendHtml(wrapperEl);

				#region << Init Libraries >>
				var wvLibraryInitialized = false;
				var libraryItemsKey = "WebVella-" + "spectrum";
				if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
					wvLibraryInitialized = tagHelperContext.Initialized;
				}

				if (!wvLibraryInitialized)
				{
					var libCssEl = new TagBuilder("link");
					libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/spectrum/spectrum.min.css");
					libCssEl.Attributes.Add("type", "text/css");
					libCssEl.Attributes.Add("rel", "stylesheet");
					output.PostContent.AppendHtml(libCssEl);
					output.PostContent.AppendHtml("\r\n\t");

					var libJsEl = new TagBuilder("script");
					libJsEl.Attributes.Add("type", "text/javascript");
					libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/spectrum/spectrum.min.js");
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
				var fileName = "form.js";
				if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldColor) + fileName))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldColor) + fileName];
					tagHelperInitialized = tagHelperContext.Initialized;
				}
				if (!tagHelperInitialized)
				{
					var scriptContent = WvHelpers.GetEmbeddedTextResource(fileName, "WebVella.TagHelpers.TagHelpers.WvFieldColor", "WebVella.TagHelpers");
					var scriptEl = new TagBuilder("script");
					scriptEl.Attributes.Add("type", "text/javascript");
					//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
					scriptEl.InnerHtml.AppendHtml(scriptContent);
					output.PostContent.AppendHtml(scriptEl);

					ViewContext.HttpContext.Items[typeof(WvFieldColor) + fileName] = new WvTagHelperContext()
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
							ColorFormInit(""{{FieldId}}"");
						});";
				scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));

				initScript.InnerHtml.AppendHtml(scriptTemplate);

				output.PostContent.AppendHtml(initScript);
				#endregion

			}
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (!String.IsNullOrWhiteSpace(Value))
				{
					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-color");
					var colorDiv = new TagBuilder("div");
					colorDiv.AddCssClass("color-box");
					colorDiv.Attributes.Add("style", $"background-color:{(Value ?? "").ToString()}");
					divEl.InnerHtml.AppendHtml(colorDiv);
					divEl.InnerHtml.Append((Value ?? "").ToString());
					output.Content.AppendHtml(divEl);
				}
				else
				{
					output.Content.AppendHtml(EmptyValEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				output.Content.Append((Value ?? "").ToString());
				return Task.CompletedTask;
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

						var viewInputPrepend = new TagBuilder("span");
						viewInputPrepend.AddCssClass("input-group-prepend");
						var viewInputPrependText = new TagBuilder("span");
						viewInputPrependText.AddCssClass("input-group-text");
						viewInputPrependText.InnerHtml.AppendHtml($"<i class='fas fa-fw fa-square' style='color:{(Value ?? "#ffffff").ToString()}'></i>");
						viewInputPrepend.InnerHtml.AppendHtml(viewInputPrependText);
						viewWrapperEl.InnerHtml.AppendHtml(viewInputPrepend);

						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass("form-control erp-color");
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

						var editInputEl = new TagBuilder("input");
						editInputEl.AddCssClass("form-control erp-color");
						editInputEl.Attributes.Add("id", $"input-{FieldId}");
						editInputEl.Attributes.Add("type", "text");
						editInputEl.Attributes.Add("value", (Value ?? "").ToString());
						editInputGroupEl.InnerHtml.AppendHtml(editInputEl);

						var editInputGroupAppendEl = new TagBuilder("span");
						editInputGroupAppendEl.AddCssClass("input-group-append");

						var editSaveBtnEl = new TagBuilder("button");
						editSaveBtnEl.Attributes.Add("type", "button");
						editSaveBtnEl.AddCssClass("btn btn-white save");
						editSaveBtnEl.Attributes.Add("title", "save");

						var editSaveIconEl = new TagBuilder("span");
						editSaveIconEl.AddCssClass("fa fa-fw fa-check go-green");
						editSaveBtnEl.InnerHtml.AppendHtml(editSaveIconEl);
						editInputGroupAppendEl.InnerHtml.AppendHtml(editSaveBtnEl);

						var editCancelBtnEl = new TagBuilder("button");
						editCancelBtnEl.Attributes.Add("type", "button");
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

					#region << Init Libraries >>
					var wvLibraryInitialized = false;
					var libraryItemsKey = "WebVella-" + "spectrum";
					if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
						wvLibraryInitialized = tagHelperContext.Initialized;
					}

					if (!wvLibraryInitialized)
					{
						var libCssEl = new TagBuilder("link");
						libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/spectrum/spectrum.min.css");
						libCssEl.Attributes.Add("type", "text/css");
						libCssEl.Attributes.Add("rel", "stylesheet");
						output.PostContent.AppendHtml(libCssEl);
						output.PostContent.AppendHtml("\r\n\t");

						var libJsEl = new TagBuilder("script");
						libJsEl.Attributes.Add("type", "text/javascript");
						libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/spectrum/spectrum.min.js");
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
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldColor) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldColor) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldColor", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldColor) + "-inline-edit"] = new WvTagHelperContext()
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
							ColorInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldColorConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false
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

					var inputEl = new TagBuilder("input");
					inputEl.AddCssClass("form-control erp-color");
					inputEl.Attributes.Add("type", "color");
					inputEl.Attributes.Add("value", (Value ?? "").ToString());
					inputEl.Attributes.Add("readonly", null);

					var appendActionSpan = new TagBuilder("span");
					appendActionSpan.AddCssClass("input-group-append");
					appendActionSpan.AddCssClass("action");

					var appendTextSpan = new TagBuilder("span");
					appendTextSpan.AddCssClass("input-group-text");

					var appendIconSpan = new TagBuilder("span");
					appendIconSpan.AddCssClass("fa fa-fw fa-lock");

					appendTextSpan.InnerHtml.AppendHtml(appendIconSpan);

					appendActionSpan.InnerHtml.AppendHtml(appendTextSpan);

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

			return Task.CompletedTask;
		}


	}
}
