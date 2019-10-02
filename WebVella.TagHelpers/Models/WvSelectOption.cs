using Newtonsoft.Json;
using System;

namespace WebVella.TagHelpers.Models
{
	public class WvSelectOption
	{
		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; } = "";

		[JsonProperty(PropertyName = "label")]
		public string Label { get; set; } = "";

		[JsonProperty(PropertyName = "icon_class")]
		public string IconClass { get; set; } = "";

		[JsonProperty(PropertyName = "color")]
		public string Color { get; set; } = "";

		public WvSelectOption()
		{

		}

		public WvSelectOption(string value, string label)
		{
			Value = value;
			Label = label;
		}

		public WvSelectOption(string value, string label, string iconClass, string color)
		{
			Value = value;
			Label = label;
			IconClass = iconClass;
			Color = color;
		}

		public WvSelectOption(WvSelectOption option) : this(option.Value, option.Label)
		{
		}
	}
}
