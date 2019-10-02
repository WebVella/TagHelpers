namespace WebVella.TagHelpers.Models
{
	public enum WvFieldRenderMode
	{
		[WvSelectOption(Label = "Inherit")]
		Undefined = 0,
		[WvSelectOption(Label = "Form")]
		Form = 1,
		[WvSelectOption(Label = "Display")]
		Display = 2,
		[WvSelectOption(Label = "InlineEdit")]
		InlineEdit = 3,
		[WvSelectOption(Label = "Simple")]
		Simple = 4
	}
}
