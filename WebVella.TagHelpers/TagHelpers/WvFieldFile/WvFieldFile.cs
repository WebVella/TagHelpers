﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-file")]
	public class WvFieldFile : WvFieldBase
	{
		private string FileName { get; set; } = "";

		private string PathTypeIcon { get; set; } = "";

		[HtmlAttributeName("accept")]
		public string Accept { get; set; } = "";

		[HtmlAttributeName("file-upload-api")]
		public string FileUploadApi { get; set; } = "/fs/upload";

		[HtmlAttributeName("src-prefix")]
		public string SrcPrefix { get; set; } = "/fs";

		[HtmlAttributeName("clipboard-support")]
		public bool ClipboardSupport { get; set; } = true;

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

			if (Value != null && !String.IsNullOrWhiteSpace((Value ?? "").ToString()))
			{
				PathTypeIcon = WvHelpers.GetPathTypeIcon(Value);
				FileName = Path.GetFileName(Value);
			}

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					var inputGroupEl = new TagBuilder("div");
					inputGroupEl.AddCssClass("input-group");
					var prependEl = new TagBuilder("span");
					prependEl.AddCssClass($"input-group-prepend icon-addon {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					prependEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
					var prependText = new TagBuilder("span");
					prependText.AddCssClass("input-group-text");
					var prependIcon = new TagBuilder("span");
					prependIcon.AddCssClass($"fa fa-fw type-icon {PathTypeIcon} {(String.IsNullOrWhiteSpace(Value) ? "fa-file" : "")}");
					prependText.InnerHtml.AppendHtml(prependIcon);
					prependEl.InnerHtml.AppendHtml(prependText);
					inputGroupEl.InnerHtml.AppendHtml(prependEl);

					var fakeInputEl = new TagBuilder("div");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-file with-progress ");
					if (String.IsNullOrWhiteSpace(Value))
					{
						inputElCssClassList.Add("erp-file-trigger");
					}
					fakeInputEl.Attributes.Add("id", $"fake-{FieldId}");

					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}

					fakeInputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));
					fakeInputEl.Attributes.Add("data-file-upload-api", FileUploadApi);

					var fakeInputFileLinkEl = new TagBuilder("a");
					fakeInputFileLinkEl.Attributes.Add("href", $"{SrcPrefix}{Value}");
					fakeInputFileLinkEl.Attributes.Add("target", "_blank");
					fakeInputFileLinkEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
					fakeInputFileLinkEl.InnerHtml.Append(FileName);
					fakeInputEl.InnerHtml.AppendHtml(fakeInputFileLinkEl);
					var fakeInputProgress = new TagBuilder("div");
					fakeInputProgress.AddCssClass("form-control-progress");
					fakeInputEl.InnerHtml.AppendHtml(fakeInputProgress);
					inputGroupEl.InnerHtml.AppendHtml(fakeInputEl);

					var appendEl = new TagBuilder("span");
					appendEl.AddCssClass("input-group-append action erp-file");
					if (!Required)
					{
						var appendDeleteLink = new TagBuilder("button");
						appendDeleteLink.Attributes.Add("type", $"button");
						appendDeleteLink.Attributes.Add("id", $"remove-{FieldId}");
						appendDeleteLink.AddCssClass($"btn btn-white remove {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")}");
						appendDeleteLink.Attributes.Add("title", "select as undefined");
						var appendDeleteLinkIcon = new TagBuilder("span");
						appendDeleteLinkIcon.AddCssClass("fa fa-fw fa-trash go-red");
						appendDeleteLink.InnerHtml.AppendHtml(appendDeleteLinkIcon);
						appendEl.InnerHtml.AppendHtml(appendDeleteLink);
					}
					if (ClipboardSupport)
					{
						var appendPasteLink = new TagBuilder("button");
						appendPasteLink.Attributes.Add("type", $"button");
						appendPasteLink.Attributes.Add("id", $"paste-{FieldId}");
						appendPasteLink.AddCssClass($"btn btn-white");
						appendPasteLink.Attributes.Add("title", "Activate 'Paste Image' from clipboard");
						var appendDeleteLinkIcon = new TagBuilder("span");
						appendDeleteLinkIcon.AddCssClass("icon fa fa-fw fa-paste");
						appendPasteLink.InnerHtml.AppendHtml(appendDeleteLinkIcon);
						appendEl.InnerHtml.AppendHtml(appendPasteLink);
					}
					var selectFileLink = new TagBuilder("button");
					selectFileLink.Attributes.Add("type", $"button");
					selectFileLink.AddCssClass("btn btn-white");
					selectFileLink.Attributes.Add("onclick", $"window.document.getElementById('file-{FieldId}').click();");
					selectFileLink.InnerHtml.AppendHtml("browse");
					appendEl.InnerHtml.AppendHtml(selectFileLink);

					inputGroupEl.InnerHtml.AppendHtml(appendEl);
					output.Content.AppendHtml(inputGroupEl);

					var realHiddenFileInput = new TagBuilder("input");
					realHiddenFileInput.Attributes.Add("id", $"file-{FieldId}");
					realHiddenFileInput.Attributes.Add("type", $"file");
					realHiddenFileInput.AddCssClass("d-none");
					realHiddenFileInput.Attributes.Add("value", $"");
					if (!String.IsNullOrWhiteSpace(Accept))
					{
						realHiddenFileInput.Attributes.Add("accept", $"{Accept}");
					}
					output.Content.AppendHtml(realHiddenFileInput);

					var realSubmitInput = new TagBuilder("input");
					realSubmitInput.Attributes.Add("id", $"input-{FieldId}");
					realSubmitInput.Attributes.Add("type", $"hidden");
					realSubmitInput.Attributes.Add("name", $"{Name}");
					realSubmitInput.Attributes.Add("value", $"{Value}");
					output.Content.AppendHtml(realSubmitInput);

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldFile) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldFile) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldFile", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldFile) + "-form"] = new WvTagHelperContext()
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
							FileFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldFileConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						Accept = Accept,
						FileUploadApi = FileUploadApi,
						SrcPrefix = SrcPrefix
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonConvert.SerializeObject(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion

				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					if (!String.IsNullOrWhiteSpace(Value))
					{
						var inputGroupEl = new TagBuilder("div");
						inputGroupEl.AddCssClass("input-group");
						var prependEl = new TagBuilder("span");
						prependEl.AddCssClass($"input-group-prepend icon-addon {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
						prependEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
						var prependText = new TagBuilder("span");
						prependText.AddCssClass("input-group-text");
						var prependIcon = new TagBuilder("span");
						prependIcon.AddCssClass($"fa fa-fw type-icon {PathTypeIcon} {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")}");
						prependText.InnerHtml.AppendHtml(prependIcon);
						prependEl.InnerHtml.AppendHtml(prependText);
						inputGroupEl.InnerHtml.AppendHtml(prependEl);

						var inputEl = new TagBuilder("div");
						inputEl.AddCssClass("form-control erp-file disabled");
						var inputElLink = new TagBuilder("a");
						inputElLink.Attributes.Add("href", $"{SrcPrefix}{Value}");
						inputElLink.Attributes.Add("target", "_blank");
						inputElLink.Attributes.Add("title", $"{SrcPrefix}{Value}");
						inputElLink.InnerHtml.Append(FileName);
						inputEl.InnerHtml.AppendHtml(inputElLink);
						inputGroupEl.InnerHtml.AppendHtml(inputEl);
						output.Content.AppendHtml(inputGroupEl);

						//Hidden input with the value
						var hiddenInput = new TagBuilder("input");
						hiddenInput.Attributes.Add("type", "hidden");
						hiddenInput.Attributes.Add("id", $"input-{FieldId}");
						hiddenInput.Attributes.Add("name", Name);
						hiddenInput.Attributes.Add("value", (Value ?? "").ToString());
						output.Content.AppendHtml(hiddenInput);
					}
					else
					{
						var inputEl = new TagBuilder("input");
						inputEl.Attributes.Add("readonly", null);
						inputEl.AddCssClass("form-control erp-file");
						inputEl.Attributes.Add("value", "");
						inputEl.Attributes.Add("name", Name);
						output.Content.AppendHtml(inputEl);
					}
				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (!String.IsNullOrWhiteSpace(Value))
				{
					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-file");
					var iconEl = new TagBuilder("span");
					iconEl.AddCssClass($"fa fa-fw {PathTypeIcon}");
					divEl.InnerHtml.AppendHtml(iconEl);
					var linkEl = new TagBuilder("a");
					linkEl.Attributes.Add("href", $"{SrcPrefix}{Value}");
					linkEl.Attributes.Add("target", $"_blank");
					linkEl.InnerHtml.Append(FileName);
					divEl.InnerHtml.AppendHtml(linkEl);
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
				var linkEl = new TagBuilder("a");
				linkEl.Attributes.Add("href", $"{SrcPrefix}{Value}");
				linkEl.Attributes.Add("target", $"_blank");
				linkEl.InnerHtml.Append(FileName);
				output.Content.AppendHtml(linkEl);
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
						viewInputPrepend.AddCssClass($"input-group-prepend icon-addon");
						viewInputPrepend.Attributes.Add("title", $"{SrcPrefix}{Value}");
						var viewInputPrependText = new TagBuilder("span");
						viewInputPrependText.AddCssClass("input-group-text");
						var prependIcon = new TagBuilder("span");
						prependIcon.AddCssClass($"fa fa-fw type-icon {PathTypeIcon} {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")}");
						viewInputPrependText.InnerHtml.AppendHtml(prependIcon);
						viewInputPrepend.InnerHtml.AppendHtml(viewInputPrependText);
						viewWrapperEl.InnerHtml.AppendHtml(viewInputPrepend);

						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass("form-control erp-file");

						var viewFormControlLinkEl = new TagBuilder("a");
						viewFormControlLinkEl.Attributes.Add("href", $"{SrcPrefix}{Value}");
						viewFormControlLinkEl.Attributes.Add("target", "_blank");
						viewFormControlLinkEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
						viewFormControlLinkEl.InnerHtml.Append(FileName);
						viewFormControlEl.InnerHtml.AppendHtml(viewFormControlLinkEl);

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

						var editWrapperPrependEl = new TagBuilder("span");
						editWrapperPrependEl.AddCssClass($"input-group-prepend icon-addon {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
						editWrapperPrependEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
						var editWrapperPrependText = new TagBuilder("span");
						editWrapperPrependText.AddCssClass("input-group-text");
						var editWrapperPrependIcon = new TagBuilder("span");
						editWrapperPrependIcon.AddCssClass($"fa fa-fw type-icon {PathTypeIcon} {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")}");
						editWrapperPrependText.InnerHtml.AppendHtml(editWrapperPrependIcon);
						editWrapperPrependEl.InnerHtml.AppendHtml(editWrapperPrependText);
						editInputGroupEl.InnerHtml.AppendHtml(editWrapperPrependEl);


						var fakeInputEl = new TagBuilder("div");
						var inputElCssClassList = new List<string>();
						inputElCssClassList.Add("form-control erp-file with-progress ");
						if (String.IsNullOrWhiteSpace(Value))
						{
							inputElCssClassList.Add("erp-file-trigger");
						}

						fakeInputEl.Attributes.Add("id", $"fake-{FieldId}");

						if (ValidationErrors.Count > 0)
						{
							inputElCssClassList.Add("is-invalid");
						}

						fakeInputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

						var fakeInputFileLinkEl = new TagBuilder("a");
						fakeInputFileLinkEl.Attributes.Add("href", $"{SrcPrefix}{Value}");
						fakeInputFileLinkEl.Attributes.Add("target", "_blank");
						fakeInputFileLinkEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
						fakeInputFileLinkEl.InnerHtml.Append(FileName);
						fakeInputEl.InnerHtml.AppendHtml(fakeInputFileLinkEl);
						var fakeInputProgress = new TagBuilder("div");
						fakeInputProgress.AddCssClass("form-control-progress");
						fakeInputEl.InnerHtml.AppendHtml(fakeInputProgress);
						editInputGroupEl.InnerHtml.AppendHtml(fakeInputEl);


						var editInputGroupAppendEl = new TagBuilder("span");
						editInputGroupAppendEl.AddCssClass("input-group-append");

						if (!Required)
						{
							var appendDeleteLink = new TagBuilder("button");
							appendDeleteLink.Attributes.Add("type", $"button");
							appendDeleteLink.Attributes.Add("id", $"remove-{FieldId}");
							appendDeleteLink.AddCssClass($"btn btn-white remove {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")}");
							appendDeleteLink.Attributes.Add("title", "select as undefined");
							var appendDeleteLinkIcon = new TagBuilder("span");
							appendDeleteLinkIcon.AddCssClass("fa fa-fw fa-trash go-red");
							appendDeleteLink.InnerHtml.AppendHtml(appendDeleteLinkIcon);
							editInputGroupAppendEl.InnerHtml.AppendHtml(appendDeleteLink);
						}

						var selectFileLink = new TagBuilder("button");
						selectFileLink.Attributes.Add("type", $"button");
						selectFileLink.AddCssClass("btn btn-white");
						selectFileLink.Attributes.Add("onclick", $"window.document.getElementById('file-{FieldId}').click();");
						selectFileLink.InnerHtml.AppendHtml("select");
						editInputGroupAppendEl.InnerHtml.AppendHtml(selectFileLink);


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

						var realHiddenFileInput = new TagBuilder("input");
						realHiddenFileInput.Attributes.Add("id", $"file-{FieldId}");
						realHiddenFileInput.Attributes.Add("type", $"file");
						realHiddenFileInput.AddCssClass("d-none");
						realHiddenFileInput.Attributes.Add("value", $"");
						if (!String.IsNullOrWhiteSpace(Accept))
						{
							realHiddenFileInput.Attributes.Add("accept", $"{Accept}");
						}
						output.Content.AppendHtml(realHiddenFileInput);

						var realSubmitInput = new TagBuilder("input");
						realSubmitInput.Attributes.Add("id", $"input-{FieldId}");
						realSubmitInput.Attributes.Add("type", $"hidden");
						realSubmitInput.Attributes.Add("value", $"{Value}");
						realSubmitInput.Attributes.Add("data-newfilepath", $"{Value}");
						realSubmitInput.Attributes.Add("data-filename", $"{FileName}");
						realSubmitInput.Attributes.Add("data-newfilename", $"{FileName}");
						output.Content.AppendHtml(realSubmitInput);

					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldFile) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldFile) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldFile", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(scriptContent);
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldFile) + "-inline-edit"] = new WvTagHelperContext()
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
							FileInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldFileConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						Accept = Accept,
						FileUploadApi = FileUploadApi,
						SrcPrefix = SrcPrefix
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonConvert.SerializeObject(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion
				}
				else if (Access == WvFieldAccess.ReadOnly)
				{

					var divEl = new TagBuilder("div");
					divEl.AddCssClass("input-group");

					var prependEl = new TagBuilder("span");
					prependEl.AddCssClass($"input-group-prepend icon-addon {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					prependEl.Attributes.Add("title", $"{SrcPrefix}{Value}");
					var prependText = new TagBuilder("span");
					prependText.AddCssClass("input-group-text");
					var prependIcon = new TagBuilder("span");
					prependIcon.AddCssClass($"fa fa-fw type-icon {PathTypeIcon} {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")}");
					prependText.InnerHtml.AppendHtml(prependIcon);
					prependEl.InnerHtml.AppendHtml(prependText);
					divEl.InnerHtml.AppendHtml(prependEl);

					var inputEl = new TagBuilder("div");
					inputEl.AddCssClass("form-control erp-file disabled");
					var inputElLink = new TagBuilder("a");
					inputElLink.Attributes.Add("href", $"{SrcPrefix}{Value}");
					inputElLink.Attributes.Add("target", "_blank");
					inputElLink.Attributes.Add("title", $"{SrcPrefix}{Value}");
					inputElLink.InnerHtml.Append(FileName);
					inputEl.InnerHtml.AppendHtml(inputElLink);
					divEl.InnerHtml.AppendHtml(inputEl);

					var appendActionSpan = new TagBuilder("span");
					appendActionSpan.AddCssClass("input-group-append");
					appendActionSpan.AddCssClass("action");

					var appendTextSpan = new TagBuilder("span");
					appendTextSpan.AddCssClass("input-group-text");

					var appendIconSpan = new TagBuilder("span");
					appendIconSpan.AddCssClass("fa fa-fw fa-lock");
					appendTextSpan.InnerHtml.AppendHtml(appendIconSpan);
					appendActionSpan.InnerHtml.AppendHtml(appendTextSpan);

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
