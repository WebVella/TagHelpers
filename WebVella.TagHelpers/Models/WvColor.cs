using System;
using System.Collections.Generic;
using System.Text;

namespace WebVella.TagHelpers.Models
{
	public enum WvColor
	{
		[WvSelectOption(Label = "white")]
		White = 0,
		[WvSelectOption(Label = "primary")]
		Primary = 1,
		[WvSelectOption(Label = "secondary")]
		Secondary = 2,
		[WvSelectOption(Label = "success")]
		Success = 3,
		[WvSelectOption(Label = "info")]
		Info = 4,
		[WvSelectOption(Label = "warning")]
		Warning = 5,
		[WvSelectOption(Label = "danger")]
		Danger = 6,
		[WvSelectOption(Label = "light")]
		Light = 7,
		[WvSelectOption(Label = "dark")]
		Dark = 8,
		[WvSelectOption(Label = "link")]
		Link = 9,
		[WvSelectOption(Label = "red")]
		Red = 10,
		[WvSelectOption(Label = "pink")]
		Pink = 11,
		[WvSelectOption(Label = "purple")]
		Purple = 12,
		[WvSelectOption(Label = "deep-purple")]
		DeepPurple = 13,
		[WvSelectOption(Label = "indigo")]
		Indigo = 14,
		[WvSelectOption(Label = "blue")]
		Blue = 15,
		[WvSelectOption(Label = "light-blue")]
		LightBlue = 16,
		[WvSelectOption(Label = "cyan")]
		Cyan = 17,
		[WvSelectOption(Label = "teal")]
		Teal = 18,
		[WvSelectOption(Label = "green")]
		Green = 19,
		[WvSelectOption(Label = "light-green")]
		LightGreen = 20,
		[WvSelectOption(Label = "lime")]
		Lime = 21,
		[WvSelectOption(Label = "yellow")]
		Yellow = 22,
		[WvSelectOption(Label = "amber")]
		Amber = 23,
		[WvSelectOption(Label = "orange")]
		Orange = 24,
		[WvSelectOption(Label = "deep-orange")]
		DeepOrange = 25,
		[WvSelectOption(Label = "brown")]
		Brown = 26,
		[WvSelectOption(Label = "black")]
		Black = 27,
		[WvSelectOption(Label = "gray")]
		Gray = 28,
		[WvSelectOption(Label = "blue-gray")]
		BlueGray = 29,
		[WvSelectOption(Label = "default")]
		Default = 30,
		[WvSelectOption(Label = "none")]
		None = 31
	}
}
