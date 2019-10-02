using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvChartType
	{
		[WvSelectOption(Label = "line")]
		Line = 0,
		[WvSelectOption(Label = "bar")]
		Bar = 1,
		[WvSelectOption(Label = "pie")]
		Pie = 2,
		[WvSelectOption(Label = "doughnut")]
		Doughnut = 3,
		[WvSelectOption(Label = "line")] //This is correct, another dataset property will be changed to apply the area
		Area = 4,
		[WvSelectOption(Label = "horizontalBar")]
		HorizontalBar = 5
	}
}
