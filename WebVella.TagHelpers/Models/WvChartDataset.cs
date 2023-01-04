using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvChartDataset
	{
		/// <summary>
		/// The label for the dataset which appears in the legend and tooltips.
		/// </summary>
		[JsonPropertyName("label")]
		public string Label { get; set; }

		[JsonPropertyName("data")]
		public List<decimal> Data { get; set; }

		[JsonPropertyName("borderColor")]
		public object BorderColor { get; set; } = null; // List<string> or string

		[JsonPropertyName("backgroundColor")]
		public object BackgroundColor { get; set; } = null; // List<string> or string

		[JsonPropertyName("fill")]
		public bool? Fill { get; set; } = null;

		[JsonPropertyName("borderWidth")]
		public int BorderWidth { get; set; } = 2;
	}
}
