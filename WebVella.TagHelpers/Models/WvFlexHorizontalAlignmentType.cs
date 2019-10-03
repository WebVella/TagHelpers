using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvFlexHorizontalAlignmentType
	{
		[WvSelectOption(Label = "Default")]
		None = 1,
		[WvSelectOption(Label = "Start")]
		Start = 2,
		[WvSelectOption(Label = "Center")]
		Center = 3,
		[WvSelectOption(Label = "End")]
		End = 4,
		[WvSelectOption(Label = "Around")]
		Around = 5,
		[WvSelectOption(Label = "Between")]
		Between = 6
	}
}
