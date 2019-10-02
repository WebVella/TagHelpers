using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvCssSize
	{
		[WvSelectOption(Label = "normal")]
		Normal = 0,
		[WvSelectOption(Label = "small")]
		Small = 1,
		[WvSelectOption(Label = "large")]
		Large = 2,
		[WvSelectOption(Label = "inherit")]
		Inherit = 3,
	}
}
