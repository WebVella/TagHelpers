using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvIcon
	{
		[JsonPropertyName("name")]
		public string Name { get; set; } = "";

		[JsonPropertyName("class")]
		public string Class { get; set; } = "";

		[JsonPropertyName("symbol")]
		public string Symbol { get; set; } = "";
	}
}
