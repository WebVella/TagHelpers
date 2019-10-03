using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvHorizontalAlignmentType
	{
		[WvSelectOption(Label = "None")]
		None = 1,
		[WvSelectOption(Label = "left")]
		Left = 2,
		[WvSelectOption(Label = "center")]
		Center = 3,
		[WvSelectOption(Label = "right")]
		Right = 4
	}
}
