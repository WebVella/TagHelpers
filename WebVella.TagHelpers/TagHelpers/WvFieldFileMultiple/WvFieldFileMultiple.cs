using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;
using Yahoo.Yui.Compressor;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-file-multiple")]
	public class WvFieldFileMultiple : WvFieldBase
	{
		[HtmlAttributeName("accept")]
		public string Accept { get; set; } = "";

		private List<string> FilePaths { get; set; } = new List<string>();

		[HtmlAttributeName("file-upload-api")]
		public string FileUploadApi { get; set; } = "/fs/upload-file-multiple";

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

			if (Value is string && Value == "")
				Value = null;

			if (Value != null && !(Value is List<string>))
			{
				output.Content.AppendHtml("<div class='go-red'>'value' property should be 'List&lt;string>'</div>");
				return Task.CompletedTask;
			}

			if (Value != null && Value is List<string>)
				FilePaths = (List<string>)Value;

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{

					#region << Hidden input for posting >>
					output.Content.AppendHtml($"<input type='hidden' id='input-{FieldId}' name='{Name}' value='{String.Join(',', FilePaths)}'/>");
					#endregion

					#region << fake upload >>
					var inputGroupEl = new TagBuilder("div");
					inputGroupEl.AddCssClass("input-group erp-file-multiple-input");

					var fakeInputEl = new TagBuilder("div");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-file with-progress");
					fakeInputEl.Attributes.Add("id", $"fake-{FieldId}");

					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}
					fakeInputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

					var fakeInputProgress = new TagBuilder("div");
					fakeInputProgress.AddCssClass("form-control-progress");
					fakeInputEl.InnerHtml.AppendHtml(fakeInputProgress);

					inputGroupEl.InnerHtml.AppendHtml(fakeInputEl);

					var appendEl = new TagBuilder("span");
					appendEl.AddCssClass("input-group-append action erp-file");
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
					realHiddenFileInput.Attributes.Add("multiple", $"true");
					realHiddenFileInput.AddCssClass("d-none");
					realHiddenFileInput.Attributes.Add("value", $"");
					if (!String.IsNullOrWhiteSpace(Accept))
					{
						realHiddenFileInput.Attributes.Add("accept", $"{Accept}");
					}
					output.Content.AppendHtml(realHiddenFileInput);

					#endregion

					#region << Files list element >>

					var filesListEl = new TagBuilder("div");
					filesListEl.AddCssClass("form-control erp-file-multiple-list form");
					filesListEl.Attributes.Add("id", $"fake-list-{FieldId}");

					if (FilePaths.Count == 0)
						filesListEl.AddCssClass("d-none");


					//Generate the files list
					foreach (var filePath in FilePaths)
					{
						var fileRow = new TagBuilder("div");
						fileRow.AddCssClass("filerow");
						fileRow.Attributes.Add("data-file-path", filePath);
						//Append icon
						var iconClass = WvHelpers.GetPathTypeIcon(filePath);
						fileRow.InnerHtml.AppendHtml($"<div class='icon'><i class='fa {iconClass}'></i></div>");

						//Append meta
						var rowMeta = new TagBuilder("div");
						rowMeta.AddCssClass("meta");

						//Append file 
						var fileName = WvHelpers.GetFileNameFromPath(filePath);
						rowMeta.InnerHtml.AppendHtml($"<a class='link' href='{filePath}' target='_blank' title='{filePath}'>{fileName}<em></em></a>");

						fileRow.InnerHtml.AppendHtml(rowMeta);

						//Action
						var rowAction = new TagBuilder("div");
						rowAction.AddCssClass("action remove");
						rowAction.InnerHtml.AppendHtml($"<a class='link' href='#'><i class='fa fa-times-circle'></i></a>");
						//rowAction.InnerHtml.AppendHtml($"<span class='progress d-none'>0%</span>");
						//rowAction.InnerHtml.AppendHtml($"<span class='error go-red d-none'><i class='fas fa-exclamation-circle'></i></span>");

						fileRow.InnerHtml.AppendHtml(rowAction);
						filesListEl.InnerHtml.AppendHtml(fileRow);
					}

					output.Content.AppendHtml(filesListEl);

					#endregion


					var jsCompressor = new JavaScriptCompressor();

					#region << Init Scripts >>
					{
						var tagHelperInitialized = false;
						var fileName = "form";
						if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldFileMultiple) + fileName))
						{
							var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldFileMultiple) + fileName];
							tagHelperInitialized = tagHelperContext.Initialized;
						}
						if (!tagHelperInitialized)
						{
							var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldFileMultiple");
							var scriptEl = new TagBuilder("script");
							scriptEl.Attributes.Add("type", "text/javascript");
							//scriptEl.InnerHtml.AppendHtml(scriptContent);
							scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
							output.PostContent.AppendHtml(scriptEl);

							ViewContext.HttpContext.Items[typeof(WvFieldFileMultiple) + fileName] = new WvTagHelperContext()
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
							FieldMultiFileFormInit(""{{FieldId}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId ?? null).ToString());

					var fieldConfig = new WvFieldFileConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						Accept = Accept,
						FileUploadApi = FileUploadApi
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonConvert.SerializeObject(fieldConfig));

					initScript.InnerHtml.AppendHtml(jsCompressor.Compress(scriptTemplate));

					output.PostContent.AppendHtml(initScript);
					#endregion

				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					output.Content.AppendHtml("Not implemented yet");

				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				output.Content.AppendHtml("Not implemented yet");
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.Content.AppendHtml("Not implemented yet");
			}
			else if (Mode == WvFieldRenderMode.InlineEdit)
			{
				output.Content.AppendHtml("Not implemented yet");
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
