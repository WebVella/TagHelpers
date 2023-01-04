using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvCurrencyType
	{
		[JsonPropertyName("symbol")]
		public string Symbol { get; set; }

		[JsonPropertyName("symbolNative")]
		public string SymbolNative { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("namePlural")]
		public string NamePlural { get; set; }

		[JsonPropertyName("code")]
		public string Code { get; set; }

		[JsonPropertyName("decimalDigits")]
		public int DecimalDigits { get; set; }

		[JsonPropertyName("rounding")]
		public int Rounding { get; set; }

		[JsonPropertyName("symbolPlacement")]
		public CurrencySymbolPlacement SymbolPlacement { get; set; } = CurrencySymbolPlacement.After;
	}

	public enum CurrencySymbolPlacement
	{
		Before = 1,
		After
	}
}
