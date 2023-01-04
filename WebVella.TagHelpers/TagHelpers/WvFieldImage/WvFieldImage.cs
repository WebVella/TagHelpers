﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers.WvFieldImage
{
	[HtmlTargetElement("wv-field-image")]
	public class WvFieldImage : WvFieldBase
	{
		private string FileName { get; set; } = "";

		private string WrapperStyle { get; set; } = "";

		private string ImageQuery { get; set; } = "";

		private List<string> StylesList { get; set; } = new List<string>();

		private List<string> QueryCommandsList { get; set; } = new List<string>();

		[HtmlAttributeName("text-remove")]
		public string TextRemove { get; set; } = "remove";

		[HtmlAttributeName("text-select")]
		public string TextSelect { get; set; } = "select";

		[HtmlAttributeName("accept")]
		public string Accept { get; set; } = "";

		[HtmlAttributeName("width")]
		public int? Width { get; set; } = null;

		[HtmlAttributeName("height")]
		public int? Height { get; set; } = null;

		[HtmlAttributeName("resize-action")]
		public ImageResizeMode ResizeAction { get; set; } = ImageResizeMode.Pad;

		[HtmlAttributeName("file-upload-api")]
		public string FileUploadApi { get; set; } = "/fs/upload";

		[HtmlAttributeName("src-prefix")]
		public string SrcPrefix { get; set; } = "/fs";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{

			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			if (Width == 0)
				Width = null;

			if (Height == 0)
				Height = null;

			#region << Init >>
			var initSuccess = InitField(context, output);

			if (!initSuccess)
			{
				return Task.CompletedTask;
			}

			if (!((string)Value).StartsWith("http"))
			{
				if (Height != null || Width != null)
				{
					QueryCommandsList.Add($"action=resize");
					QueryCommandsList.Add($"mode={ResizeAction}");
				}

				if (Height != null)
				{
					StylesList.Add($"height:{Height}px");
					QueryCommandsList.Add($"height={Height}");
				}

				if (Width != null)
				{
					StylesList.Add($"width:{Width}px");
					QueryCommandsList.Add($"width={Width}");
				}

			}

			WrapperStyle = String.Join(";", StylesList);
			ImageQuery = "";
			if (QueryCommandsList.Count > 0)
			{
				ImageQuery = "?" + String.Join("&", QueryCommandsList);
			}
			#endregion

			#region << Render >>

			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					var outsideWrapper = new TagBuilder("div");

					var viewWrapper = new TagBuilder("div");
					viewWrapper.Attributes.Add("id", $"view-{FieldId}");
					viewWrapper.AddCssClass($"image-wrapper erp-image view-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					viewWrapper.Attributes.Add("style", WrapperStyle);

					var viewImage = new TagBuilder("img");
					viewImage.AddCssClass("wrapper-image");
					if (!String.IsNullOrWhiteSpace(Value))
					{
						viewImage.Attributes.Add("src", $"{SrcPrefix}{Value}{ImageQuery}");
					}
					else
					{
						viewImage.Attributes.Add("src", $"");
					}

					viewImage.Attributes.Add("title", $"{FileName}");
					viewWrapper.InnerHtml.AppendHtml(viewImage);

					var viewRemoveLink = new TagBuilder("a");
					viewRemoveLink.Attributes.Add("href", "javascript:void(0)");
					viewRemoveLink.AddCssClass("action remove");
					viewRemoveLink.InnerHtml.AppendHtml(TextRemove);
					viewWrapper.InnerHtml.AppendHtml(viewRemoveLink);

					outsideWrapper.InnerHtml.AppendHtml(viewWrapper);


					var editWrapper = new TagBuilder("div");
					editWrapper.Attributes.Add("id", $"edit-{FieldId}");
					editWrapper.AddCssClass($"image-wrapper erp-image edit-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "" : "d-none")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					editWrapper.Attributes.Add("style", WrapperStyle);

					var editInput = new TagBuilder("input");
					editInput.Attributes.Add("type", "file");
					editInput.Attributes.Add("value", "");
					editInput.Attributes.Add("id", $"file-{FieldId}");
					editInput.Attributes.Add("accept", $"{Accept}");
					editWrapper.InnerHtml.AppendHtml(editInput);

					var editWrapperIconDiv = new TagBuilder("div");
					editWrapperIconDiv.AddCssClass("wrapper-icon");
					var editWrapperIconSpan = new TagBuilder("span");
					editWrapperIconSpan.AddCssClass("fa fa-image fa-fw");
					editWrapperIconDiv.InnerHtml.AppendHtml(editWrapperIconSpan);
					editWrapper.InnerHtml.AppendHtml(editWrapperIconDiv);

					var editWrapperTextDiv = new TagBuilder("div");
					editWrapperTextDiv.AddCssClass("wrapper-text");
					editWrapperTextDiv.Attributes.Add("id", $"fake-{Name}-{FieldId}");
					var editWrapperTextLink = new TagBuilder("button");
					editWrapperTextLink.Attributes.Add("type", "button");
					editWrapperTextLink.AddCssClass("btn btn-secondary btn-sm");
					editWrapperTextLink.InnerHtml.AppendHtml(TextSelect);
					editWrapperTextDiv.InnerHtml.AppendHtml(editWrapperTextLink);
					var editWrapperTextSpan = new TagBuilder("span");
					editWrapperTextSpan.AddCssClass("d-none");
					editWrapperTextDiv.InnerHtml.AppendHtml(editWrapperTextSpan);
					editWrapper.InnerHtml.AppendHtml(editWrapperTextDiv);

					outsideWrapper.InnerHtml.AppendHtml(editWrapper);

					var hiddenInput = new TagBuilder("input");
					hiddenInput.Attributes.Add("type", "hidden");
					hiddenInput.Attributes.Add("id", $"input-{FieldId}");
					hiddenInput.Attributes.Add("name", $"{Name}");
					hiddenInput.Attributes.Add("value", (Value ?? "").ToString());
					hiddenInput.Attributes.Add("data-original-value", (Value ?? "").ToString());
					hiddenInput.Attributes.Add("data-original-filename", Name);
					outsideWrapper.InnerHtml.AppendHtml(hiddenInput);

					output.Content.AppendHtml(outsideWrapper);

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldImage) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldImage) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldImage", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldImage) + "-form"] = new WvTagHelperContext()
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
							ImageFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldImageConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						Accept = Accept,
						Width = Width,
						Height = Height,
						ResizeAction = ResizeAction,
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
					var outerWrapper = new TagBuilder("div");
					var viewWrapper = new TagBuilder("div");
					viewWrapper.Attributes.Add("id", $"view-{FieldId}");
					viewWrapper.AddCssClass($"image-wrapper erp-image view-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					viewWrapper.Attributes.Add("style", $"{WrapperStyle}");

					var viewImage = new TagBuilder("img");
					viewImage.AddCssClass("wrapper-image");
					viewImage.Attributes.Add("src", $"{SrcPrefix}{Value}{ImageQuery}");
					viewImage.Attributes.Add("title", $"{FileName}");
					viewWrapper.InnerHtml.AppendHtml(viewImage);

					outerWrapper.InnerHtml.AppendHtml(viewWrapper);
					//Hidden input with the value
					var hiddenInput = new TagBuilder("input");
					hiddenInput.Attributes.Add("type", "hidden");
					hiddenInput.Attributes.Add("id", $"input-{FieldId}");
					hiddenInput.Attributes.Add("name", Name);
					hiddenInput.Attributes.Add("value", (Value ?? "").ToString());
					output.PostContent.AppendHtml(hiddenInput);


					output.PostContent.AppendHtml(outerWrapper);
				}

			}
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (!String.IsNullOrWhiteSpace(Value))
				{
					var outerWrapper = new TagBuilder("div");
					var viewWrapper = new TagBuilder("div");
					viewWrapper.Attributes.Add("id", $"view-{FieldId}");
					viewWrapper.AddCssClass($"image-wrapper erp-image view-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					viewWrapper.Attributes.Add("style", $"{WrapperStyle}");

					var viewImage = new TagBuilder("img");
					viewImage.AddCssClass("wrapper-image");
					viewImage.Attributes.Add("src", $"{SrcPrefix}{Value}{ImageQuery}");
					viewImage.Attributes.Add("title", $"{FileName}");
					viewWrapper.InnerHtml.AppendHtml(viewImage);

					outerWrapper.InnerHtml.AppendHtml(viewWrapper);
					output.PostContent.AppendHtml(outerWrapper);

				}
				else
				{
					output.Content.AppendHtml(EmptyValEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				var viewImage = new TagBuilder("img");
				viewImage.AddCssClass("img-fluid");
				viewImage.Attributes.Add("src", $"{SrcPrefix}{Value}{ImageQuery}");
				viewImage.Attributes.Add("title", $"{FileName}");
				output.Content.AppendHtml(viewImage);
				return Task.CompletedTask;
			}
			else if (Mode == WvFieldRenderMode.InlineEdit)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					var outsideWrapper = new TagBuilder("div");

					var viewWrapper = new TagBuilder("div");
					viewWrapper.Attributes.Add("id", $"view-{FieldId}");
					viewWrapper.AddCssClass($"image-wrapper erp-image view-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					viewWrapper.Attributes.Add("style", WrapperStyle);

					var viewImage = new TagBuilder("img");
					viewImage.AddCssClass("wrapper-image");
					viewImage.Attributes.Add("src", $"{SrcPrefix}{Value}{ImageQuery}");
					viewImage.Attributes.Add("title", $"{FileName}");
					viewWrapper.InnerHtml.AppendHtml(viewImage);

					var viewRemoveLink = new TagBuilder("a");
					viewRemoveLink.Attributes.Add("href", "javascript:void(0)");
					viewRemoveLink.AddCssClass("action remove");
					viewRemoveLink.InnerHtml.AppendHtml(TextRemove);
					viewWrapper.InnerHtml.AppendHtml(viewRemoveLink);

					outsideWrapper.InnerHtml.AppendHtml(viewWrapper);


					var editWrapper = new TagBuilder("div");
					editWrapper.Attributes.Add("id", $"edit-{FieldId}");
					editWrapper.AddCssClass($"image-wrapper erp-image edit-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "" : "d-none")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					editWrapper.Attributes.Add("style", WrapperStyle);

					var editInput = new TagBuilder("input");
					editInput.Attributes.Add("type", "file");
					editInput.Attributes.Add("value", "");
					editInput.Attributes.Add("id", $"file-{FieldId}");
					editInput.Attributes.Add("accept", $"{Accept}");
					editWrapper.InnerHtml.AppendHtml(editInput);

					var editWrapperIconDiv = new TagBuilder("div");
					editWrapperIconDiv.AddCssClass("wrapper-icon");
					var editWrapperIconSpan = new TagBuilder("span");
					editWrapperIconSpan.AddCssClass("fa fa-image fa-fw");
					editWrapperIconDiv.InnerHtml.AppendHtml(editWrapperIconSpan);
					editWrapper.InnerHtml.AppendHtml(editWrapperIconDiv);

					var editWrapperTextDiv = new TagBuilder("div");
					editWrapperTextDiv.AddCssClass("wrapper-text");
					editWrapperTextDiv.Attributes.Add("id", $"fake-{Name}-{FieldId}");
					var editWrapperTextLink = new TagBuilder("button");
					editWrapperTextLink.Attributes.Add("type", "button");
					editWrapperTextLink.AddCssClass("btn btn-secondary btn-sm");
					editWrapperTextLink.InnerHtml.AppendHtml(TextSelect);
					editWrapperTextDiv.InnerHtml.AppendHtml(editWrapperTextLink);
					var editWrapperTextSpan = new TagBuilder("span");
					editWrapperTextSpan.AddCssClass("d-none");
					editWrapperTextDiv.InnerHtml.AppendHtml(editWrapperTextSpan);
					editWrapper.InnerHtml.AppendHtml(editWrapperTextDiv);

					outsideWrapper.InnerHtml.AppendHtml(editWrapper);

					var hiddenInput = new TagBuilder("input");
					hiddenInput.Attributes.Add("type", "hidden");
					hiddenInput.Attributes.Add("id", $"input-{FieldId}");
					hiddenInput.Attributes.Add("name", $"{Name}");
					hiddenInput.Attributes.Add("value", (Value ?? "").ToString());
					hiddenInput.Attributes.Add("data-original-value", (Value ?? "").ToString());
					hiddenInput.Attributes.Add("data-original-filename", Name);
					outsideWrapper.InnerHtml.AppendHtml(hiddenInput);

					output.Content.AppendHtml(outsideWrapper);

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldImage) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldImage) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldImage", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldImage) + "-inline-edit"] = new WvTagHelperContext()
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
							ImageInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldImageConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						Accept = Accept,
						Width = Width,
						Height = Height,
						ResizeAction = ResizeAction,
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
					var outerWrapper = new TagBuilder("div");
					var viewWrapper = new TagBuilder("div");
					viewWrapper.Attributes.Add("id", $"view-{FieldId}");
					viewWrapper.AddCssClass($"image-wrapper erp-image view-wrapper  {(String.IsNullOrWhiteSpace(Value) ? "d-none" : "")} {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
					viewWrapper.Attributes.Add("style", $"{WrapperStyle}");

					var viewImage = new TagBuilder("img");
					viewImage.AddCssClass("wrapper-image");
					viewImage.Attributes.Add("src", $"{SrcPrefix}{Value}{ImageQuery}");
					viewImage.Attributes.Add("title", $"{FileName}");
					viewWrapper.InnerHtml.AppendHtml(viewImage);

					outerWrapper.InnerHtml.AppendHtml(viewWrapper);
					output.PostContent.AppendHtml(outerWrapper);
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
