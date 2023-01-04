using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvCurrency
	{

		[JsonPropertyName("id")]
		public string Id { get; set; } = "";

		[JsonPropertyName("alternate_symbols")]
		public List<string> AlternateSymbols { get; set; } = new List<string>();

		[JsonPropertyName("decimal_mark")]
		public string DecimalMark { get; set; } = "";

		[JsonPropertyName("disambiguate_symbol")]
		public string DisambiguateSymbol { get; set; } = "";

		[JsonPropertyName("html_entity")]
		public string html_entity { get; set; } = "";

		[JsonPropertyName("iso_code")]
		public string IsoCode { get; set; } = "";

		[JsonPropertyName("iso_numeric")]
		public string IsoNumeric { get; set; } = "";

		[JsonPropertyName("name")]
		public string Name { get; set; } = "";

		[JsonPropertyName("priority")]
		public int Priority { get; set; } = 100;

		[JsonPropertyName("smallest_denomination")]
		public int SmallestDenomination { get; set; } = 1;

		[JsonPropertyName("subunit")]
		public string SubUnit { get; set; } = "";

		[JsonPropertyName("subunit_to_unit")]
		public int SubUnitToUnit { get; set; } = 100;

		[JsonPropertyName("symbol")]
		public string Symbol { get; set; } = "";

		[JsonPropertyName("symbol_first")]
		public bool SymbolFirst { get; set; } = false;

		[JsonPropertyName("thousands_separator")]
		public string ThousandsSeparator { get; set; } = "";
	}
}
