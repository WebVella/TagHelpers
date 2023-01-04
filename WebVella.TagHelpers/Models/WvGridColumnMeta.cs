using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvGridColumnMeta
	{
		[JsonPropertyName("container_id")]
		public string ContainerId { get; set; } = "";

		[JsonPropertyName("name")]
		public string Name { get; set; } = "";

		[JsonPropertyName("label")]
		public string Label { get; set; } = "";

		[JsonPropertyName("width")]
		public string Width { get; set; } = "";

		[JsonPropertyName("searchable")]
		public bool Searchable { get; set; } = false;

		[JsonPropertyName("sortable")]
		public bool Sortable { get; set; } = false;

		[JsonPropertyName("class")]
		public string Class { get; set; } = "";
	}
}
