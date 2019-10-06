using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public class WvIcon
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; } = "";	
		
		[JsonProperty(PropertyName = "class")]
		public string Class { get; set; } = "";		

		[JsonProperty(PropertyName = "symbol")]
		public string Symbol { get; set; } = "";
	}
}
