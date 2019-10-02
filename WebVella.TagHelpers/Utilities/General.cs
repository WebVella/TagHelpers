using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WebVella.TagHelpers.Models;


namespace WebVella.TagHelpers.Utils
{
	internal static class Utility
	{
		internal static void ValidateValueToFieldType(WvFieldType? fieldType, dynamic InValue, out dynamic OutValue, out List<string> errorList)
		{
			OutValue = null;
			errorList = new List<string>();
			if (InValue != null && InValue is Enum)
			{
				InValue = ((int)InValue).ToString();
			}

			switch (fieldType)
			{
				case WvFieldType.AutoNumberField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = null;
						}
						else if (InValue is decimal)
						{
							OutValue = (decimal)InValue;
						}
						else if (Decimal.TryParse(InValue.ToString(), out decimal result))
						{
							OutValue = result;
						}
						else
						{
							errorList.Add("Value should be a decimal");
						}
					}
					break;
				case WvFieldType.CheckboxField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = null;
						}
						else if (InValue is bool)
						{
							OutValue = (bool)InValue;
						}
						else if (Boolean.TryParse(InValue.ToString(), out bool result))
						{
							OutValue = result;
						}
						else
						{
							errorList.Add("Value should be a boolean");
						}
					}
					break;
				case WvFieldType.CurrencyField:
				case WvFieldType.NumberField:
				case WvFieldType.PercentField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = null;
						}
						else if (InValue is decimal)
						{
							OutValue = (decimal)InValue;
						}
						else if (Decimal.TryParse(InValue.ToString(), out decimal result))
						{
							OutValue = result;
						}
						else
						{
							errorList.Add("Value should be a decimal");
						}
					}
					break;
				case WvFieldType.DateField:
				case WvFieldType.DateTimeField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = null;
						}
						else if (InValue is DateTime)
						{
							OutValue = (DateTime)InValue;
						}
						else if (DateTime.TryParse(InValue.ToString(), out DateTime result))
						{
							OutValue = result;
						}
						else
						{
							errorList.Add("Value should be a DateTime");
						}
					}
					break;
				case WvFieldType.EmailField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = "";
						}
						else
						{
							var emailRgx = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
							if (!String.IsNullOrWhiteSpace(InValue) && !emailRgx.IsMatch(InValue.ToString()))
							{
								errorList.Add("Value is not a valid email!");
							}
							OutValue = InValue.ToString();
						}
					}
					break;
				case WvFieldType.GuidField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = null;
						}
						else if (InValue is Guid)
						{
							OutValue = (Guid)InValue;
						}
						else if (Guid.TryParse(InValue.ToString(), out Guid result))
						{
							OutValue = result;
						}
						else
						{
							errorList.Add("Value should be a Guid");
						}
					}
					break;
				case WvFieldType.HtmlField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = "";
						}
						else
						{
							InValue = InValue.ToString();
							//Check if Html value is valid
							HtmlDocument doc = new HtmlDocument();
							doc.LoadHtml(InValue);
							doc.OptionFixNestedTags = true;
							doc.OptionAutoCloseOnEnd = true;
							if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
							{
								foreach (var error in doc.ParseErrors)
								{
									errorList.Add($"Invalid html on line {error.Line}. {error.Reason}");
								}
							}
							else
							{
								OutValue = doc.DocumentNode.OuterHtml;
							}
						}
					}
					break;
				case WvFieldType.MultiSelectField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = new List<string>();
						}
						else if (InValue is List<string>)
						{
							OutValue = (List<string>)InValue;
						}
						else
						{
							var newList = new List<string>();
							newList.Add(InValue.ToString());
							OutValue = newList;
						}
					}
					break;
				case WvFieldType.FileField:
				case WvFieldType.ImageField:
				case WvFieldType.MultiLineTextField:
				case WvFieldType.PasswordField:
				case WvFieldType.PhoneField:
				case WvFieldType.SelectField:
				case WvFieldType.TextField:
				case WvFieldType.UrlField:
					{
						if (InValue == null || InValue.ToString() == "")
						{
							OutValue = "";
						}
						else
						{
							OutValue = InValue.ToString();
						}
					}
					break;
				default:
					{
						OutValue = InValue;
					}
					break;
			}
		}
		internal static List<WvFilterType> GetFilterTypesForFieldType(WvFieldType fieldType)
		{
			var result = new List<WvFilterType>();

			switch (fieldType)
			{
				case WvFieldType.CheckboxField:
					{
						result.Add(WvFilterType.EQ);
					}
					break;
				case WvFieldType.AutoNumberField:
				case WvFieldType.CurrencyField:
				case WvFieldType.NumberField:
				case WvFieldType.PercentField:
					{
						result.Add(WvFilterType.EQ);
						result.Add(WvFilterType.NOT);
						result.Add(WvFilterType.LT);
						result.Add(WvFilterType.LTE);
						result.Add(WvFilterType.GT);
						result.Add(WvFilterType.GTE);
						result.Add(WvFilterType.BETWEEN);
						result.Add(WvFilterType.NOTBETWEEN);
					}
					break;
				case WvFieldType.DateField:
				case WvFieldType.DateTimeField:
					{
						result.Add(WvFilterType.EQ);
						result.Add(WvFilterType.NOT);
						result.Add(WvFilterType.LT);
						result.Add(WvFilterType.LTE);
						result.Add(WvFilterType.GT);
						result.Add(WvFilterType.GTE);
						result.Add(WvFilterType.BETWEEN);
						result.Add(WvFilterType.NOTBETWEEN);
					}
					break;
				case WvFieldType.GuidField:
					{
						result.Add(WvFilterType.EQ);
					}
					break;
				case WvFieldType.MultiSelectField:
					{
						result.Add(WvFilterType.CONTAINS);
					}
					break;
				default:
					{
						result.Add(WvFilterType.STARTSWITH);
						result.Add(WvFilterType.CONTAINS);
						result.Add(WvFilterType.EQ);
						result.Add(WvFilterType.NOT);
						result.Add(WvFilterType.REGEX);
						result.Add(WvFilterType.FTS);
					}
					break;
			}
			return result;
		}


		internal static string GetEmbeddedTextResource(string name, string nameSpace, string assemblyName = null)
		{
			string resourceName = $"{nameSpace}.{name}";
			Assembly assembly = null;
			if (!String.IsNullOrWhiteSpace(assemblyName))
			{
				assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == assemblyName);
			}
			else
			{
				assembly = Assembly.GetExecutingAssembly();
			}
			Stream resource = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resource);
			return reader.ReadToEnd();
		}

		internal static string GetEmbeddedTextResource(string name, string nameSpace, Assembly assembly)
		{
			string resourceName = $"{nameSpace}.{name}";
			Stream resource = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resource);
			return reader.ReadToEnd();
		}

		internal static bool EmbeddedResourceExists(string name, string nameSpace, string assemblyName = null)
		{
			string resourceName = $"{nameSpace}.{name}";
			Assembly assembly = null;
			if (!String.IsNullOrWhiteSpace(assemblyName))
			{
				assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == assemblyName);
			}
			else
			{
				assembly = Assembly.GetExecutingAssembly();
			}
			var resources = assembly.GetManifestResourceNames();
			return resources.Contains(resourceName);
		}

		internal static bool EmbeddedResourceExists(string name, string nameSpace, Assembly assembly)
		{
			string resourceName = $"{nameSpace}.{name}";
			var resources = assembly.GetManifestResourceNames();
			return resources.Contains(resourceName);
		}

		internal static Assembly GetTypeAssembly(string typeName)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
						.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							|| a.FullName.ToLowerInvariant().StartsWith("system.")));
			foreach (var assembly in assemblies)
			{
				foreach (Type t in assembly.GetTypes())
				{
					string name = $"{t.Namespace}.{t.Name}";
					if (name == typeName)
						return assembly;
				}
			}
			return null;
		}

		internal static Type GetType(string typeName)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
						.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							|| a.FullName.ToLowerInvariant().StartsWith("system.")));
			foreach (var assembly in assemblies)
			{
				foreach (Type t in assembly.GetTypes())
				{
					string name = $"{t.Namespace}.{t.Name}";
					if (name == typeName)
						return t;
				}
			}
			return null;
		}

	}
}
