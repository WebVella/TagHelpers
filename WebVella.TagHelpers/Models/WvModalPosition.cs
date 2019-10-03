using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvModalPosition
	{
		[WvSelectOption(Label = "")]
		Top = 0,
		[WvSelectOption(Label = "modal-dialog-centered")]
		VerticallyCentered = 1,
	}
}
