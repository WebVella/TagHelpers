using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvCssBreakpoint
	{
		[WvSelectOption(Label = "none")]
		None = 0,
		[WvSelectOption(Label = "xs")]
		XSmall = 1,
		[WvSelectOption(Label = "sm")]
		Small = 2,
		[WvSelectOption(Label = "md")]
		Medium = 3,
		[WvSelectOption(Label = "lg")]
		Large = 4,
		[WvSelectOption(Label = "xl")]
		XLarge = 5
	}
}
