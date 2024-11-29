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

	[HtmlTargetElement("wv-field-text")]
	[RestrictChildren("wv-field-prepend", "wv-field-append")]
	public class WvFieldText : WvFieldBase
	{
		[HtmlAttributeName("maxlength")]
		public int? MaxLength { get; set; } = null;

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

			#endregion

			#region << Render >>
			if (Mode == WvFieldRenderMode.Form)
			{

				var inputGroupEl = new TagBuilder("div");
				inputGroupEl.AddCssClass("input-group");

				if (Size == WvCssSize.Large)
					inputGroupEl.AddCssClass("input-group-lg");
				if (Size == WvCssSize.Small)
					inputGroupEl.AddCssClass("input-group-sm");

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
				var inputEl = new TagBuilder("input");
				var inputElCssClassList = new List<string>();
				inputElCssClassList.Add("form-control erp-text");

				inputEl.Attributes.Add("type", "text");
				inputEl.Attributes.Add("value", (Value ?? "").ToString());
				inputEl.Attributes.Add("id", $"input-{FieldId}");
				inputEl.Attributes.Add("name", Name);
				if (Access == WvFieldAccess.Full || Access == WvFieldAccess.FullAndCreate)
				{
					if (Required)
					{
						inputEl.Attributes.Add("required", null);
					}
					if (!String.IsNullOrWhiteSpace(Placeholder))
					{
						inputEl.Attributes.Add("placeholder", Placeholder);
					}
					if (MaxLength != null && MaxLength != 0)
					{
						inputEl.Attributes.Add("maxlength", MaxLength.ToString());
					}

					if (AutoComplete != null)
					{
						if (AutoComplete == true)
						{
							var autoCompleteValue = "on";
							if ((new List<string>() { "name" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "name";
							}
							else if ((new List<string>() { "fname", "firstname", "first_name", "first-name", "givenname", "given_name", "given-name" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "given-name";
							}
							else if ((new List<string>() { "mname", "middlename", "middle_name", "middle-name" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "additional-name";
							}
							else if ((new List<string>() { "lname", "lastname", "last_name", "last-name", "familyname", "family_name", "family-name" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "family-name";
							}
							else if ((new List<string>() { "address", "street", "streetaddress", "street_address", "street-address" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "street-address";
							}
							else if ((new List<string>() { "region", "province", "state" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "address-level1";
							}
							else if ((new List<string>() { "city", "town", "place" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "address-level2";
							}
							else if ((new List<string>() { "zip", "zip2", "postal", "postalcode", "postal_code", "postal-code" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "postal-code";
							}
							else if ((new List<string>() { "country" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "country";
							}
							else if ((new List<string>() { "username" }).Contains(Name.ToLowerInvariant()))
							{
								autoCompleteValue = "username";
							}

							inputEl.Attributes.Add("autocomplete", autoCompleteValue);
						}
						else
						{
							inputEl.Attributes.Add("autocomplete", "off");
						}
					}

				}
				else if (Access == WvFieldAccess.ReadOnly)
				{
					inputEl.Attributes.Add("readonly", null);
				}

				if (ValidationErrors.Count > 0)
				{
					inputElCssClassList.Add("is-invalid");
				}
				inputEl.Attributes.Add("class", String.Join(' ', inputElCssClassList));
				inputGroupEl.InnerHtml.AppendHtml(inputEl);
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
			else if (Mode == WvFieldRenderMode.Display)
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

                if (!String.IsNullOrWhiteSpace(Value))
				{
					var divEl = new TagBuilder("div");
					divEl.Attributes.Add("id", $"input-{FieldId}");
					divEl.AddCssClass("form-control-plaintext erp-text");
					if (Size == WvCssSize.Large)
						divEl.AddCssClass("form-control-lg");
					if (Size == WvCssSize.Small)
						divEl.AddCssClass("form-control-sm");

					/*
					 * Append an anchor field with the provided link value, if it is present
					 * Feature: LInkable Text Field
					 *Author: Amarjeet-L
					 */
					if (Link != null && Link != "")
					{
						divEl.InnerHtml.AppendHtml("<a href='" + Link + "' target='_blank'>" + (Value ?? "").ToString() + "</a>");
					}
					else
					{
						divEl.InnerHtml.Append((Value ?? "").ToString());
					}
                    inputGroupEl.InnerHtml.AppendHtml(divEl);
				}
				else
				{
                    inputGroupEl.InnerHtml.AppendHtml(EmptyValEl);
				}

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
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				/*
					 * Append an anchor field with the provided link value, if it is present
					 * Feature: LInkable Text Field
					 *Author: Amarjeet-L
					 */
				if (Link != null && Link != "")
				{
					output.Content.AppendHtml("<a href='" + Link + "' target='_blank'>" + (Value ?? "").ToString() + "</a>");
				}
				else
				{
					output.Content.Append((Value ?? "").ToString());
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
						if (Size == WvCssSize.Large)
							viewWrapperEl.AddCssClass("input-group-lg");
						if (Size == WvCssSize.Small)
							viewWrapperEl.AddCssClass("input-group-sm");

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
						viewFormControlEl.AddCssClass("form-control erp-text");
						viewFormControlEl.InnerHtml.Append((Value ?? "").ToString());
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
						if (Size == WvCssSize.Large)
							editInputGroupEl.AddCssClass("input-group-lg");
						if (Size == WvCssSize.Small)
							editInputGroupEl.AddCssClass("input-group-sm");

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
						var editInputEl = new TagBuilder("input");
						editInputEl.AddCssClass("form-control erp-text");
						if (Required)
						{
							editInputEl.Attributes.Add("required", null);
						}
						if (!String.IsNullOrWhiteSpace(Placeholder))
						{
							editInputEl.Attributes.Add("placeholder", Placeholder);
						}
						if (MaxLength != null && MaxLength != 0)
						{
							editInputEl.Attributes.Add("maxlength", MaxLength.ToString());
						}
						editInputEl.Attributes.Add("value", (Value ?? "").ToString());
						editInputGroupEl.InnerHtml.AppendHtml(editInputEl);

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

					#region << Init Scripts >>
					var tagHelperInitialized = false;
					if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldText) + "-inline-edit"))
					{
						var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldText) + "-inline-edit"];
						tagHelperInitialized = tagHelperContext.Initialized;
					}
					if (!tagHelperInitialized)
					{
						var scriptContent = WvHelpers.GetEmbeddedTextResource("inline-edit.js", "WebVella.TagHelpers.TagHelpers.WvFieldText", "WebVella.TagHelpers");
						var scriptEl = new TagBuilder("script");
						scriptEl.Attributes.Add("type", "text/javascript");
						scriptEl.InnerHtml.AppendHtml(scriptContent);
						output.PostContent.AppendHtml(scriptEl);

						ViewContext.HttpContext.Items[typeof(WvFieldText) + "-inline-edit"] = new WvTagHelperContext()
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
							TextInlineEditInit(""{{FieldId}}"",""{{Name}}"",{{ConfigJson}});
						});";
					scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));
					scriptTemplate = scriptTemplate.Replace("{{Name}}", Name);

					var fieldConfig = new WvFieldTextConfig()
					{
						ApiUrl = ApiUrl,
						CanAddValues = Access == WvFieldAccess.FullAndCreate ? true : false
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
					if (Size == WvCssSize.Large)
						divEl.AddCssClass("input-group-lg");
					if (Size == WvCssSize.Small)
						divEl.AddCssClass("input-group-sm");
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
					var inputEl = new TagBuilder("input");
					inputEl.AddCssClass("form-control erp-text");
					inputEl.Attributes.Add("type", "text");
					inputEl.Attributes.Add("value", (Value ?? "").ToString());
					inputEl.Attributes.Add("readonly", null);
					divEl.InnerHtml.AppendHtml(inputEl);
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
				output.Content.AppendHtml(SubInputEl);
			}

			return;
		}


	}
}
