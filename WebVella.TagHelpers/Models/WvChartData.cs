using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvChartData
	{
		[JsonPropertyName("labels")]
		public List<string> Labels { get; set; }

		[JsonPropertyName("datasets")]
		public List<WvChartDataset> Datasets { get; set; }
	}
}
