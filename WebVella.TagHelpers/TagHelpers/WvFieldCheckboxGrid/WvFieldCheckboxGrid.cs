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

	[HtmlTargetElement("wv-field-checkbox-grid")]
	public class WvFieldCheckboxGrid : WvFieldBase
	{
		[HtmlAttributeName("text-true")]
		public string TextTrue { get; set; } = "";

		[HtmlAttributeName("text-false")]
		public string TextFalse { get; set; } = "";

		[HtmlAttributeName("rows")]
		public List<WvSelectOption> Rows { get; set; } = new List<WvSelectOption>();

		[HtmlAttributeName("columns")]
		public List<WvSelectOption> Columns { get; set; } = new List<WvSelectOption>();

		protected List<WvKeyStringList> ValueGrid { get; set; } = new List<WvKeyStringList>();

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			//ValueGuid
			if (Value is string && ((string)Value).StartsWith("["))
			{
				ValueGrid = JsonSerializer.Deserialize<List<WvKeyStringList>>((string)Value);
			}
			else
			{
				ValueGrid = new List<WvKeyStringList>();
			}


			#region << Init >>
			var initSuccess = InitField(context, output);

			if (!initSuccess)
			{
				return Task.CompletedTask;
			}

			if (String.IsNullOrWhiteSpace(TextTrue))
			{
				TextTrue = "selected";
			}

			if (String.IsNullOrWhiteSpace(TextFalse))
			{
				TextFalse = "not selected";
			}

			#endregion


			#region << Render >>
			//Check if all columns or rows are with empty string as label
			var allColumnsWithEmptyLabels = true;
			var allRowsWithEmptyLabels = true;
			foreach (var column in Columns)
			{
				if (!String.IsNullOrWhiteSpace(column.Label))
				{
					allColumnsWithEmptyLabels = false;
					break;
				}
			}
			foreach (var row in Rows)
			{
				if (!String.IsNullOrWhiteSpace(row.Label))
				{
					allRowsWithEmptyLabels = false;
					break;
				}
			}

			if (Mode == WvFieldRenderMode.Form)
			{
				var wrapper1 = new TagBuilder("div");
				wrapper1.AddCssClass("form-control-plaintext erp-checkbox-grid erp-list");

				var tableEl = new TagBuilder("table");
				tableEl.AddCssClass("table table-bordered table-hover table-sm mb-0");

				if (!allColumnsWithEmptyLabels)
				{
					var tableHead = new TagBuilder("thead");
					var tableHeadTr = new TagBuilder("tr");
					var tableHeadFirstTh = new TagBuilder("th");
					tableHeadTr.InnerHtml.AppendHtml(tableHeadFirstTh);

					foreach (var column in Columns)
					{
						var tableHeadTh = new TagBuilder("th");
						tableHeadTh.InnerHtml.Append(column.Label);
						tableHeadTr.InnerHtml.AppendHtml(tableHeadTh);
					}
					tableHead.InnerHtml.AppendHtml(tableHeadTr);
					tableEl.InnerHtml.AppendHtml(tableHead);
				}
				var tableBodyEl = new TagBuilder("tbody");
				foreach (var row in Rows)
				{

					var rowValuesIndex = ValueGrid.FindIndex(x => x.Key == row.Value);
					var rowValues = new List<string>();
					if (rowValuesIndex > -1)
					{
						rowValues = ValueGrid[rowValuesIndex].Values;
					}
					var rowTrEl = new TagBuilder("tr");
					if (!allRowsWithEmptyLabels)
					{
						var rowFirstTdEl = new TagBuilder("td");
						rowFirstTdEl.InnerHtml.Append(row.Label);
						rowTrEl.InnerHtml.AppendHtml(rowFirstTdEl);
					}
					foreach (var column in Columns)
					{
						var isChecked = rowValues.Contains(column.Value);
						var columnTdEl = new TagBuilder("td");

						var wrapper2 = new TagBuilder("div");
						wrapper2.AddCssClass("form-check");
						var labelWrapper = new TagBuilder("label");
						labelWrapper.AddCssClass("form-check-label");

						var inputFake = new TagBuilder("input");
						inputFake.Attributes.Add("data-row-key", $"{row.Value}");
						inputFake.Attributes.Add("type", "checkbox");
						inputFake.AddCssClass($"gchk-{FieldId}");
						inputFake.Attributes.Add("value", $"{column.Value}");
						if (Access == WvFieldAccess.ReadOnly)
						{
							inputFake.Attributes.Add("readonly", null);
						}

						inputFake.AddCssClass($"form-check-input {(ValidationErrors.Count > 0 ? "is-invalid" : "")}");

						if (isChecked)
						{
							inputFake.Attributes.Add("checked", "checked");
						}
						labelWrapper.InnerHtml.AppendHtml(inputFake);

						labelWrapper.InnerHtml.AppendHtml(TextTrue);
						wrapper2.InnerHtml.AppendHtml(labelWrapper);

						columnTdEl.InnerHtml.AppendHtml(wrapper2);
						rowTrEl.InnerHtml.AppendHtml(columnTdEl);
					}
					tableBodyEl.InnerHtml.AppendHtml(rowTrEl);
				}

				tableEl.InnerHtml.AppendHtml(tableBodyEl);

				wrapper1.InnerHtml.AppendHtml(tableEl);

				var inputHidden = new TagBuilder("input");
				inputHidden.Attributes.Add("type", "hidden");
				inputHidden.Attributes.Add("id", $"input-{FieldId}");
				inputHidden.Attributes.Add("name", Name);
				inputHidden.Attributes.Add("value", (Value ?? "").ToString().ToLowerInvariant());
				wrapper1.InnerHtml.AppendHtml(inputHidden);

				output.Content.AppendHtml(wrapper1);


				#region << Init Scripts >>
				var tagHelperInitialized = false;
				var fileName = "form.js";
				if (ViewContext.HttpContext.Items.ContainsKey(typeof(WvFieldCheckboxGrid) + fileName))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[typeof(WvFieldCheckboxGrid) + fileName];
					tagHelperInitialized = tagHelperContext.Initialized;
				}
				if (!tagHelperInitialized)
				{
					var scriptContent = WvHelpers.GetEmbeddedTextResource(fileName, "WebVella.TagHelpers.TagHelpers.WvFieldCheckboxGrid", "WebVella.TagHelpers");
					var scriptEl = new TagBuilder("script");
					scriptEl.Attributes.Add("type", "text/javascript");
					scriptEl.InnerHtml.AppendHtml(scriptContent);
					output.PostContent.AppendHtml(scriptEl);

					ViewContext.HttpContext.Items[typeof(WvFieldCheckboxGrid) + fileName] = new WvTagHelperContext()
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
							CheckboxGridFormInit(""{{FieldId}}"");
						});";
				scriptTemplate = scriptTemplate.Replace("{{FieldId}}", (FieldId != null ? FieldId.Value.ToString() : ""));

				initScript.InnerHtml.AppendHtml(scriptTemplate);

				output.PostContent.AppendHtml(initScript);
				#endregion

			}
			else if (Mode == WvFieldRenderMode.Display)
			{
				var wrapper1 = new TagBuilder("div");
				wrapper1.Attributes.Add("id", $"input-{FieldId}");
				wrapper1.AddCssClass("form-control-plaintext erp-checkbox-grid erp-list");

				var tableEl = new TagBuilder("table");
				tableEl.AddCssClass("table table-bordered table-hover table-sm mb-0");

				if (!allColumnsWithEmptyLabels)
				{
					var tableHead = new TagBuilder("thead");
					var tableHeadTr = new TagBuilder("tr");
					var tableHeadFirstTh = new TagBuilder("th");
					tableHeadTr.InnerHtml.AppendHtml(tableHeadFirstTh);

					foreach (var column in Columns)
					{
						var tableHeadTh = new TagBuilder("th");
						tableHeadTh.InnerHtml.Append(column.Label);
						tableHeadTr.InnerHtml.AppendHtml(tableHeadTh);
					}
					tableHead.InnerHtml.AppendHtml(tableHeadTr);
					tableEl.InnerHtml.AppendHtml(tableHead);
				}
				var tableBodyEl = new TagBuilder("tbody");
				foreach (var row in Rows)
				{

					var rowValuesIndex = ValueGrid.FindIndex(x => x.Key == row.Value);
					var rowValues = new List<string>();
					if (rowValuesIndex > -1)
					{
						rowValues = ValueGrid[rowValuesIndex].Values;
					}
					var rowTrEl = new TagBuilder("tr");
					if (!allRowsWithEmptyLabels)
					{
						var rowFirstTdEl = new TagBuilder("td");
						rowFirstTdEl.InnerHtml.Append(row.Label);
						rowTrEl.InnerHtml.AppendHtml(rowFirstTdEl);
					}
					foreach (var column in Columns)
					{
						var isChecked = rowValues.Contains(column.Value);
						var columnTdEl = new TagBuilder("td");

						var iconEl = new TagBuilder("span");
						iconEl.AddCssClass($"{(isChecked ? "fa fa-check" : "go-gray")}");
						iconEl.InnerHtml.Append($"{(isChecked ? "" : "-")}");

						columnTdEl.InnerHtml.AppendHtml(iconEl);
						rowTrEl.InnerHtml.AppendHtml(columnTdEl);
					}
					tableBodyEl.InnerHtml.AppendHtml(rowTrEl);
				}

				tableEl.InnerHtml.AppendHtml(tableBodyEl);

				wrapper1.InnerHtml.AppendHtml(tableEl);

				output.Content.AppendHtml(wrapper1);

			}
			else if (Mode == WvFieldRenderMode.Simple)
			{
				output.SuppressOutput();
				var alertEl = new TagBuilder("div");
				alertEl.AddCssClass($"alert alert-danger");
				alertEl.InnerHtml.Append($"Not Implemented yet");
				output.Content.AppendHtml(alertEl);
			}
			else if (Mode == WvFieldRenderMode.InlineEdit)
			{
				var alertEl = new TagBuilder("div");
				alertEl.AddCssClass($"alert alert-danger");
				alertEl.InnerHtml.Append($"Not Implemented yet");
				output.Content.AppendHtml(alertEl);
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
