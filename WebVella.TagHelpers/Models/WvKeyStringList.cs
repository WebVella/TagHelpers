using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvKeyStringList
	{
		[JsonPropertyName("key")]
		public string Key { get; set; } = "";

		[JsonPropertyName("values")]
		public List<string> Values { get; set; } = new List<string>();
	}
}
