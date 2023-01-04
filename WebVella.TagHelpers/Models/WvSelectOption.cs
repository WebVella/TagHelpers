using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvSelectOption
	{
		[JsonPropertyName("value")]
		public string Value { get; set; } = "";

		[JsonPropertyName("label")]
		public string Label { get; set; } = "";

		[JsonPropertyName("icon_class")]
		public string IconClass { get; set; } = "";

		[JsonPropertyName("color")]
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
