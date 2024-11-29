using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-file-multiple")]
	public class WvFieldFileMultiple : WvFieldBase
	{
		[HtmlAttributeName("accept")]
		public string Accept { get; set; } = "";

		[HtmlAttributeName("get-handler-prefix")]
		public string GetHandlerPrefix { get; set; } = "/fs";

		[HtmlAttributeName("file-upload-api")]
		public string FileUploadApi { get; set; } = "/fs/upload-file-multiple";

		[HtmlAttributeName("path-prop-name")]
		public string PathPropName { get; set; } = "path";

		[HtmlAttributeName("size-prop-name")]
		public string SizePropName { get; set; } = "size";

		[HtmlAttributeName("name-prop-name")]
		public string NamePropName { get; set; } = "name";

		[HtmlAttributeName("icon-prop-name")]
		public string IconPropName { get; set; } = "icon";

		[HtmlAttributeName("timestamp-prop-name")]
		public string TimestampPropName { get; set; } = "timestamp";

		[HtmlAttributeName("author-prop-name")]
		public string AuthorPropName { get; set; } = "author";

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
					Value = GetFileObjectsFromStringList(filePaths);
				}
				else
				{
					var filePaths = new List<string> { stringValue };
					Value = GetFileObjectsFromStringList(filePaths);
				}
			}

			else if (Value is List<string>)
			{
				var filePaths = (List<string>)Value;
				Value = GetFileObjectsFromStringList(filePaths);
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
					DateTime? fileTimestamp = null;
					var fileAuthor = "";

					if (!String.IsNullOrWhiteSpace(PathPropName) && type.GetProperty(PathPropName) != null && type.GetProperty(PathPropName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(PathPropName).GetValue(fileObject);
						if (propValue is string)
						{
							filePath = propValue.ToString();
						}
					}

					if (!String.IsNullOrWhiteSpace(SizePropName) && type.GetProperty(SizePropName) != null && type.GetProperty(SizePropName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(SizePropName).GetValue(fileObject);
						if (propValue is int || propValue is decimal || propValue is double)
						{
							fileSize = (int)propValue;
						}
					}

					if (!String.IsNullOrWhiteSpace(IconPropName) && type.GetProperty(IconPropName) != null && type.GetProperty(IconPropName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(IconPropName).GetValue(fileObject);
						if (propValue is string)
						{
							fileIcon = propValue.ToString();
						}
					}
					if (!String.IsNullOrWhiteSpace(filePath) && String.IsNullOrWhiteSpace(fileIcon))
					{
						fileIcon = "fa " + WvHelpers.GetPathTypeIcon(filePath);
					}

					if (!String.IsNullOrWhiteSpace(NamePropName) && type.GetProperty(NamePropName) != null && type.GetProperty(NamePropName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(NamePropName).GetValue(fileObject);
						if (propValue is string)
						{
							fileName = propValue.ToString();
						}
					}
					if (!String.IsNullOrWhiteSpace(filePath) && String.IsNullOrWhiteSpace(fileName))
					{
						fileName = WvHelpers.GetFileNameFromPath(filePath);
					}

					if (!String.IsNullOrWhiteSpace(TimestampPropName) && type.GetProperty(TimestampPropName) != null && type.GetProperty(TimestampPropName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(TimestampPropName).GetValue(fileObject);
						if (propValue is DateTime)
						{
							fileTimestamp = (DateTime)propValue;
						}
					}

					if (!String.IsNullOrWhiteSpace(AuthorPropName) && type.GetProperty(AuthorPropName) != null && type.GetProperty(AuthorPropName).GetValue(fileObject) != null)
					{
						var propValue = type.GetProperty(AuthorPropName).GetValue(fileObject);
						if (propValue is string)
						{
							fileAuthor = propValue.ToString();
						}
					}

					dynamic file = new
					{
						path = filePath,
						size = fileSize,
						name = fileName,
						icon = fileIcon,
						timestamp = fileTimestamp,
						author = fileAuthor
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
					output.Content.AppendHtml($"<input type='hidden' id='input-{FieldId}' name='{Name}' value='{String.Join(',', fileObjects.Select(x => x.path).ToList())}'/>");
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

					fakeInputEl.Attributes.Add("data-handler-prefix", GetHandlerPrefix);
					fakeInputEl.Attributes.Add("data-file-upload-api", FileUploadApi);
					fakeInputEl.Attributes.Add("data-path-name", PathPropName);
					fakeInputEl.Attributes.Add("data-size-name", SizePropName);
					fakeInputEl.Attributes.Add("data-name-name", NamePropName);
					fakeInputEl.Attributes.Add("data-icon-name", IconPropName);
					fakeInputEl.Attributes.Add("data-timestamp-name", TimestampPropName);
					fakeInputEl.Attributes.Add("data-author-name", AuthorPropName);
					fakeInputEl.Attributes.Add("data-field-id", (FieldId != null ? FieldId.Value.ToString() : ""));


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
						fileRow.Attributes.Add("data-field-id", (FieldId != null ? FieldId.Value.ToString() : ""));

						//Append icon
						fileRow.InnerHtml.AppendHtml($"<div class='icon'><i class='fa {fileObject.icon}'></i></div>");

						//Append meta
						var rowMeta = new TagBuilder("div");
						rowMeta.AddCssClass("meta");

						//Append file 
						rowMeta.InnerHtml.AppendHtml($"<a class='link' href='{GetHandlerPrefix}{fileObject.path}' target='_blank' title='{GetHandlerPrefix}{fileObject.path}'>{fileObject.name}<em></em></a>");

						if (((int)fileObject.size) > 0 || ((DateTime?)fileObject.timestamp) != null || String.IsNullOrWhiteSpace((string)fileObject.author))
						{

							var fileRowMetaDetailsEl = new TagBuilder("div");
							fileRowMetaDetailsEl.AddCssClass("details");

							if (((int)fileObject.size) > 0)
							{
								var sizeString = WvHelpers.GetSizeStringFromSize((int)fileObject.size);
								fileRowMetaDetailsEl.InnerHtml.AppendHtml($"<span class='size'>{sizeString}</span>");
							}
							if (((DateTime?)fileObject.timestamp) != null)
							{
								var timestampString = ((DateTime)fileObject.timestamp).ToString("dd MMM yyyy HH:mm", Culture);
								fileRowMetaDetailsEl.InnerHtml.AppendHtml($"<span class='timestamp'>{timestampString}</span>");
							}
							if (!String.IsNullOrWhiteSpace((string)fileObject.author))
							{
								fileRowMetaDetailsEl.InnerHtml.AppendHtml($"<span class='author'>{(string)fileObject.author}</span>");
							}

							rowMeta.InnerHtml.AppendHtml(fileRowMetaDetailsEl);

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
							scriptEl.Attributes.Add("type", "text/javascript");
							//scriptEl.InnerHtml.AppendHtml(scriptContent);
							scriptEl.InnerHtml.AppendHtml(scriptContent);
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
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));

					var fieldConfig = new WvFieldFileConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						Accept = Accept,
						FileUploadApi = FileUploadApi
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion

				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					output.Content.AppendHtml(GenerateDisplayHtml(fileObjects, GetHandlerPrefix));

				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				output.Content.AppendHtml(GenerateDisplayHtml(fileObjects, GetHandlerPrefix));
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				foreach (var fileObject in fileObjects)
				{
					var fileSpan = new TagBuilder("span");
					fileSpan.AddCssClass("mr-2");
					if (!String.IsNullOrWhiteSpace(fileObject.icon))
					{
						fileSpan.InnerHtml.AppendHtml($"<i class='{fileObject.icon}'></i> ");
					}
					var fileLink = new TagBuilder("a");
					fileLink.Attributes.Add("href", GetHandlerPrefix + fileObject.path);
					fileLink.Attributes.Add("title", GetHandlerPrefix + fileObject.path);
					fileLink.Attributes.Add("target", "_blank");

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

		private List<dynamic> GetFileObjectsFromStringList(List<string> filePaths)
		{
			var resultFiles = new List<dynamic>();
			foreach (var filePath in filePaths)
			{
				var iconClass = "fa " + WvHelpers.GetPathTypeIcon(filePath);
				var fileName = WvHelpers.GetFileNameFromPath(filePath);
				dynamic file = new
				{
					path = filePath,
					size = 0,
					name = fileName,
					icon = iconClass,
					timestamp = (DateTime?)null,
					author = ""
				};
				resultFiles.Add(file);
			}

			return resultFiles;
		}

		public TagBuilder GenerateDisplayHtml(List<dynamic> fileObjects, string getHandlerPrefix)
		{
			var resultEl = new TagBuilder("div");
			if (fileObjects.Count == 0)
			{
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
				fileRowEl.Attributes.Add("href", getHandlerPrefix + fileObject.path);
				fileRowEl.Attributes.Add("target", "_blank");
				fileRowEl.Attributes.Add("title", getHandlerPrefix + fileObject.path);
				if (!String.IsNullOrWhiteSpace(fileObject.icon))
				{
					fileRowEl.InnerHtml.AppendHtml($"<div class='icon'><i class='{fileObject.icon}'></i></div>");
				}

				var fileRowMetaEl = new TagBuilder("div");
				fileRowMetaEl.AddCssClass("meta");
				fileRowMetaEl.InnerHtml.AppendHtml($"<span class='link go-blue'>{(String.IsNullOrWhiteSpace(fileObject.name) ? "unknown name" : fileObject.name)}</span>");

				if (((int)fileObject.size) > 0 || ((DateTime?)fileObject.timestamp) != null || String.IsNullOrWhiteSpace((string)fileObject.author))
				{
					var fileRowMetaDetailsEl = new TagBuilder("div");
					fileRowMetaDetailsEl.AddCssClass("details");

					if (((int)fileObject.size) > 0)
					{
						var sizeString = WvHelpers.GetSizeStringFromSize((int)fileObject.size);
						fileRowMetaDetailsEl.InnerHtml.AppendHtml($"<span class='size'>{sizeString}</span>");
					}
					if (((DateTime?)fileObject.timestamp) != null)
					{
						var timestampString = ((DateTime)fileObject.timestamp).ToString("dd MMM yyyy HH:mm", Culture);
						fileRowMetaDetailsEl.InnerHtml.AppendHtml($"<span class='timestamp'>{timestampString}</span>");
					}
					if (!String.IsNullOrWhiteSpace((string)fileObject.author))
					{
						fileRowMetaDetailsEl.InnerHtml.AppendHtml($"<span class='author'>{(string)fileObject.author}</span>");
					}

					fileRowMetaEl.InnerHtml.AppendHtml(fileRowMetaDetailsEl);
				}

				fileRowEl.InnerHtml.AppendHtml(fileRowMetaEl);
				resultEl.InnerHtml.AppendHtml(fileRowEl);
			}
			return resultEl;
		}
	}


}
