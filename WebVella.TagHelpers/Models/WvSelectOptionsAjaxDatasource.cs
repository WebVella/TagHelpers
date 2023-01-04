using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvSelectOptionsAjaxDatasource
	{
		[JsonPropertyName("ds")]
		public string DatasourceName { get; set; } = "";

		[JsonPropertyName("use_select_api")]
		public bool UseSelectApi { get; set; } = false;

		[JsonPropertyName("value")]
		public string Value { get; set; } = "id";

		[JsonPropertyName("label")]
		public string Label { get; set; } = "label";

		[JsonPropertyName("page_size")]
		public int PageSize { get; set; } = 10;

		[JsonPropertyName("init_options")]
		public List<WvSelectOption> InitOptions { get; set; } = new List<WvSelectOption>();
	}
}
