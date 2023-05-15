using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using WebVella.TagHelpers.Models;

namespace WebVella.TagHelpers.Utilities
{
	public static class EnumExtensions
	{
		public static string ToDescriptionString<T>(this T e) where T : IConvertible
		{
			string description = "";

			if (e is Enum)
			{
				Type type = e.GetType();
				var memInfo = type.GetMember(type.GetEnumName(e.ToInt32(CultureInfo.InvariantCulture)));
				var soAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (soAttributes.Length > 0)
				{
					// we're only getting the first description we find
					// others will be ignored
					description = ((DescriptionAttribute)soAttributes[0]).Description;
				}
			}

			return description;
		}

        public static List<TEnum> GetList<TEnum>() where TEnum : Enum
        {
            List<TEnum> selectEnumOptions = new();

            if (!typeof(TEnum).IsEnum)
            {
                return selectEnumOptions;
            }

            foreach (FieldInfo field in typeof(TEnum).GetFields())
            {
                if (field.FieldType.IsEnum)
                {
                    selectEnumOptions.Add((TEnum)field.GetValue(0));
                }
            }

            return selectEnumOptions;
        }

        public static List<WvSelectOption> GetAsSelectOptions<TEnum>() where TEnum : Enum
        {
            List<WvSelectOption> selectEnumOptions = new();

            if (!typeof(TEnum).IsEnum)
            {
                return selectEnumOptions;
            }

            foreach (var value in Enum.GetValues(typeof(TEnum)))
            {
                selectEnumOptions.Add(new WvSelectOption
                {
                    Value = ((int)value).ToString(),
                    Label = ((TEnum)value).ToDescriptionString()
                });
            }

            return selectEnumOptions;
        }
    }
}
