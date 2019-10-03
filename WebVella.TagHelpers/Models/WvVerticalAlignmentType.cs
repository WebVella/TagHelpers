using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvVerticalAlignmentType
	{
		[WvSelectOption(Label = "None")]
		None = 1,
		[WvSelectOption(Label = "Top")]
		Top = 2,
		[WvSelectOption(Label = "Middle")]
		Middle = 3,
		[WvSelectOption(Label = "Bottom")]
		Bottom = 4
	}
}
