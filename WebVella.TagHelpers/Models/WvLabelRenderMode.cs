namespace WebVella.TagHelpers.Models
{
	public enum WvLabelRenderMode
	{
		[WvSelectOption(Label = "Inherit")]
		Undefined = 0,
		[WvSelectOption(Label = "Stacked")]
		Stacked = 1,
		[WvSelectOption(Label = "Horizontal")]
		Horizontal = 2,
		[WvSelectOption(Label = "Hidden")]
		Hidden = 3
	}
}
