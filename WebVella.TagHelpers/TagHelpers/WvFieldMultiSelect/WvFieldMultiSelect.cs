﻿using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-field-multiselect")]
	[RestrictChildren("wv-field-prepend", "wv-field-append")]
	public class WvFieldMultiSelect : WvFieldBase
	{
		[HtmlAttributeName("options")]
		public List<WvSelectOption> Options { get; set; } = new List<WvSelectOption>();

		[HtmlAttributeName("ajax-datasource")]
		public WvSelectOptionsAjaxDatasource AjaxDatasource { get; set; } = null;

		[HtmlAttributeName("ajax-datasource-api")]
		public string AjaxDatasourceApi { get; set; } = "";

		[HtmlAttributeName("select-match-type")]
		public WvSelectMatchType SelectMatchType { get; set; } = WvSelectMatchType.Contains;

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
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
				errorEl.InnerHtml.Append($"Error:Select options cannot be null");

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
					inputGroupEl.AddCssClass("input-group");
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
					//Hidden Control to be posted when no option is selected
					var dummyHiddenInput = new TagBuilder("input");
					dummyHiddenInput.Attributes.Add("type", "hidden");
					dummyHiddenInput.Attributes.Add("id", $"dummy-{FieldId}");
					if (Value == null || ((List<string>)Value).Count == 0)
					{
						dummyHiddenInput.Attributes.Add("name", $"{Name}");
					}
					dummyHiddenInput.Attributes.Add("value", "");
					inputGroupEl.InnerHtml.AppendHtml(dummyHiddenInput);

					//Control
					var selectEl = new TagBuilder("select");
					selectEl.Attributes.Add("id", $"input-{FieldId}");
					//Name will be attached and removed depending on if there is selected values or not, in order for the dummy to post
					if (Value != null && ((List<string>)Value).Count > 0)
					{
						selectEl.Attributes.Add("name", $"{Name}");
					}
					selectEl.Attributes.Add("data-field-name", $"{Name}");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-multiselect invisible");
					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}
					selectEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));

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

					selectEl.Attributes.Add("multiple", "multiple");

					foreach (var option in Options)
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", option.Value);
						Debug.WriteLine("============================");
						string valueJson = JsonSerializer.Serialize(Value);
						string optionValueJson = JsonSerializer.Serialize(option.Value);
						Debug.WriteLine("value: " + valueJson);
						if (Value != null && ((List<string>)Value).Any(x => x == option.Value))
						{
							optionEl.Attributes.Add("selected", null);
							Debug.WriteLine("selected: " + optionValueJson);
						}
						optionEl.Attributes.Add("data-icon", option.IconClass);
						optionEl.Attributes.Add("data-color", option.Color);
						optionEl.Attributes.Add("data-value", JsonSerializer.Serialize(Value));
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
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldMultiSelect) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldMultiSelect) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldMultiSelect", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						//scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldMultiSelect) + "-form"] = new WvTagHelperContext()
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
							MultiSelectFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldMultiSelectConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
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
					//	<div id="add-option-modal-@fieldMeta.Id" class="modal" data-backdrop="true">
					//		<div class="modal-dialog" name="add-option">
					//			<div class="modal-content">
					//				<div class="modal-header">
					//					<h5 class="modal-title">Add @(fieldMeta.Label)</h5>
					//				</div>
					//				<div class="modal-body">
					//					<div class="alert alert-danger d-none"></div>
					//					<div class="form-group">
					//						<label class="control-label">New value</label>
					//						<input class="form-control erp-multiselect add-option-input" value="" required/>
					//					</div>
					//				</div>
					//				<div class="modal-footer">
					//					<button class="btn btn-primary btn-sm" type="submit"><i class="fa fa-plus-circle"></i> Add</button>
					//					<button class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
					//				</div>
					//			</div>
					//		</div>
					//	</div>

					//}



				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					//Have to render it as a normal select as readonly prop does not work with select 2. Also in order for the select not to work it should be disabled,
					//which will not pass the value, this the hidden input

					var hiddenInput = new TagBuilder("input");
					hiddenInput.Attributes.Add("type", "hidden");
					hiddenInput.Attributes.Add("id", $"input-{FieldId}");
					hiddenInput.Attributes.Add("name", $"{Name}");
					hiddenInput.Attributes.Add("value", (Value ?? "").ToString());
					output.Content.AppendHtml(hiddenInput);


					var inputGroupEl = new TagBuilder("div");
					inputGroupEl.AddCssClass("input-group");
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
					selectEl.Attributes.Add("id", $"select-{FieldId}");
					selectEl.Attributes.Add("readonly", null);
					selectEl.Attributes.Add("disabled", $"disabled");
					var inputElCssClassList = new List<string>();
					inputElCssClassList.Add("form-control erp-multiselect invisible");
					if (ValidationErrors.Count > 0)
					{
						inputElCssClassList.Add("is-invalid");
					}
					selectEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));
					if (Required)
					{
						selectEl.Attributes.Add("required", null);
					}

					selectEl.Attributes.Add("multiple", "multiple");

					foreach (var option in Options)
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", option.Value);
						if (Value != null && ((List<string>)Value).Any(x => x == option.Value))
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

				if (((List<string>)Value).Count > 0)
				{


					var select2ContainerEl = new TagBuilder("span");
					select2ContainerEl.Attributes.Add("id", $"input-{FieldId}");
					select2ContainerEl.AddCssClass("select2 select2-container select2-container--bootstrap4 d-block disabled clean");

					var select2SelectionEl = new TagBuilder("span");
					select2SelectionEl.AddCssClass("selection");

					var select2SelectionInnerEl = new TagBuilder("span");
					select2SelectionInnerEl.AddCssClass("select2-selection select2-selection--multiple d-flex");

					var select2SelectionUlEl = new TagBuilder("ul");
					select2SelectionUlEl.AddCssClass("select2-selection__rendered");

					foreach (var dataKey in Value)
					{
						var optionEl = new TagBuilder("li");
						var option = Options.FirstOrDefault(x => x.Value == dataKey);
						if (option == null)
						{
							optionEl.AddCssClass("select2-selection__choice missing");
							optionEl.Attributes.Add("title", dataKey);
							optionEl.Attributes.Add("data-key", dataKey);
							optionEl.InnerHtml.Append(dataKey);
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
							optionEl.AddCssClass("select2-selection__choice");
							optionEl.Attributes.Add("title", option.Label);
							optionEl.Attributes.Add("data-key", dataKey);
							if (String.IsNullOrWhiteSpace(option.IconClass))
							{
								optionEl.InnerHtml.Append(option.Label);
							}
							else
							{
								var color = "#999";
								if (!String.IsNullOrWhiteSpace(option.Color))
									color = option.Color;

								optionEl.InnerHtml.AppendHtml($"<i class=\"{option.IconClass}\" style=\"color:{color}\"></i> {option.Label}");
							}

						}
						select2SelectionUlEl.InnerHtml.AppendHtml(optionEl);
					}

					select2SelectionInnerEl.InnerHtml.AppendHtml(select2SelectionUlEl);
					select2SelectionEl.InnerHtml.AppendHtml(select2SelectionInnerEl);
					select2ContainerEl.InnerHtml.AppendHtml(select2SelectionEl);

					output.Content.AppendHtml(select2ContainerEl);
				}
				else
				{
					output.Content.AppendHtml(EmptyValEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				var valueLabels = new List<string>();
				foreach (var dataKey in (List<string>)Value)
				{
					var option = Options.FirstOrDefault(x => x.Value == dataKey);
					if (option == null)
					{
						valueLabels.Add(dataKey);
					}
					else
					{
						if (String.IsNullOrWhiteSpace(option.IconClass))
						{
							valueLabels.Add(option.Label);
						}
						else
						{
							var color = "#999";
							if (!String.IsNullOrWhiteSpace(option.Color))
								color = option.Color;

							valueLabels.Add($"<i class=\"{option.IconClass}\" style=\"color:{color}\"></i> {option.Label}");
						}
					}
				}

				output.Content.AppendHtml(String.Join(", ", valueLabels));
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
						viewFormControlEl.AddCssClass("form-control erp-multiselect");

						var select2ContainerEl = new TagBuilder("span");
						select2ContainerEl.AddCssClass("select2 select2-container select2-container--bootstrap4 d-block disabled");

						var select2SelectionEl = new TagBuilder("span");
						select2SelectionEl.AddCssClass("selection");

						var select2SelectionInnerEl = new TagBuilder("span");
						select2SelectionInnerEl.AddCssClass("select2-selection select2-selection--multiple");

						var select2SelectionUlEl = new TagBuilder("ul");
						select2SelectionUlEl.AddCssClass("select2-selection__rendered");

						foreach (var dataKey in Value)
						{
							var optionEl = new TagBuilder("li");
							var option = Options.FirstOrDefault(x => x.Value == dataKey);
							if (option == null)
							{
								optionEl.AddCssClass("select2-selection__choice missing");
								optionEl.Attributes.Add("title", dataKey);
								optionEl.Attributes.Add("data-key", dataKey);
								optionEl.InnerHtml.Append(dataKey);
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
								optionEl.AddCssClass("select2-selection__choice");
								optionEl.Attributes.Add("title", option.Label);
								optionEl.Attributes.Add("data-key", dataKey);
								if (String.IsNullOrWhiteSpace(option.IconClass))
								{
									optionEl.InnerHtml.Append(option.Label);
								}
								else
								{
									var color = "#999";
									if (!String.IsNullOrWhiteSpace(option.Color))
										color = option.Color;

									optionEl.InnerHtml.AppendHtml($"<i class=\"{option.IconClass}\" style=\"color:{color}\"></i> {option.Label}");
								}
							}
							select2SelectionUlEl.InnerHtml.AppendHtml(optionEl);
						}

						select2SelectionInnerEl.InnerHtml.AppendHtml(select2SelectionUlEl);
						select2SelectionEl.InnerHtml.AppendHtml(select2SelectionInnerEl);
						select2ContainerEl.InnerHtml.AppendHtml(select2SelectionEl);
						viewFormControlEl.InnerHtml.AppendHtml(select2ContainerEl);
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
						inputElCssClassList.Add("form-control erp-multiselect");
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
						selectEl.Attributes.Add("multiple", "multiple");
						selectEl.Attributes.Add("data-original-value", JsonSerializer.Serialize((Value ?? "").ToString()));


						foreach (var option in Options)
						{
							var optionEl = new TagBuilder("option");
							optionEl.Attributes.Add("value", option.Value);
							if (((List<string>)Value).Any(x => x == option.Value))
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
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldMultiSelect) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldMultiSelect) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldMultiSelect", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldMultiSelect) + "-inline-edit"] = new WvTagHelperContext()
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
							MultiSelectInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldMultiSelectConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
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
					var viewFormControlEl = new TagBuilder("div");
					viewFormControlEl.AddCssClass("form-control erp-multiselect");

					var select2ContainerEl = new TagBuilder("span");
					select2ContainerEl.AddCssClass("select2 select2-container select2-container--bootstrap4 d-block disabled");

					var select2SelectionEl = new TagBuilder("span");
					select2SelectionEl.AddCssClass("selection");

					var select2SelectionInnerEl = new TagBuilder("span");
					select2SelectionInnerEl.AddCssClass("select2-selection select2-selection--multiple");

					var select2SelectionUlEl = new TagBuilder("ul");
					select2SelectionUlEl.AddCssClass("select2-selection__rendered");

					foreach (var dataKey in Value)
					{
						var optionEl = new TagBuilder("li");
						var option = Options.FirstOrDefault(x => x.Value == dataKey);
						if (option == null)
						{
							optionEl.AddCssClass("select2-selection__choice missing");
							optionEl.Attributes.Add("title", dataKey);
							optionEl.Attributes.Add("data-key", dataKey);
							optionEl.InnerHtml.Append(dataKey);
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
							optionEl.AddCssClass("select2-selection__choice");
							optionEl.Attributes.Add("title", option.Label);
							optionEl.Attributes.Add("data-key", dataKey);
							if (String.IsNullOrWhiteSpace(option.IconClass))
							{
								optionEl.InnerHtml.Append(option.Label);
							}
							else
							{
								var color = "#999";
								if (!String.IsNullOrWhiteSpace(option.Color))
									color = option.Color;

								optionEl.InnerHtml.AppendHtml($"<i class=\"{option.IconClass}\" style=\"color:{color}\"></i> {option.Label}");
							}
						}
						select2SelectionUlEl.InnerHtml.AppendHtml(optionEl);
					}
					select2SelectionInnerEl.InnerHtml.AppendHtml(select2SelectionUlEl);
					select2SelectionEl.InnerHtml.AppendHtml(select2SelectionInnerEl);
					select2ContainerEl.InnerHtml.AppendHtml(select2SelectionEl);
					viewFormControlEl.InnerHtml.AppendHtml(select2ContainerEl);
					divEl.InnerHtml.AppendHtml(viewFormControlEl);

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
