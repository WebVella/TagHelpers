using HtmlAgilityPack;
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
	[HtmlTargetElement("wv-field-select")]
	[RestrictChildren("wv-field-prepend", "wv-field-append")]
	public class WvFieldSelect : WvFieldBase
	{
		[HtmlAttributeName("options")]
		public List<WvSelectOption> Options { get; set; } = new List<WvSelectOption>();

		[HtmlAttributeName("ajax-datasource")]
		public WvSelectOptionsAjaxDatasource AjaxDatasource { get; set; } = null;

		[HtmlAttributeName("ajax-datasource-api")]
		public string AjaxDatasourceApi { get; set; } = "";

		[HtmlAttributeName("select-match-type")]
		public WvSelectMatchType SelectMatchType { get; set; } = WvSelectMatchType.Contains;
		/*
		 * The link that will be opened.
		 * Feature: LInkable Text Field
		 *Author: Amarjeet-L
		 */
		[HtmlAttributeName("link")]
		public string Link { get; set; } = null;

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

			if (Options == null)
				Options = new List<WvSelectOption>();

			if (Options.Count == 0 && AjaxDatasource != null && AjaxDatasource.InitOptions.Count > 0)
				Options = AjaxDatasource.InitOptions;

			if (AjaxDatasource != null && String.IsNullOrWhiteSpace(AjaxDatasourceApi))
			{
				if (!AjaxDatasource.UseSelectApi)
				{
					//old fashion call
					AjaxDatasourceApi = "/api/v3/en_US/eql-ds";
				}
				else
				{
					//The new api
					AjaxDatasourceApi = "/api/v3/en_US/eql-ds-select2";
				}
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

			#region << Validate Options >>
			if (Options == null)
			{
				var divEl = new TagBuilder("div");
				divEl.AddCssClass("form-control-plaintext erp-plain-text");

				var errorListEl = new TagBuilder("ul");
				errorListEl.AddCssClass("erp-error-list list-unstyled");

				var errorEl = new TagBuilder("li");
				errorEl.AddCssClass("go-red");

				var iconEl = new TagBuilder("span");
				iconEl.AddCssClass("fa fa-fw fa-exclamation");

				errorEl.InnerHtml.AppendHtml(iconEl);
				errorEl.InnerHtml.Append($"Error: Select options cannot be null");

				errorListEl.InnerHtml.AppendHtml(errorEl);
				divEl.InnerHtml.AppendHtml(errorListEl);
				output.Content.AppendHtml(divEl);
				//Finally
				if (SubInputEl != null)
				{
					output.PostContent.AppendHtml(SubInputEl);
				}
				return;
			}
			#endregion

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					var inputGroupEl = new TagBuilder("div");
					if (PrependHtml.Count > 0 || AppendHtml.Count > 0)
					{
						inputGroupEl.AddCssClass("input-group");
					}
					else
					{
						inputGroupEl.AddCssClass("");
					}
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
					var selectEl = new TagBuilder("select");
					selectEl.Attributes.Add("id", $"input-{FieldId}");
					selectEl.Attributes.Add("name", $"{Name}");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-select");
					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}
					selectEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));
					var emptyOptionAdded = false;
					if (Required)
					{
						selectEl.Attributes.Add("required", null);

						if (String.IsNullOrWhiteSpace(Value))
						{
							var defaultOption = Options.FirstOrDefault(x => x.Value == DefaultValue);

							if (defaultOption != null)
							{
								Value = DefaultValue;
							}
						}
					}
					else
					{
						var optionEl = new TagBuilder("option");
						// Should work only with <option></option> and the select2 placeholder to be presented
						selectEl.InnerHtml.AppendHtml(optionEl);
						emptyOptionAdded = true;
					}

					foreach (var option in Options)
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", option.Value);
						if (option.Value == (Value ?? "").ToString())
						{
							optionEl.Attributes.Add("selected", null);
						}
						optionEl.Attributes.Add("data-icon", option.IconClass);
						optionEl.Attributes.Add("data-color", option.Color);
						optionEl.InnerHtml.Append(option.Label);
						selectEl.InnerHtml.AppendHtml(optionEl);
					}

					//At least one option should be in the select so it can submit
					if (!emptyOptionAdded && Options.Count == 0)
					{
						var optionEl = new TagBuilder("option");
						// Should work only with <option></option> and the select2 placeholder to be presented
						selectEl.InnerHtml.AppendHtml(optionEl);
					}

					inputGroupEl.InnerHtml.AppendHtml(selectEl);
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

					#region << Init Select2 >>
					{
						var wvLibraryInitialized = false;
						var libraryItemsKey = "WebVella-" + "select2";
						if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
						{
							var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
							wvLibraryInitialized = tagHelperContext.Initialized;
						}

						if (!wvLibraryInitialized)
						{
							{
								var libCssEl = new TagBuilder("link");
								libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/select2/css/select2.min.css");
								libCssEl.Attributes.Add("type", "text/css");
								libCssEl.Attributes.Add("rel", "stylesheet");
								output.PostContent.AppendHtml(libCssEl);
								output.PostContent.AppendHtml("\r\n\t");
							}
							{
								var libCssEl = new TagBuilder("link");
								libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/select2-bootstrap-theme/select2-bootstrap4.css");
								libCssEl.Attributes.Add("type", "text/css");
								libCssEl.Attributes.Add("rel", "stylesheet");
								output.PostContent.AppendHtml(libCssEl);
								output.PostContent.AppendHtml("\r\n\t");
							}

							var libJsEl = new TagBuilder("script");
							libJsEl.Attributes.Add("type", "text/javascript");
							libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/select2/js/select2.min.js");
							output.PostContent.AppendHtml(libJsEl);
							output.PostContent.AppendHtml("\r\n\t");

							ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
							{
								Initialized = true
							};
						}
					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldSelect) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldSelect) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldSelect", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldSelect) + "-form"] = new WvTagHelperContext()
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
							SelectFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldSelectConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						IsInvalid = ValidationErrors.Count > 0,
						AjaxDatasource = AjaxDatasource,
						AjaxDatasourceApi = AjaxDatasourceApi,
						SelectMatchType = SelectMatchType,
						Placeholder = Placeholder
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion


					//TODO Implement CanAddValues
					//@if (config.CanAddValues ?? false)
					//{
					//<div id="add-option-modal-@fieldMeta.Id" class="modal" data-backdrop="true">
					//	<div class="modal-dialog" name="add-option">
					//		<div class="modal-content">
					//			<div class="modal-header">
					//				<h5 class="modal-title">Add @(fieldMeta.Label)</h5>
					//			</div>
					//			<div class="modal-body">
					//				<div class="alert alert-danger d-none"></div>
					//				<div class="form-group">
					//					<label class="control-label">New value</label>
					//					<input class="form-control erp-select add-option-input" value="" required />
					//				</div>
					//			</div>
					//			<div class="modal-footer">
					//				<button class="btn btn-primary btn-sm" type="submit"><i class="fa fa-plus-circle"></i> Add</button>
					//				<button class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
					//			</div>
					//		</div>
					//	</div>
					//</div>

					//}



				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					//Have to render it as a normal select as readonly prop does not work with select 2. Also in order for the select not to work it should be disabled,
					//which will not pass the value, this the hidden input

					//Hidden input
					var hiddenEl = new TagBuilder("input");
					hiddenEl.Attributes.Add("id", $"input-{FieldId}");
					hiddenEl.Attributes.Add("name", $"{Name}");
					hiddenEl.Attributes.Add("value", (Value ?? "").ToString());
					hiddenEl.Attributes.Add("type", "hidden");
					output.Content.AppendHtml(hiddenEl);

					var inputGroupEl = new TagBuilder("div");
					if (PrependHtml.Count > 0 || AppendHtml.Count > 0)
					{
						inputGroupEl.AddCssClass("input-group");
					}
					else
					{
						inputGroupEl.AddCssClass("d-flex");
					}
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

					//Dummy disabled select
					var selectEl = new TagBuilder("select");
					selectEl.Attributes.Add("id", $"select-{FieldId}");
					selectEl.Attributes.Add("readonly", null);
					selectEl.Attributes.Add("disabled", "disabled");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-select");
					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}
					selectEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));
					if (Required)
					{
						selectEl.Attributes.Add("required", null);

						if (String.IsNullOrWhiteSpace(Value))
						{
							var defaultOption = Options.FirstOrDefault(x => x.Value == DefaultValue);

							if (defaultOption != null)
							{
								Value = DefaultValue;
							}
						}
					}
					else
					{
						var optionEl = new TagBuilder("option");
						// Should work only with <option></option> and the select2 placeholder to be presented
						//optionEl.Attributes.Add("value", "");
						//if (String.IsNullOrWhiteSpace(Value))
						//{
						//    optionEl.Attributes.Add("selected", null);
						//}
						//optionEl.InnerHtml.Append("not selected");
						selectEl.InnerHtml.AppendHtml(optionEl);
					}

					foreach (var option in Options)
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", option.Value);
						if (option.Value == (Value ?? "").ToString())
						{
							optionEl.Attributes.Add("selected", null);
						}
						optionEl.Attributes.Add("data-icon", option.IconClass);
						optionEl.Attributes.Add("data-color", option.Color);
						optionEl.InnerHtml.Append(option.Label);
						selectEl.InnerHtml.AppendHtml(optionEl);
					}
					inputGroupEl.InnerHtml.AppendHtml(selectEl);
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
				var valueString = (Value ?? "").ToString();
				if (!String.IsNullOrWhiteSpace(valueString))
				{

					var formControlEl = new TagBuilder("div");
					formControlEl.Attributes.Add("id", $"input-{FieldId}");
					formControlEl.AddCssClass("form-control-plaintext erp-select");

					var optionEl = new TagBuilder("span");
					var selectedOption = Options.FirstOrDefault(x => x.Value == valueString);
					if (selectedOption == null)
					{
						optionEl.InnerHtml.Append(valueString);
						if (Value != null)
						{
							var optionElIcon = new TagBuilder("span");
							optionElIcon.AddCssClass("fa fa-fw fa-exclamation-circle go-red");
							optionElIcon.Attributes.Add("title", "the value is not supported by this field anymore");
							optionEl.InnerHtml.AppendHtml(optionElIcon);
						}
					}
					else
					{
						optionEl.Attributes.Add("title", selectedOption.Label);
						optionEl.Attributes.Add("data-key", valueString);
						if (String.IsNullOrWhiteSpace(selectedOption.IconClass))
						{
							optionEl.InnerHtml.Append(selectedOption.Label);
						}
						else
						{
							var color = "#999";
							if (!String.IsNullOrWhiteSpace(selectedOption.Color))
								color = selectedOption.Color;

							optionEl.InnerHtml.AppendHtml($"<i class=\"{selectedOption.IconClass}\" style=\"color:{color}\"></i> {selectedOption.Label}");
						}
					}

					formControlEl.InnerHtml.AppendHtml(optionEl);

					output.Content.AppendHtml(formControlEl);
				}
				else
				{
					output.Content.AppendHtml(EmptyValEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				var valueString = (Value ?? "").ToString();

				if (String.IsNullOrWhiteSpace(valueString))
				{
					output.Content.AppendHtml("");
				}
				else
				{
					var selectedOption = Options.FirstOrDefault(x => x.Value == valueString);
					if (selectedOption == null)
					{
						output.Content.Append(valueString);
						// in simple mode exclamation mark should not be rendered
						//if (Value != null && (Value.ToString() != ""))
						//{
						//	var optionElIcon = new TagBuilder("span");
						//	optionElIcon.AddCssClass("fa fa-fw fa-exclamation-circle go-red");
						//	optionElIcon.Attributes.Add("title", "the value is not supported by this field anymore");
						//	output.Content.AppendHtml(optionElIcon);
						//}
					}
					else
					{
						if (String.IsNullOrWhiteSpace(selectedOption.IconClass))
						{
							output.Content.Append(selectedOption.Label);
						}
						else
						{
							var color = "#999";
							if (!String.IsNullOrWhiteSpace(selectedOption.Color))
								color = selectedOption.Color;

							output.Content.AppendHtml($"<i class=\"{selectedOption.IconClass}\" style=\"color:{color}\"></i> {selectedOption.Label}");
						}
					}
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
						viewFormControlEl.AddCssClass("form-control erp-select");

						var selectedOption = Options.FirstOrDefault(x => x.Value == (Value ?? "").ToString());
						if (selectedOption == null)
						{
							viewFormControlEl.InnerHtml.Append((Value ?? "").ToString());
							if (Value != null && (Value.ToString() != ""))
							{
								var optionElIcon = new TagBuilder("span");
								optionElIcon.AddCssClass("fa fa-fw fa-exclamation-circle go-red");
								optionElIcon.Attributes.Add("title", "the value is not supported by this field anymore");
								viewFormControlEl.InnerHtml.AppendHtml(optionElIcon);
							}
						}
						else
						{
							if (String.IsNullOrWhiteSpace(selectedOption.IconClass))
							{
								viewFormControlEl.InnerHtml.Append(selectedOption.Label);
							}
							else
							{
								var color = "#999";
								if (!String.IsNullOrWhiteSpace(selectedOption.Color))
									color = selectedOption.Color;

								viewFormControlEl.InnerHtml.AppendHtml($"<i class=\"{selectedOption.IconClass}\" style=\"color:{color}\"></i> {selectedOption.Label}");
							}
						}

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
						/*
					 * Append an anchor field with the provided link value, if it is present
					 * Feature: LInkable Text Field
					 *Author: Amarjeet-L
					 */
						if (Link != "" && Link != null)
						{
							var linkInputActionEl = new TagBuilder("span");
							linkInputActionEl.AddCssClass("input-group-append");
							foreach (var htmlString in AppendHtml)
							{
								linkInputActionEl.InnerHtml.AppendHtml(htmlString);
							}
							linkInputActionEl.InnerHtml.AppendHtml("<a href='" + Link + "' target='_blank' class='btn btn-white' title='details'><i class='fas fa-external-link-alt'></i></a>");
							viewWrapperEl.InnerHtml.AppendHtml(linkInputActionEl);
						}
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
						var formControl = new TagBuilder("div");
						var inputElCssClassList = new List<string>();
						inputElCssClassList.Add("form-control erp-select");
						if (ValidationErrors.Count > 0)
						{
							inputElCssClassList.Add("is-invalid");
						}
						formControl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

						var selectEl = new TagBuilder("select");
						selectEl.Attributes.Add("id", $"input-{FieldId}");
						selectEl.Attributes.Add("name", $"{Name}");

						var emptyOptionAdded = false;
						if (Required)
						{
							selectEl.Attributes.Add("required", null);
						}
						else
						{
							var optionEl = new TagBuilder("option");
							// Should work only with <option></option> and the select2 placeholder to be presented
							selectEl.InnerHtml.AppendHtml(optionEl);
							emptyOptionAdded = true;
						}

						selectEl.Attributes.Add("data-original-value", JsonSerializer.Serialize((Value ?? "").ToString()));

						foreach (var option in Options)
						{
							var optionEl = new TagBuilder("option");
							optionEl.Attributes.Add("value", option.Value);
							optionEl.Attributes.Add("data-icon", option.IconClass);
							optionEl.Attributes.Add("data-color", option.Color);
							if (option.Value == (Value ?? "").ToString())
							{
								optionEl.Attributes.Add("selected", null);
							}
							optionEl.InnerHtml.Append(option.Label);
							selectEl.InnerHtml.AppendHtml(optionEl);
						}

						//At least one option should be in the select so it can submit
						if (!emptyOptionAdded && Options.Count == 0)
						{
							var optionEl = new TagBuilder("option");
							// Should work only with <option></option> and the select2 placeholder to be presented
							selectEl.InnerHtml.AppendHtml(optionEl);
						}

						formControl.InnerHtml.AppendHtml(selectEl);
						editInputGroupEl.InnerHtml.AppendHtml(formControl);

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

					#region << Init Select2 >>
					{
						var wvLibraryInitialized = false;
						var libraryItemsKey = "WebVella-" + "select2";
						if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
						{
							var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
							wvLibraryInitialized = tagHelperContext.Initialized;
						}

						if (!wvLibraryInitialized)
						{
							{
								var libCssEl = new TagBuilder("link");
								libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/select2/css/select2.min.css");
								libCssEl.Attributes.Add("type", "text/css");
								libCssEl.Attributes.Add("rel", "stylesheet");
								output.PostContent.AppendHtml(libCssEl);
								output.PostContent.AppendHtml("\r\n\t");
							}
							{
								var libCssEl = new TagBuilder("link");
								libCssEl.Attributes.Add("href", "/_content/WebVella.TagHelpers/lib/select2-bootstrap-theme/select2-bootstrap4.css");
								libCssEl.Attributes.Add("type", "text/css");
								libCssEl.Attributes.Add("rel", "stylesheet");
								output.PostContent.AppendHtml(libCssEl);
								output.PostContent.AppendHtml("\r\n\t");
							}

							var libJsEl = new TagBuilder("script");
							libJsEl.Attributes.Add("type", "text/javascript");
							libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/select2/js/select2.min.js");
							output.PostContent.AppendHtml(libJsEl);
							output.PostContent.AppendHtml("\r\n\t");

							ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
							{
								Initialized = true
							};
						}
					}
					#endregion

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldSelect) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldSelect) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldSelect", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldSelect) + "-inline-edit"] = new WvTagHelperContext()
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
							SelectInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);


					var fieldConfig = new WvFieldSelectConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						IsInvalid = ValidationErrors.Count > 0,
						AjaxDatasource = AjaxDatasource,
						AjaxDatasourceApi = AjaxDatasourceApi,
						SelectMatchType = SelectMatchType,
						Placeholder = Placeholder
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
					var formControlEl = new TagBuilder("div");
					formControlEl.AddCssClass("form-control erp-select");

					var optionEl = new TagBuilder("span");
					optionEl.Attributes.Add("selected", null);
					var selectedOption = Options.FirstOrDefault(x => x.Value == (Value ?? "").ToString());
					if (selectedOption == null)
					{
						optionEl.InnerHtml.Append((Value ?? "").ToString());
						if (Value != null && (Value.ToString() != ""))
						{
							var optionElIcon = new TagBuilder("span");
							optionElIcon.AddCssClass("fa fa-fw fa-exclamation-circle go-red");
							optionElIcon.Attributes.Add("title", "the value is not supported by this field anymore");
							optionEl.InnerHtml.AppendHtml(optionElIcon);
						}
					}
					else
					{
						optionEl.Attributes.Add("title", selectedOption.Label);
						optionEl.Attributes.Add("data-key", (Value ?? "").ToString());
						if (String.IsNullOrWhiteSpace(selectedOption.IconClass))
						{
							optionEl.InnerHtml.Append(selectedOption.Label);
						}
						else
						{
							var color = "#999";
							if (!String.IsNullOrWhiteSpace(selectedOption.Color))
								color = selectedOption.Color;

							optionEl.InnerHtml.AppendHtml($"<i class=\"{selectedOption.IconClass}\" style=\"color:{color}\"></i> {selectedOption.Label}");
						}

					}
					formControlEl.InnerHtml.AppendHtml(optionEl);

					divEl.InnerHtml.AppendHtml(formControlEl);
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
