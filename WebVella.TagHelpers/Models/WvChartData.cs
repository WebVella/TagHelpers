using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public class WvChartData
	{
		[JsonProperty(PropertyName = "labels")]
		public List<string> Labels { get; set; }

		[JsonProperty(PropertyName = "datasets")]
		public List<WvChartDataset> Datasets { get; set; }
	}
}
