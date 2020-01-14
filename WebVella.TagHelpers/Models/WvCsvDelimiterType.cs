using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvCsvDelimiterType
	{
        [WvSelectOption(Label = "comma")]
        COMMA = 0,
        [WvSelectOption(Label = "tab")]
        TAB = 1,
	}
}
