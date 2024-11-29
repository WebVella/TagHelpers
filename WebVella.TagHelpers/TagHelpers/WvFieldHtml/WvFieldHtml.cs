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
	[HtmlTargetElement("wv-field-html")]
	[RestrictChildren("wv-field-prepend", "wv-field-append")]
	public class WvFieldHtml : WvFieldBase
	{
		[HtmlAttributeName("upload-mode")]
		public HtmlUploadMode UploadMode { get; set; } = HtmlUploadMode.None;

		[HtmlAttributeName("toolbar-mode")]
		public HtmlToolbarMode ToolbarMode { get; set; } = HtmlToolbarMode.Basic;

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return;
			}
			var processedValue = "";

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

			#region << Init Value >>
			if (Value != null)
			{
				processedValue = Value.ToString();
			}
			#endregion

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					var inputEl = new TagBuilder("textarea");
					inputEl.Attributes.Add("id", $"input-{FieldId}");
					inputEl.Attributes.Add("name", Name);
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-html");

					if (Required)
					{
						inputEl.Attributes.Add("required", null);
					}

					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}

					inputEl.InnerHtml.Append(processedValue);

					inputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

					output.Content.AppendHtml(inputEl);

					#region << Init Libraries >>
					//Due to many problems is included in the head not module by module
					//var wvLibraryInitialized = false;
					//var libraryItemsKey = "WebVella-" + "ckeditor";
					//if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
					//{
					//	var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
					//	wvLibraryInitialized = tagHelperContext.Initialized;
					//}

					//if(!wvLibraryInitialized){
					//	var libJsEl = new TagBuilder("script");
					//	libJsEl.Attributes.Add("type", "text/javascript");
					//	libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/ckeditor5/ckeditor.js");
					//	output.PostContent.AppendHtml(libJsEl);	
					//	output.PostContent.AppendHtml("\r\n\t");			

					//	ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
					//	{
					//		Initialized = true
					//	};
					//}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldHtml) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldHtml) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldHtml", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldHtml) + "-form"] = new WvTagHelperContext()
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
							InitHtmlFieldCKEditor(""{{FieldId}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));

					var fieldConfig = new WvFieldHtmlConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						UploadMode = UploadMode,
						ToolbarMode = ToolbarMode
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
						prependEl.AddCssClass($"input-group-prepend erp-multilinetext {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
						foreach (var htmlString in PrependHtml)
						{
							prependEl.InnerHtml.AppendHtml(htmlString);
						}
						inputGroupEl.InnerHtml.AppendHtml(prependEl);
					}
					//Control
					var inputEl = new TagBuilder("div");
					inputEl.Attributes.Add("id", $"plaintext-{FieldId}");
					inputEl.AddCssClass("form-control erp-html disabled");
					inputEl.Attributes.Add("style", "height:auto;");
					inputEl.InnerHtml.AppendHtml(processedValue);
					inputGroupEl.InnerHtml.AppendHtml(inputEl);
					//Append
					if (AppendHtml.Count > 0)
					{
						var appendEl = new TagBuilder("span");
						appendEl.AddCssClass($"input-group-append erp-multilinetext {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");

						foreach (var htmlString in AppendHtml)
						{
							appendEl.InnerHtml.AppendHtml(htmlString);
						}
						inputGroupEl.InnerHtml.AppendHtml(appendEl);
					}
					//Hidden input with the value
					var hiddenInput = new TagBuilder("input");
					hiddenInput.Attributes.Add("type", "hidden");
					hiddenInput.Attributes.Add("id", $"input-{FieldId}");
					hiddenInput.Attributes.Add("name", Name);
					hiddenInput.Attributes.Add("value", processedValue);
					output.Content.AppendHtml(hiddenInput);

					output.Content.AppendHtml(inputGroupEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (!String.IsNullOrWhiteSpace(processedValue))
				{
					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-html");
					divEl.InnerHtml.AppendHtml(processedValue);
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
				output.Content.AppendHtml(processedValue);
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
							viewInputPrepend.AddCssClass("input-group-prepend erp-multilinetext");
							foreach (var htmlString in PrependHtml)
							{
								viewInputPrepend.InnerHtml.AppendHtml(htmlString);
							}
							viewWrapperEl.InnerHtml.AppendHtml(viewInputPrepend);
						}
						//Control

						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass("form-control erp-html");
						viewFormControlEl.Attributes.Add("style", "height:auto;");
						viewFormControlEl.InnerHtml.AppendHtml(processedValue);
						viewWrapperEl.InnerHtml.AppendHtml(viewFormControlEl);

						//Append
						var viewInputActionEl = new TagBuilder("span");
						viewInputActionEl.AddCssClass("input-group-append erp-multilinetext action");
						foreach (var htmlString in AppendHtml)
						{
							viewInputActionEl.InnerHtml.AppendHtml(htmlString);
						}
						viewInputActionEl.InnerHtml.AppendHtml("<button type=\"button\" class='btn btn-white' title='edit'><i class='fa fa-fw fa-pencil-alt'></i></button>");
						viewWrapperEl.InnerHtml.AppendHtml(viewInputActionEl);
						output.Content.AppendHtml(viewWrapperEl);
					}
					#endregion

					#region << Edit Modal >>
					{
						var editModalEl = new TagBuilder("div");
						editModalEl.AddCssClass("modal");
						editModalEl.Attributes.Add("id", $"edit-{FieldId}");
						editModalEl.Attributes.Add("tabindex", "-1");
						var editModalDialog = new TagBuilder("div");
						editModalDialog.AddCssClass("modal-dialog modal-lg");
						var editModalContent = new TagBuilder("div");
						editModalContent.AddCssClass("modal-content");

						var editModalHeader = new TagBuilder("div");
						editModalHeader.AddCssClass("modal-header");
						var editModalHeaderTitle = new TagBuilder("h5");
						editModalHeaderTitle.AddCssClass("modal-title");
						editModalHeaderTitle.InnerHtml.Append("edit field ");
						var editModalHeaderTitleSpan = new TagBuilder("span");
						editModalHeaderTitleSpan.AddCssClass("go-green");
						editModalHeaderTitleSpan.InnerHtml.Append(Name);
						editModalHeaderTitle.InnerHtml.AppendHtml(editModalHeaderTitleSpan);
						editModalHeader.InnerHtml.AppendHtml(editModalHeaderTitle);
						var editModalHeaderButton = new TagBuilder("button");
						editModalHeaderButton.Attributes.Add("type", "button");
						editModalHeaderButton.AddCssClass("close");
						editModalHeaderButton.Attributes.Add("data-dismiss", "modal");
						editModalHeaderButton.InnerHtml.AppendHtml(new TagBuilder("span").InnerHtml.AppendHtml("&times;"));
						editModalHeader.InnerHtml.AppendHtml(editModalHeaderButton);
						editModalContent.InnerHtml.AppendHtml(editModalHeader);

						var editModalBody = new TagBuilder("div");
						editModalBody.AddCssClass("modal-body");
						var editModalBodyTextArea = new TagBuilder("textarea");
						editModalBodyTextArea.Attributes.Add("id", $"input-{FieldId}");
						editModalBodyTextArea.AddCssClass("form-control erp-html");
						if (Required)
						{
							editModalBodyTextArea.Attributes.Add("required", null);
						}
						editModalBodyTextArea.InnerHtml.AppendHtml(processedValue);
						editModalBody.InnerHtml.AppendHtml(editModalBodyTextArea);
						editModalContent.InnerHtml.AppendHtml(editModalBody);

						var editModalFooter = new TagBuilder("div");
						editModalFooter.AddCssClass("modal-footer");
						var editModalFooterSave = new TagBuilder("button");
						editModalFooterSave.Attributes.Add("type", "button");
						editModalFooterSave.AddCssClass("btn btn-primary save btn-sm");
						var editModalFooterSaveIcon = new TagBuilder("span");
						editModalFooterSaveIcon.AddCssClass("fa fa-check");
						editModalFooterSave.InnerHtml.AppendHtml(editModalFooterSaveIcon);
						editModalFooterSave.InnerHtml.AppendHtml(" save");
						editModalFooter.InnerHtml.AppendHtml(editModalFooterSave);
						var editModalFooterCancel = new TagBuilder("button");
						editModalFooterCancel.Attributes.Add("type", "button");
						editModalFooterCancel.AddCssClass("btn btn-secondary cancel btn-sm");
						editModalFooterCancel.InnerHtml.Append("cancel");
						editModalFooter.InnerHtml.AppendHtml(editModalFooterCancel);
						editModalContent.InnerHtml.AppendHtml(editModalFooter);

						editModalDialog.InnerHtml.AppendHtml(editModalContent);
						editModalEl.InnerHtml.AppendHtml(editModalDialog);

						output.Content.AppendHtml(editModalEl);
					}
					#endregion

					#region << Init Libraries >>
					//Due to many problems is included in the head not module by module
					//var wvLibraryInitialized = false;
					//var libraryItemsKey = "WebVella-" + "ckeditor";
					//if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
					//{
					//	var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
					//	wvLibraryInitialized = tagHelperContext.Initialized;
					//}

					//if(!wvLibraryInitialized){
					//	var libJsEl = new TagBuilder("script");
					//	libJsEl.Attributes.Add("type", "text/javascript");
					//	libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/ckeditor5/ckeditor.js");
					//	output.PostContent.AppendHtml(libJsEl);	
					//	output.PostContent.AppendHtml("\r\n\t");			

					//	ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
					//	{
					//		Initialized = true
					//	};
					//}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldHtml) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldHtml) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldHtml", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldHtml) + "-inline-edit"] = new WvTagHelperContext()
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
							HtmlInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldHtmlConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						UploadMode = UploadMode,
						ToolbarMode = ToolbarMode
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
						viewInputPrepend.AddCssClass("input-group-prepend  erp-multilinetext");
						foreach (var htmlString in PrependHtml)
						{
							viewInputPrepend.InnerHtml.AppendHtml(htmlString);
						}
						divEl.InnerHtml.AppendHtml(viewInputPrepend);
					}
					//Control
					var inputEl = new TagBuilder("div");
					inputEl.AddCssClass("form-control erp-html disabled");
					inputEl.InnerHtml.AppendHtml(processedValue);
					divEl.InnerHtml.AppendHtml(inputEl);
					//Append
					var appendActionSpan = new TagBuilder("span");
					appendActionSpan.AddCssClass("input-group-append  erp-multilinetext");
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
