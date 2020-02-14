using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebVella.TagHelpers.Models
{
	public class WvSelectOptionsAjaxDatasource
	{
		[JsonProperty(PropertyName = "ds")]
		public string DatasourceName { get; set; } = "";

		[JsonProperty(PropertyName = "use_select_api")]
		public bool UseSelectApi { get; set; } = false;

		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; } = "id";

		[JsonProperty(PropertyName = "label")]
		public string Label { get; set; } = "label";

		[JsonProperty(PropertyName = "page_size")]
		public int PageSize { get; set; } = 10;

		[JsonProperty(PropertyName = "init_options")]
		public List<WvSelectOption> InitOptions { get; set; } = new List<WvSelectOption>();
	}
}
