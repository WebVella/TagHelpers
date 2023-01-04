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

	[HtmlTargetElement("wv-field-icon")]
	public class WvFieldIcon : WvFieldBase
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
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

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					var inputGroupEl = new TagBuilder("div");
					inputGroupEl.AddCssClass("input-group");

					var prependIcon = new TagBuilder("div");
					prependIcon.AddCssClass("input-group-prepend");
					prependIcon.InnerHtml.AppendHtml($"<span class=\"input-group-text\"><i class=\"fa-fw {(Value != null ? Value.ToString() : "")}\"></i></span>");
					inputGroupEl.InnerHtml.AppendHtml(prependIcon);


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
					if (Required)
					{
						selectEl.Attributes.Add("required", null);
					}
					else
					{
						var optionEl = new TagBuilder("option");
						selectEl.InnerHtml.AppendHtml(optionEl);
					}

					if (!String.IsNullOrWhiteSpace((string)Value))
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", Value.ToString());
						optionEl.Attributes.Add("selected", null);
						optionEl.InnerHtml.Append(Value.ToString());
						selectEl.InnerHtml.AppendHtml(optionEl);
					}

					inputGroupEl.InnerHtml.AppendHtml(selectEl);

					output.Content.AppendHtml(inputGroupEl);

					#region << Init FA Icons >>
					{
						var wvFAIconsLibraryInitialized = false;
						var libraryItemsKey = "WebVella-" + "fontawesome-icon-json";
						if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
						{
							var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
							wvFAIconsLibraryInitialized = tagHelperContext.Initialized;
						}

						if (!wvFAIconsLibraryInitialized)
						{
							var libJsEl = new TagBuilder("script");
							libJsEl.Attributes.Add("type", "text/javascript");
							var scriptIconTemplate = @"
								WvFontAwesomeIcons = {{FAIcons}}
							";
							scriptIconTemplate = scriptIconTemplate.Replace("{{FAIcons}}", JsonSerializer.Serialize(WvHelpers.GetAllFontAwesomeIcons()));
							libJsEl.InnerHtml.AppendHtml(scriptIconTemplate);

							output.PostContent.AppendHtml(libJsEl);
							output.PostContent.AppendHtml("\r\n\t");

							ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
							{
								Initialized = true
							};
						}
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
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldIcon) + "-form"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldIcon) + "-form"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("form.js", "WebVella.TagHelpers.TagHelpers.WvFieldIcon", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldIcon) + "-form"] = new WvTagHelperContext()
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
							IconFieldFormInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldSelectConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						IsInvalid = ValidationErrors.Count > 0
					};

					scriptTemplate = scriptTemplate.Replace("{{ConfigJson}}", JsonSerializer.Serialize(fieldConfig));

					initScript.InnerHtml.AppendHtml(scriptTemplate);

					output.PostContent.AppendHtml(initScript);
					#endregion
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
					inputGroupEl.AddCssClass("input-group");

					var prependIcon = new TagBuilder("div");
					prependIcon.AddCssClass("input-group-prepend");
					prependIcon.InnerHtml.AppendHtml($"<span class=\"input-group-text\"><i class=\"fa-fw {(Value != null ? Value.ToString() : "")}\"></i></span>");
					inputGroupEl.InnerHtml.AppendHtml(prependIcon);

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
					}
					if (Value != null)
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", Value.ToString());
						optionEl.Attributes.Add("selected", null);
						optionEl.InnerHtml.Append(Value.ToString());
						selectEl.InnerHtml.AppendHtml(optionEl);
					}

					inputGroupEl.InnerHtml.AppendHtml(selectEl);

					output.Content.AppendHtml(inputGroupEl);
				}
			}
			else if (Mode == WvFieldRenderMode.Display)
			{

				if (Value != null)
				{

					var formControlEl = new TagBuilder("div");
					formControlEl.Attributes.Add("id", $"input-{FieldId}");
					formControlEl.AddCssClass("form-control-plaintext erp-select");

					var optionEl = new TagBuilder("span");
					optionEl.Attributes.Add("data-key", (Value ?? "").ToString());
					var color = "#333";
					optionEl.InnerHtml.AppendHtml($"<i class=\"fa-fw {Value.ToString()}\" style=\"color:{color}\"></i> {Value.ToString()}");

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
				if (Value == null)
				{
					output.Content.AppendHtml("");
				}
				else
				{
					var color = "#333";
					output.Content.AppendHtml($"<i class=\"fa-fw {Value.ToString()}\" style=\"color:{color}\"></i> {Value.ToString()}");
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

						var prependIcon = new TagBuilder("div");
						prependIcon.AddCssClass("input-group-prepend");
						prependIcon.InnerHtml.AppendHtml($"<span class=\"input-group-text\"><i class=\"fa-fw {(Value != null ? Value.ToString() : "")}\"></i></span>");
						viewWrapperEl.InnerHtml.AppendHtml(prependIcon);

						//Control
						var viewFormControlEl = new TagBuilder("div");
						viewFormControlEl.AddCssClass("form-control erp-select");

						if (Value != null)
						{
							viewFormControlEl.InnerHtml.AppendHtml($"{Value.ToString()}");
						}

						viewWrapperEl.InnerHtml.AppendHtml(viewFormControlEl);

						//Append
						var viewInputActionEl = new TagBuilder("span");
						viewInputActionEl.AddCssClass("input-group-append action");
						viewInputActionEl.InnerHtml.AppendHtml("<button type=\"button\" class='btn btn-white ' title='edit'><i class='fa fa-fw fa-pencil-alt'></i></button>");
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

						var prependIcon = new TagBuilder("div");
						prependIcon.AddCssClass("input-group-prepend");
						prependIcon.InnerHtml.AppendHtml($"<span class=\"input-group-text\"><i class=\"fa-fw {(Value != null ? Value.ToString() : "")}\"></i></span>");
						editInputGroupEl.InnerHtml.AppendHtml(prependIcon);


						//Control
						var formControl = new TagBuilder("select");
						var inputElCssClassList = new List<string>();
						inputElCssClassList.Add("form-control erp-select");
						if (ValidationErrors.Count > 0)
						{
							inputElCssClassList.Add("is-invalid");
						}
						formControl.Attributes.Add("class", String.Join(' ', inputElCssClassList));
						formControl.Attributes.Add("id", $"input-{FieldId}");

						if (Required)
						{
							formControl.Attributes.Add("required", null);
						}

						formControl.Attributes.Add("data-original-value", JsonSerializer.Serialize((Value ?? "").ToString()));

						if (Value != null)
						{
							var optionEl = new TagBuilder("option");
							optionEl.Attributes.Add("value", Value.ToString());
							optionEl.Attributes.Add("selected", null);
							optionEl.InnerHtml.Append(Value.ToString());
							formControl.InnerHtml.AppendHtml(optionEl);
						}

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

					#region << Init Libraries >>
					{
						var wvLibraryInitialized = false;
						var libraryItemsKey = "WebVella-" + "fontawesome-icon-json";
						if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
						{
							var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
							wvLibraryInitialized = tagHelperContext.Initialized;
						}

						if (!wvLibraryInitialized)
						{
							var libJsEl = new TagBuilder("script");
							libJsEl.Attributes.Add("type", "text/javascript");
							var scriptIconTemplate = @"
								var WvFontAwesomeIcons = {{FAIcons}}
							";
							scriptIconTemplate = scriptIconTemplate.Replace("{{FAIcons}}", JsonSerializer.Serialize(WvHelpers.GetAllFontAwesomeIcons()));
							libJsEl.InnerHtml.AppendHtml(scriptIconTemplate);

							output.PostContent.AppendHtml(libJsEl);
							output.PostContent.AppendHtml("\r\n\t");

							ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
							{
								Initialized = true
							};
						}
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
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldIcon) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldIcon) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldIcon", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						//scriptEl.InnerHtml.AppendHtml(jsCompressor.Compress(scriptContent));
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldIcon) + "-inline-edit"] = new WvTagHelperContext()
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
							IconFieldInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldSelectConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false,
						IsInvalid = ValidationErrors.Count > 0
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

					var prependIcon = new TagBuilder("div");
					prependIcon.AddCssClass("input-group-prepend");
					prependIcon.InnerHtml.AppendHtml($"<span class=\"input-group-text\"><i class=\"fa-fw {(Value != null ? Value.ToString() : "")}\"></i></span>");
					divEl.InnerHtml.AppendHtml(prependIcon);

					//Control
					var formControlEl = new TagBuilder("div");
					formControlEl.AddCssClass("form-control erp-select");


					if (Value != null)
					{
						var optionEl = new TagBuilder("option");
						optionEl.Attributes.Add("value", Value.ToString());
						optionEl.Attributes.Add("selected", null);
						optionEl.InnerHtml.Append(Value.ToString());
						formControlEl.InnerHtml.AppendHtml(optionEl);
					}

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
