using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public class WvCurrencyType
	{
		[JsonProperty(PropertyName = "symbol")]
		public string Symbol { get; set; }

		[JsonProperty(PropertyName = "symbolNative")]
		public string SymbolNative { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "namePlural")]
		public string NamePlural { get; set; }

		[JsonProperty(PropertyName = "code")]
		public string Code { get; set; }

		[JsonProperty(PropertyName = "decimalDigits")]
		public int DecimalDigits { get; set; }

		[JsonProperty(PropertyName = "rounding")]
		public int Rounding { get; set; }

		[JsonProperty(PropertyName = "symbolPlacement")]
		public CurrencySymbolPlacement SymbolPlacement { get; set; } = CurrencySymbolPlacement.After;
	}

	public enum CurrencySymbolPlacement
	{
		Before = 1,
		After
	}
}
