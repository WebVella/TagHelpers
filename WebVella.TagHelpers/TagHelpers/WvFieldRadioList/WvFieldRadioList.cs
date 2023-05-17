﻿using Microsoft.AspNetCore.Mvc.Rendering;
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

	[HtmlTargetElement("wv-field-radio-list")]
	public class WvFieldRadioList : WvFieldBase
	{
		[HtmlAttributeName("options")]
		public List<WvSelectOption> Options { get; set; } = new List<WvSelectOption>();

		[HtmlAttributeName("is-button-group")]
		public bool IsButtonGroup { get; set; } = false;

		[HtmlAttributeName("button-class-unchecked")]
		public string ButtonClassUnchecked { get; set; } = "btn-secondary";

		[HtmlAttributeName("button-class-checked")]
		public string ButtonClassChecked { get; set; } = "btn-primary";

        [HtmlAttributeName("is-block")]
        public bool IsBlock { get; set; } = false;

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
				if (!IsButtonGroup)
				{
					var wrapper1 = new TagBuilder("div");
					wrapper1.AddCssClass("form-control-plaintext erp-radio-list");
					wrapper1.Attributes.Add("id", $"input-" + FieldId);
					foreach (var selectOption in Options)
					{
						var wrapper2 = new TagBuilder("div");
						wrapper2.AddCssClass($"form-check {(IsBlock ? "" : "form-check-inline")} ml-1");
						var labelWrapper = new TagBuilder("label");
						labelWrapper.AddCssClass("form-check-label");

						var inputChkb = new TagBuilder("input");
						inputChkb.Attributes.Add("type", "radio");
						inputChkb.Attributes.Add("value", selectOption.Value);
						inputChkb.Attributes.Add("name", Name);
						if (Access == WvFieldAccess.ReadOnly)
						{
							inputChkb.Attributes.Add("readonly", null);
						}

						inputChkb.AddCssClass($"form-check-input {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");

						if (Value != null && Value == selectOption.Value)
						{
							inputChkb.Attributes.Add("checked", "checked");
						}
						labelWrapper.InnerHtml.AppendHtml(inputChkb);

						labelWrapper.InnerHtml.AppendHtml(selectOption.Label);
						wrapper2.InnerHtml.AppendHtml(labelWrapper);
						wrapper1.InnerHtml.AppendHtml(wrapper2);

					}

					output.Content.AppendHtml(wrapper1);
				}
				else
				{
					var wrapper1 = new TagBuilder("div");
					wrapper1.AddCssClass("form-control-plaintext erp-radio-list is-buttons");
					wrapper1.Attributes.Add("id", $"input-" + FieldId);
					var wrapper2 = new TagBuilder("div");
					wrapper2.AddCssClass("btn-group btn-group-toggle");
					foreach (var selectOption in Options)
					{
						var labelWrapper = new TagBuilder("label");
						if (Value != null && Value == selectOption.Value)
						{
							labelWrapper.AddCssClass($"btn {ButtonClassChecked}");
						}
						else
						{
							labelWrapper.AddCssClass($"btn {ButtonClassUnchecked}");
						}
						var inputChkb = new TagBuilder("input");
						inputChkb.Attributes.Add("type", "radio");
						inputChkb.Attributes.Add("value", selectOption.Value);
						inputChkb.Attributes.Add("name", Name);
						if (Access == WvFieldAccess.ReadOnly)
						{
							inputChkb.Attributes.Add("readonly", null);
						}
						inputChkb.Attributes.Add("autocomplete", "off");
						inputChkb.AddCssClass($"form-check-input {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");

						if (Value != null && Value == selectOption.Value)
						{
							inputChkb.Attributes.Add("checked", "checked");
						}
						labelWrapper.InnerHtml.AppendHtml(inputChkb);

						labelWrapper.InnerHtml.AppendHtml(selectOption.Label);
						wrapper2.InnerHtml.AppendHtml(labelWrapper);

					}
					wrapper1.InnerHtml.AppendHtml(wrapper2);
					output.Content.AppendHtml(wrapper1);
				}

				#region << Init Scripts >>
				var tagHelperInitialized = false;
				var fileName = "form.js";
				if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldRadioList) + fileName))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldRadioList) + fileName];
					tagHelperInitialized = tagHelperContext.Initialized;
				}
				if (!tagHelperInitialized)
				{
					var scriptContent = WvHelpers.GetEmbeddedTextResource(fileName, "WebVella.TagHelpers.TagHelpers.WvFieldRadioList", "WebVella.TagHelpers");
					var scriptEl = new TagBuilder("script");
					scriptEl.Attributes.Add("type", "text/javascript");
					scriptEl.InnerHtml.AppendHtml(scriptContent);
					output.PostContent.AppendHtml(scriptEl);

					ViewContext.HttpContext.Items[typeof(WvFieldRadioList) + fileName] = new WvTagHelperContext()
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
							RadioListFormInit(""{{FieldId}}"",{{IsButtonGroup}},""{{ButtonClassUnchecked}}"",""{{ButtonClassChecked}}"");
						});";
				scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
				scriptTemplate = scriptTemplate.Replace("{{IsButtonGroup}}", IsButtonGroup.ToString().ToLowerInvariant());
				scriptTemplate = scriptTemplate.Replace("{{ButtonClassUnchecked}}", ButtonClassUnchecked);
				scriptTemplate = scriptTemplate.Replace("{{ButtonClassChecked}}", ButtonClassChecked);

				initScript.InnerHtml.AppendHtml(scriptTemplate);

				output.PostContent.AppendHtml(initScript);
				#endregion

			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				var wrapper = new TagBuilder("div");
				wrapper.Attributes.Add("id", $"input-{FieldId}");
				wrapper.AddCssClass("form-control-plaintext erp-radio-list");
				if (!String.IsNullOrWhiteSpace(Name))
				{
					wrapper.Attributes.Add("data-field-name", Name);
				}
				wrapper.Attributes.Add("data-field-value", (Value ?? "").ToString().ToLowerInvariant());

				var selectedOption = Options.FirstOrDefault(x => x.Value == Value);
				var renderedValue = Value;
				if (selectedOption != null)
					renderedValue = selectedOption.Label;

				wrapper.InnerHtml.AppendHtml(renderedValue);
				output.Content.AppendHtml(wrapper);

			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				var selectedOption = Options.FirstOrDefault(x => x.Value == Value);
				var renderedValue = Value;
				if (selectedOption != null)
					renderedValue = selectedOption.Label;

				output.Content.AppendHtml(renderedValue);
			}
			else if (Mode == WvFieldRenderMode.InlineEdit)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					//View
					{
						var viewWrapper = new TagBuilder("div");
						viewWrapper.Attributes.Add("id", $"view-{FieldId}");
						viewWrapper.AddCssClass("input-group view-wrapper");


						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass($"form-control erp-radio-list {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");
						viewFormControlEl.Attributes.Add("title", "double click to edit");

						var selectedOption = Options.FirstOrDefault(x => x.Value == Value);
						var renderedValue = Value;
						if (selectedOption != null)
							renderedValue = selectedOption.Label;

						viewFormControlEl.InnerHtml.AppendHtml(renderedValue);
						viewWrapper.InnerHtml.AppendHtml(viewFormControlEl);


						var appendEl = new TagBuilder("span");
						appendEl.AddCssClass("input-group-append action");
						var appendButton = new TagBuilder("button");
						appendButton.Attributes.Add("type", "button");
						appendButton.Attributes.Add("title", "edit");
						appendButton.AddCssClass("btn btn-secondary");
						var appendIcon = new TagBuilder("span");
						appendIcon.AddCssClass("fa fa-fw fa-pencil-alt");
						appendButton.InnerHtml.AppendHtml(appendIcon);
						appendEl.InnerHtml.AppendHtml(appendButton);
						viewWrapper.InnerHtml.AppendHtml(appendEl);

						output.Content.AppendHtml(viewWrapper);
					}

					//Edit
					{
						var editWrapper = new TagBuilder("div");
						editWrapper.Attributes.Add("id", $"edit-{FieldId}");
						editWrapper.AddCssClass("edit-wrapper");
						editWrapper.Attributes.Add("style", "display:none;");
						var inputGroupEl = new TagBuilder("div");
						inputGroupEl.AddCssClass("input-group erp-checkbox-list");
						var editFormControlEl = new TagBuilder("div");
						editFormControlEl.AddCssClass("form-control");
						foreach (var selectOption in Options)
						{

							var editFormCheckEl = new TagBuilder("div");
							editFormCheckEl.AddCssClass("form-check form-check-inline ml-1");
							var editFormLabelEl = new TagBuilder("label");
							editFormLabelEl.AddCssClass("form-check-label");

							var editInputEl = new TagBuilder("input");
							editInputEl.Attributes.Add("type", "radio");
							editInputEl.Attributes.Add("value", selectOption.Value);
							editInputEl.AddCssClass("form-check-input");
							editInputEl.Attributes.Add("name", Name);
							if (Value != null && Value == selectOption.Value)
							{
								editInputEl.Attributes.Add("checked", "checked");
							}

							editFormLabelEl.InnerHtml.AppendHtml(editInputEl);
							editFormLabelEl.InnerHtml.AppendHtml(selectOption.Label);
							editFormCheckEl.InnerHtml.AppendHtml(editFormLabelEl);
							editFormControlEl.InnerHtml.AppendHtml(editFormCheckEl);
						}
						inputGroupEl.InnerHtml.AppendHtml(editFormControlEl);

						var editInputAppend = new TagBuilder("span");
						editInputAppend.AddCssClass("input-group-append");
						var editSaveBtn = new TagBuilder("button");
						editSaveBtn.Attributes.Add("type", "submit");
						editSaveBtn.AddCssClass("btn btn-secondary save");
						editSaveBtn.Attributes.Add("title", "Save");
						var editSaveIcon = new TagBuilder("span");
						editSaveIcon.AddCssClass("fa fa-fw fa-check go-green");
						editSaveBtn.InnerHtml.AppendHtml(editSaveIcon);
						editInputAppend.InnerHtml.AppendHtml(editSaveBtn);

						var editCancelBtn = new TagBuilder("button");
						editCancelBtn.Attributes.Add("type", "submit");
						editCancelBtn.AddCssClass("btn btn-secondary cancel");
						editCancelBtn.Attributes.Add("title", "Cancel");
						var editCancelIcon = new TagBuilder("span");
						editCancelIcon.AddCssClass("fa fa-fw fa-times go-gray");
						editCancelBtn.InnerHtml.AppendHtml(editCancelIcon);
						editInputAppend.InnerHtml.AppendHtml(editCancelBtn);

						inputGroupEl.InnerHtml.AppendHtml(editInputAppend);
						editWrapper.InnerHtml.AppendHtml(inputGroupEl);
						output.Content.AppendHtml(editWrapper);

						#region << Init Scripts >>
						var tagHelperInitialized = false;
						if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldRadioList) + "-inline-edit"))
						{
							var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldRadioList) + "-inline-edit"];
							tagHelperInitialized = tagHelperContext.Initialized;
						}
						if (!tagHelperInitialized)
						{
							var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldRadioList", "WebVella.TagHelpers");
							var scriptEl = new TagBuilder("script");
							scriptEl.Attributes.Add("type", "text/javascript");
							scriptEl.InnerHtml.AppendHtml(scriptContent);
							output.PostContent.AppendHtml(scriptEl);

							ViewContext.HttpContext.Items[typeof(WvFieldRadioList) + "-inline-edit"] = new WvTagHelperContext()
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
							RadioListInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
						scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
						scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);


						var fieldConfig = new WvFieldCheckboxConfig()
						{
							ApiUrl = ApiUrl,
							CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false
						};

						scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

						initScript.InnerHtml.AppendHtml(scriptTemplate);

						output.PostContent.AppendHtml(initScript);
						#endregion
					}
				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					var divEl = new TagBuilder("div");
					divEl.AddCssClass("input-group");

					var prependWrapper = new TagBuilder("span");
					prependWrapper.AddCssClass("input-group-prepend");

					var prependEl = new TagBuilder("span");
					prependEl.AddCssClass("input-group-text");

					var prependIcon = new TagBuilder("span");
					prependIcon.AddCssClass($"fa fa-fw fa-{(Value ? "check" : "times")}");
					prependEl.InnerHtml.AppendHtml(prependIcon);
					prependWrapper.InnerHtml.AppendHtml(prependEl);
					divEl.InnerHtml.AppendHtml(prependWrapper);

					var selectedOption = Options.FirstOrDefault(x => x.Value == Value);
					var renderedValue = Value;
					if (selectedOption != null)
						renderedValue = selectedOption.Label;

					var inputEl = new TagBuilder("input");
					inputEl.AddCssClass("form-control erp-radio-list");
					inputEl.Attributes.Add("type", "text");
					inputEl.Attributes.Add("value", renderedValue);
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
