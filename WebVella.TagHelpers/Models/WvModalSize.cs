using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvModalSize
	{
		[WvSelectOption(Label = "normal")]
		Normal = 0,
		[WvSelectOption(Label = "modal-sm")]
		Small = 1,
		[WvSelectOption(Label = "modal-lg")]
		Large = 2,
		[WvSelectOption(Label = "modal-xl")]
		ExtraLarge = 3,
		[WvSelectOption(Label = "modal-full")]
		Full = 4,
	}
}
