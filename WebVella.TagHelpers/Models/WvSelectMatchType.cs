namespace WebVella.TagHelpers.Models
{
	public enum WvSelectMatchType
	{
		[WvSelectOption(Label = "contains")]
		Contains = 0,
		[WvSelectOption(Label = "starts with")]
		StartsWith = 1
	}
}
