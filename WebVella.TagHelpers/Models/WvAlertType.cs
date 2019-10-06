using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvAlertType
	{
		[WvSelectOption(Label = "danger")]
		Danger = 0,
		[WvSelectOption(Label = "warning")]
		Warning = 1,
		[WvSelectOption(Label = "success")]
		Success = 2,
		[WvSelectOption(Label = "info")]
		Info = 3,
		[WvSelectOption(Label = "primary")]
		Primary = 4,
		[WvSelectOption(Label = "secondary")]
		Secondary = 5,
		[WvSelectOption(Label = "light")]
		Light = 6,
		[WvSelectOption(Label = "dark")]
		Dark = 7
	}
}
