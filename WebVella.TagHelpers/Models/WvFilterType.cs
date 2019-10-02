namespace WebVella.TagHelpers.Models
{
	public enum WvFilterType
	{
		[WvSelectOption(Label = "")]
		Undefined = 0,
		[WvSelectOption(Label = "Starts with")]
		STARTSWITH = 1,
		[WvSelectOption(Label = "Contains")]
		CONTAINS = 2,
		[WvSelectOption(Label = "Equals")]
		EQ = 3,
		[WvSelectOption(Label = "Does not equal")]
		NOT = 4,
		[WvSelectOption(Label = "Less than")]
		LT = 5,
		[WvSelectOption(Label = "Less than or equal to")]
		LTE = 6,
		[WvSelectOption(Label = "Greater than")]
		GT = 7,
		[WvSelectOption(Label = "Greater than or equal to")]
		GTE = 8,
		[WvSelectOption(Label = "Matches RegEx")]
		REGEX = 9,
		[WvSelectOption(Label = "Full text search")]
		FTS = 10,
		[WvSelectOption(Label = "Between")]
		BETWEEN = 11,
		[WvSelectOption(Label = "Not Between")]
		NOTBETWEEN = 12,
	}
}
