using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvButtonType
	{
		[WvSelectOption(Label = "button")]
		Button = 0,
		[WvSelectOption(Label = "submit")]
		Submit = 1,
		[WvSelectOption(Label = "Link As Button")]
		LinkAsButton = 2,
		[WvSelectOption(Label = "Button Link")]
		ButtonLink = 3,
	}
}
