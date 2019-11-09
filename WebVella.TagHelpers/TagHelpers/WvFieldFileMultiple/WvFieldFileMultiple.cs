using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

		[HtmlAttributeName("handler-prefix")]
		public string HandlerPrefix { get; set; } = "/fs";

		[HtmlAttributeName("file-upload-api")]
		public string FileUploadApi { get; set; } = "/fs/upload-file-multiple";

		[HtmlAttributeName("path-name")]
		public string PathName { get; set; } = "path";

		[HtmlAttributeName("size-name")]
		public string SizeName { get; set; } = "size";

		[HtmlAttributeName("name-name")]
		public string NameName { get; set; } = "name";

		[HtmlAttributeName("icon-name")]
		public string IconName { get; set; } = "icon";

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

			if (Value == null)
			{
				Value = new List<dynamic>();
			}
			else if (Value is string)
			{
				var stringValue = (string)Value;
				if (String.IsNullOrWhiteSpace(stringValue))
				{
					Value = new List<dynamic>();
				}
				else if (stringValue.Contains(","))
				{
					var filePaths = stringValue.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
					Value = GetFileObjectsFromStringList(filePaths, HandlerPrefix);
				}
				else{
					var filePaths = new List<string>{stringValue};
					Value = GetFileObjectsFromStringList(filePaths, HandlerPrefix);
				}
			}

			else if (Value is List<string>)
			{
				var filePaths = (List<string>)Value;
				Value = GetFileObjectsFromStringList(filePaths, HandlerPrefix);
			}
			else if (Value is List<object>)
			{
				var valueObjects = (List<object>)Value;
				var resultFiles = new List<dynamic>();
				foreach (var fileObject in valueObjects)
				{
					var type = fileObject.GetType();
					var filePath = "";
					var fileSize = 0;
					var fileIcon = "";
					var fileName = "";

					if (!String.IsNullOrWhiteSpace(PathName) && type.GetProperty(PathName) != null && type.GetProperty(PathName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(PathName).GetValue(fileObject);
						if (propValue is string)
						{
							filePath = HandlerPrefix + propValue.ToString();
						}
					}

					if (!String.IsNullOrWhiteSpace(SizeName) && type.GetProperty(SizeName) != null && type.GetProperty(SizeName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(SizeName).GetValue(fileObject);
						if (propValue is int || propValue is decimal || propValue is double)
						{
							fileSize = (int)propValue;
						}
					}

					if (!String.IsNullOrWhiteSpace(IconName) && type.GetProperty(IconName) != null && type.GetProperty(IconName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(IconName).GetValue(fileObject);
						if (propValue is string)
						{
							fileIcon = propValue.ToString();
						}
					}
					if (!String.IsNullOrWhiteSpace(filePath) && String.IsNullOrWhiteSpace(fileIcon))
					{
						fileIcon = "fa " + WvHelpers.GetPathTypeIcon(filePath);
					}

					if (!String.IsNullOrWhiteSpace(NameName) && type.GetProperty(NameName) != null && type.GetProperty(NameName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(NameName).GetValue(fileObject);
						if (propValue is string)
						{
							fileName = propValue.ToString();
						}
					}
					if (!String.IsNullOrWhiteSpace(filePath) && String.IsNullOrWhiteSpace(fileName))
					{
						fileName = WvHelpers.GetFileNameFromPath(filePath);
					}

					dynamic file = new
					{
						path = filePath,
						size = fileSize,
						name = fileName,
						icon = fileIcon
					};
					resultFiles.Add(file);
				}
				Value = resultFiles;
			}
			else
			{
				output.Content.AppendHtml("<div class='go-red'>'value' property type is not supported</div>");
				return Task.CompletedTask;
			}

			#endregion

			#region << Render >>
			var fileObjects = (List<dynamic>)Value;
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{

					#region << Hidden input for posting >>
					output.Content.AppendHtml($"<input type='hidden' id='input-{FieldId}' name='{Name}' value='{String.Join(',', fileObjects.Select(x=> x.path).ToList())}'/>");
					#endregion

					#region << fake upload >>
					var inputGroupEl = new TagBuilder("div");
					inputGroupEl.AddCssClass("input-group erp-file-multiple-input");

					var fakeInputEl = new TagBuilder("div");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-file with-progress");
					fakeInputEl.Attributes.Add("id", $"fake-{FieldId}");
					fakeInputEl.Attributes.Add("style", "cursor:pointer");
					inputElCssClassList.Add("go-gray");
					fakeInputEl.InnerHtml.AppendHtml("Activate 'Paste Image' from clipboard");


					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}
					fakeInputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

					var fakeInputProgress = new TagBuilder("div");
					fakeInputProgress.AddCssClass("form-control-progress");
					fakeInputEl.InnerHtml.AppendHtml(fakeInputProgress);

					fakeInputEl.Attributes.Add("data-handler-prefix",HandlerPrefix);
					fakeInputEl.Attributes.Add("data-file-upload-api",FileUploadApi);
					fakeInputEl.Attributes.Add("data-path-name",PathName);
					fakeInputEl.Attributes.Add("data-size-name",SizeName);
					fakeInputEl.Attributes.Add("data-name-name",NameName);
					fakeInputEl.Attributes.Add("data-icon-name",IconName);


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

					if (fileObjects.Count == 0)
						filesListEl.AddCssClass("d-none");


					//Generate the files list
					foreach (var fileObject in fileObjects)
					{
						var fileRow = new TagBuilder("div");
						fileRow.AddCssClass("filerow");
						fileRow.Attributes.Add("data-file-path", fileObject.path);

						//Append icon
						fileRow.InnerHtml.AppendHtml($"<div class='icon'><i class='fa {fileObject.icon}'></i></div>");

						//Append meta
						var rowMeta = new TagBuilder("div");
						rowMeta.AddCssClass("meta");

						//Append file 
						rowMeta.InnerHtml.AppendHtml($"<a class='link' href='{HandlerPrefix}{fileObject.path}' target='_blank' title='{HandlerPrefix}{fileObject.path}'>{fileObject.name}<em></em></a>");

						if (((int)fileObject.size) > 0 ){
							var sizeString = WvHelpers.GetSizeStringFromSize(fileObject.size);
							rowMeta.InnerHtml.AppendHtml($"<div class='size'>{sizeString}</div>");
						}

						fileRow.InnerHtml.AppendHtml(rowMeta);

						//Action
						var rowAction = new TagBuilder("div");
						rowAction.AddCssClass("action remove");
						rowAction.InnerHtml.AppendHtml($"<a class='link' href='#'><i class='fa fa-times-circle'></i></a>");

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
							var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldFileMultiple", "WebVella.TagHelpers");
							var scriptEl = new TagBuilder("script");
							//scriptEl.Attributes.Add("type", "text/javascript");
							scriptEl.InnerHtml.AppendHtml(scriptContent);
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
					output.Content.AppendHtml(GenerateDisplayHtml(fileObjects));

				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				output.Content.AppendHtml(GenerateDisplayHtml(fileObjects));
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				foreach (var fileObject in fileObjects)
				{
					var fileSpan = new TagBuilder("span");
					fileSpan.AddCssClass("mr-2");
					if(!String.IsNullOrWhiteSpace(fileObject.icon)){
						fileSpan.InnerHtml.AppendHtml($"<i class='{fileObject.icon}'></i> ");
					}
					var fileLink = new TagBuilder("a");
					fileLink.Attributes.Add("href",fileObject.path);
					fileLink.Attributes.Add("target","_blank");

					fileLink.InnerHtml.Append(fileObject.name);

					fileSpan.InnerHtml.AppendHtml(fileLink);

					output.Content.AppendHtml(fileSpan);
				}

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

		private List<dynamic> GetFileObjectsFromStringList(List<string> filePaths, string handlerPrefix)
		{
			var resultFiles = new List<dynamic>();
			foreach (var path in filePaths)
			{
				var filePath = path;
				if(!filePath.StartsWith(handlerPrefix)){
					filePath = handlerPrefix + filePath;
				}
				var iconClass = "fa " + WvHelpers.GetPathTypeIcon(filePath);
				var fileName = WvHelpers.GetFileNameFromPath(filePath);
				dynamic file = new
				{
					path = filePath,
					size = 0,
					name = fileName,
					icon = iconClass
				};
				resultFiles.Add(file);
			}

			return resultFiles;
		}

		public TagBuilder GenerateDisplayHtml(List<dynamic> fileObjects){
			var resultEl = new TagBuilder("div");
			if(fileObjects.Count == 0){
				resultEl.AddCssClass("form-control-plaintext");
				resultEl.AddCssClass("go-gray");
				resultEl.InnerHtml.AppendHtml("No data");
				return resultEl;
			}

			resultEl.AddCssClass("wv-field-file-multiple form-control-plaintext");

			foreach (var fileObject in fileObjects)
			{
				var fileRowEl = new TagBuilder("a");
				fileRowEl.AddCssClass("filerow");
				fileRowEl.Attributes.Add("href",fileObject.path);
				fileRowEl.Attributes.Add("target","_blank");
				fileRowEl.Attributes.Add("title",fileObject.path);
				if(!String.IsNullOrWhiteSpace(fileObject.icon)){
					fileRowEl.InnerHtml.AppendHtml($"<div class='icon'><i class='{fileObject.icon}'></i></div>");
				}

				var fileRowMetaEl = new TagBuilder("div");
				fileRowMetaEl.AddCssClass("meta");
				fileRowMetaEl.InnerHtml.AppendHtml($"<span class='link go-blue'>{(String.IsNullOrWhiteSpace(fileObject.name) ? "unknown name" : fileObject.name)}</span>");
				if(((int)fileObject.size) > 0){
					var sizeString = WvHelpers.GetSizeStringFromSize((int)fileObject.size);
					fileRowMetaEl.InnerHtml.AppendHtml($"<div class='size'>{sizeString}</div>");
				}


				fileRowEl.InnerHtml.AppendHtml(fileRowMetaEl);
				resultEl.InnerHtml.AppendHtml(fileRowEl);
			}
			return resultEl;
		}
	}


}
