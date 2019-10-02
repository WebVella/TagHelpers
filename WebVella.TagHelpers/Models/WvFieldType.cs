namespace WebVella.TagHelpers.Models
{
	public enum WvFieldType
	{
		[WvSelectOption(Label = "autonumber")]
		AutoNumberField = 1,
		[WvSelectOption(Label = "checkbox")]
		CheckboxField = 2,
		[WvSelectOption(Label = "currency")]
		CurrencyField = 3,
		[WvSelectOption(Label = "date")]
		DateField = 4,
		[WvSelectOption(Label = "datetime")]
		DateTimeField = 5,
		[WvSelectOption(Label = "email")]
		EmailField = 6,
		[WvSelectOption(Label = "file")]
		FileField = 7,
		[WvSelectOption(Label = "html")]
		HtmlField = 8,
		[WvSelectOption(Label = "image")]
		ImageField = 9,
		[WvSelectOption(Label = "multilinetext")]
		MultiLineTextField = 10,
		[WvSelectOption(Label = "multiselect")]
		MultiSelectField = 11,
		[WvSelectOption(Label = "number")]
		NumberField = 12,
		[WvSelectOption(Label = "password")]
		PasswordField = 13,
		[WvSelectOption(Label = "percent")]
		PercentField = 14,
		[WvSelectOption(Label = "phone")]
		PhoneField = 15,
		[WvSelectOption(Label = "guid")]
		GuidField = 16,
		[WvSelectOption(Label = "select")]
		SelectField = 17,
		[WvSelectOption(Label = "text")]
		TextField = 18,
		[WvSelectOption(Label = "url")]
		UrlField = 19,
		[WvSelectOption(Label = "relation")]
		RelationField = 20
	}
}
