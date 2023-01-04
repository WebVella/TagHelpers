using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WebVella.TagHelpers.Models;


namespace WebVella.TagHelpers.Utilities
{
	public static class WvHelpers
	{
		#region << Embedded resources >>
		public static string GetEmbeddedTextResource(string name, string nameSpace, string assemblyName = null)
		{
			string resourceName = $"{nameSpace}.{name}";
			Assembly assembly = null;
			if (!String.IsNullOrWhiteSpace(assemblyName))
			{
				assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName);
			}
			else
			{
				assembly = Assembly.GetExecutingAssembly();
			}
			Stream resource = assembly.GetManifestResourceStream(resourceName);
			if (resource == null)
				throw new Exception($"file: {name} in resource: {resourceName} not found as embedded resource");

			StreamReader reader = new StreamReader(resource);
			return reader.ReadToEnd();
		}

		public static string GetEmbeddedTextResource(string name, string nameSpace, Assembly assembly)
		{
			string resourceName = $"{nameSpace}.{name}";
			Stream resource = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resource);
			return reader.ReadToEnd();
		}
		public static bool EmbeddedResourceExists(string name, string nameSpace, string assemblyName = null)
		{
			string resourceName = $"{nameSpace}.{name}";
			Assembly assembly = null;
			if (!String.IsNullOrWhiteSpace(assemblyName))
			{
				assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName);
			}
			else
			{
				assembly = Assembly.GetExecutingAssembly();
			}
			var resources = assembly.GetManifestResourceNames();
			return resources.Contains(resourceName);
		}
		public static bool EmbeddedResourceExists(string name, string nameSpace, Assembly assembly)
		{
			string resourceName = $"{nameSpace}.{name}";
			var resources = assembly.GetManifestResourceNames();
			return resources.Contains(resourceName);
		}
		public static Assembly GetTypeAssembly(string typeName)
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
		public static Type GetType(string typeName)
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

		#endregion

		#region << Fields >>
		public static void ValidateValueToFieldType(WvFieldType? fieldType, dynamic InValue, out dynamic OutValue, out List<string> errorList)
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
							InValue = InValue.Replace("<script>", "&lt;script&gt;").Replace("</script>", "&lt;/script&gt;");
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
		public static List<WvFilterType> GetFilterTypesForFieldType(WvFieldType fieldType)
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

		public static List<WvCurrency> GetAllCurrency()
		{
			var result = new List<WvCurrency>();
			var currencyJson = "";
			#region << Currency List as String >>
			currencyJson = @"{
  ""aed"": {
  	""priority"": 100,
    ""iso_code"": ""AED"",
    ""name"": ""United Arab Emirates Dirham"",
    ""symbol"": ""د.إ"",
    ""alternate_symbols"": [""DH"", ""Dhs""],
    ""subunit"": ""Fils"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""784"",
    ""smallest_denomination"": 25
  },
  ""afn"": {
    ""priority"": 100,
    ""iso_code"": ""AFN"",
    ""name"": ""Afghan Afghani"",
    ""symbol"": ""؋"",
    ""alternate_symbols"": [""Af"", ""Afs""],
    ""subunit"": ""Pul"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""971"",
    ""smallest_denomination"": 100
  },
  ""all"": {
    ""priority"": 100,
    ""iso_code"": ""ALL"",
    ""name"": ""Albanian Lek"",
    ""symbol"": ""L"",
    ""disambiguate_symbol"": ""Lek"",
    ""alternate_symbols"": [""Lek""],
    ""subunit"": ""Qintar"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""008"",
    ""smallest_denomination"": 100
  },
  ""amd"": {
    ""priority"": 100,
    ""iso_code"": ""AMD"",
    ""name"": ""Armenian Dram"",
    ""symbol"": ""դր."",
    ""alternate_symbols"": [""dram""],
    ""subunit"": ""Luma"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""051"",
    ""smallest_denomination"": 10
  },
  ""ang"": {
    ""priority"": 100,
    ""iso_code"": ""ANG"",
    ""name"": ""Netherlands Antillean Gulden"",
    ""symbol"": ""ƒ"",
    ""alternate_symbols"": [""NAƒ"", ""NAf"", ""f""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x0192;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""532"",
    ""smallest_denomination"": 1
  },
  ""aoa"": {
    ""priority"": 100,
    ""iso_code"": ""AOA"",
    ""name"": ""Angolan Kwanza"",
    ""symbol"": ""Kz"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cêntimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""973"",
    ""smallest_denomination"": 10
  },
  ""ars"": {
    ""priority"": 100,
    ""iso_code"": ""ARS"",
    ""name"": ""Argentine Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""$m/n"",
    ""alternate_symbols"": [""$m/n"", ""m$n""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""032"",
    ""smallest_denomination"": 1
  },
  ""aud"": {
    ""priority"": 4,
    ""iso_code"": ""AUD"",
    ""name"": ""Australian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""A$"",
    ""alternate_symbols"": [""A$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""036"",
    ""smallest_denomination"": 5
  },
  ""awg"": {
    ""priority"": 100,
    ""iso_code"": ""AWG"",
    ""name"": ""Aruban Florin"",
    ""symbol"": ""ƒ"",
    ""alternate_symbols"": [""Afl""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x0192;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""533"",
    ""smallest_denomination"": 5
  },
  ""azn"": {
    ""priority"": 100,
    ""iso_code"": ""AZN"",
    ""name"": ""Azerbaijani Manat"",
    ""symbol"": ""₼"",
    ""alternate_symbols"": [""m"", ""man""],
    ""subunit"": ""Qəpik"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""944"",
    ""smallest_denomination"": 1
  },
  ""bam"": {
    ""priority"": 100,
    ""iso_code"": ""BAM"",
    ""name"": ""Bosnia and Herzegovina Convertible Mark"",
    ""symbol"": ""КМ"",
    ""alternate_symbols"": [""KM""],
    ""subunit"": ""Fening"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""977"",
    ""smallest_denomination"": 5
  },
  ""bbd"": {
    ""priority"": 100,
    ""iso_code"": ""BBD"",
    ""name"": ""Barbadian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""Bds$"",
    ""alternate_symbols"": [""Bds$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""052"",
    ""smallest_denomination"": 1
  },
  ""bdt"": {
    ""priority"": 100,
    ""iso_code"": ""BDT"",
    ""name"": ""Bangladeshi Taka"",
    ""symbol"": ""৳"",
    ""alternate_symbols"": [""Tk""],
    ""subunit"": ""Paisa"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""050"",
    ""smallest_denomination"": 1
  },
  ""bgn"": {
    ""priority"": 100,
    ""iso_code"": ""BGN"",
    ""name"": ""Bulgarian Lev"",
    ""symbol"": ""лв."",
    ""alternate_symbols"": [""lev"", ""leva"", ""лев"", ""лева""],
    ""subunit"": ""Stotinka"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""975"",
    ""smallest_denomination"": 1
  },
  ""bhd"": {
    ""priority"": 100,
    ""iso_code"": ""BHD"",
    ""name"": ""Bahraini Dinar"",
    ""symbol"": ""ب.د"",
    ""alternate_symbols"": [""BD""],
    ""subunit"": ""Fils"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""048"",
    ""smallest_denomination"": 5
  },
  ""bif"": {
    ""priority"": 100,
    ""iso_code"": ""BIF"",
    ""name"": ""Burundian Franc"",
    ""symbol"": ""Fr"",
    ""disambiguate_symbol"": ""FBu"",
    ""alternate_symbols"": [""FBu""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""108"",
    ""smallest_denomination"": 100
  },
  ""bmd"": {
    ""priority"": 100,
    ""iso_code"": ""BMD"",
    ""name"": ""Bermudian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""BD$"",
    ""alternate_symbols"": [""BD$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""060"",
    ""smallest_denomination"": 1
  },
  ""bnd"": {
    ""priority"": 100,
    ""iso_code"": ""BND"",
    ""name"": ""Brunei Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""BND"",
    ""alternate_symbols"": [""B$""],
    ""subunit"": ""Sen"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""096"",
    ""smallest_denomination"": 1
  },
  ""bob"": {
    ""priority"": 100,
    ""iso_code"": ""BOB"",
    ""name"": ""Bolivian Boliviano"",
    ""symbol"": ""Bs."",
    ""alternate_symbols"": [""Bs""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""068"",
    ""smallest_denomination"": 10
  },
  ""brl"": {
    ""priority"": 100,
    ""iso_code"": ""BRL"",
    ""name"": ""Brazilian Real"",
    ""symbol"": ""R$"",
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""R$"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""986"",
    ""smallest_denomination"": 5
  },
  ""bsd"": {
    ""priority"": 100,
    ""iso_code"": ""BSD"",
    ""name"": ""Bahamian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""BSD"",
    ""alternate_symbols"": [""B$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""044"",
    ""smallest_denomination"": 1
  },
  ""btn"": {
    ""priority"": 100,
    ""iso_code"": ""BTN"",
    ""name"": ""Bhutanese Ngultrum"",
    ""symbol"": ""Nu."",
    ""alternate_symbols"": [""Nu""],
    ""subunit"": ""Chertrum"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""064"",
    ""smallest_denomination"": 5
  },
  ""bwp"": {
    ""priority"": 100,
    ""iso_code"": ""BWP"",
    ""name"": ""Botswana Pula"",
    ""symbol"": ""P"",
    ""alternate_symbols"": [],
    ""subunit"": ""Thebe"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""072"",
    ""smallest_denomination"": 5
  },
  ""byn"": {
    ""priority"": 100,
    ""iso_code"": ""BYN"",
    ""name"": ""Belarusian Ruble"",
    ""symbol"": ""Br"",
    ""disambiguate_symbol"": ""BYN"",
    ""alternate_symbols"": [""бел. руб."", ""б.р."", ""руб."", ""р.""],
    ""subunit"": ""Kapeyka"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": "" "",
    ""iso_numeric"": ""933"",
    ""smallest_denomination"": 1
  },
  ""byr"": {
    ""priority"": 50,
    ""iso_code"": ""BYR"",
    ""name"": ""Belarusian Ruble"",
    ""symbol"": ""Br"",
    ""disambiguate_symbol"": ""BYR"",
    ""alternate_symbols"": [""бел. руб."", ""б.р."", ""руб."", ""р.""],
    ""subunit"": null,
    ""subunit_to_unit"": 1,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": "" "",
    ""iso_numeric"": ""974"",
    ""smallest_denomination"": 100
  },
  ""bzd"": {
    ""priority"": 100,
    ""iso_code"": ""BZD"",
    ""name"": ""Belize Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""BZ$"",
    ""alternate_symbols"": [""BZ$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""084"",
    ""smallest_denomination"": 1
  },
  ""cad"": {
    ""priority"": 5,
    ""iso_code"": ""CAD"",
    ""name"": ""Canadian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""C$"",
    ""alternate_symbols"": [""C$"", ""CAD$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""124"",
    ""smallest_denomination"": 5
  },
  ""cdf"": {
    ""priority"": 100,
    ""iso_code"": ""CDF"",
    ""name"": ""Congolese Franc"",
    ""symbol"": ""Fr"",
    ""disambiguate_symbol"": ""FC"",
    ""alternate_symbols"": [""FC""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""976"",
    ""smallest_denomination"": 1
  },
  ""chf"": {
    ""priority"": 100,
    ""iso_code"": ""CHF"",
    ""name"": ""Swiss Franc"",
    ""symbol"": ""CHF"",
    ""alternate_symbols"": [""SFr"", ""Fr""],
    ""subunit"": ""Rappen"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""756"",
    ""smallest_denomination"": 5
  },
  ""clf"": {
    ""priority"": 100,
    ""iso_code"": ""CLF"",
    ""name"": ""Unidad de Fomento"",
    ""symbol"": ""UF"",
    ""alternate_symbols"": [],
    ""subunit"": ""Peso"",
    ""subunit_to_unit"": 10000,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""990""
  },
  ""clp"": {
    ""priority"": 100,
    ""iso_code"": ""CLP"",
    ""name"": ""Chilean Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""CLP"",
    ""alternate_symbols"": [],
    ""subunit"": ""Peso"",
    ""subunit_to_unit"": 1,
    ""symbol_first"": true,
    ""html_entity"": ""&#36;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""152"",
    ""smallest_denomination"": 1
  },
  ""cny"": {
    ""priority"": 100,
    ""iso_code"": ""CNY"",
    ""name"": ""Chinese Renminbi Yuan"",
    ""symbol"": ""¥"",
    ""alternate_symbols"": [""CN¥"", ""元"", ""CN元""],
    ""subunit"": ""Fen"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""￥"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""156"",
    ""smallest_denomination"": 1
  },
  ""cop"": {
    ""priority"": 100,
    ""iso_code"": ""COP"",
    ""name"": ""Colombian Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""COL$"",
    ""alternate_symbols"": [""COL$""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""170"",
    ""smallest_denomination"": 20
  },
  ""crc"": {
    ""priority"": 100,
    ""iso_code"": ""CRC"",
    ""name"": ""Costa Rican Colón"",
    ""symbol"": ""₡"",
    ""alternate_symbols"": [""¢""],
    ""subunit"": ""Céntimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A1;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""188"",
    ""smallest_denomination"": 500
  },
  ""cuc"": {
    ""priority"": 100,
    ""iso_code"": ""CUC"",
    ""name"": ""Cuban Convertible Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""CUC$"",
    ""alternate_symbols"": [""CUC$""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""931"",
    ""smallest_denomination"": 1
  },
  ""cup"": {
    ""priority"": 100,
    ""iso_code"": ""CUP"",
    ""name"": ""Cuban Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""$MN"",
    ""alternate_symbols"": [""$MN""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""192"",
    ""smallest_denomination"": 1
  },
  ""cve"": {
    ""priority"": 100,
    ""iso_code"": ""CVE"",
    ""name"": ""Cape Verdean Escudo"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""Esc"",
    ""alternate_symbols"": [""Esc""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""132"",
    ""smallest_denomination"": 100
  },
  ""czk"": {
    ""priority"": 100,
    ""iso_code"": ""CZK"",
    ""name"": ""Czech Koruna"",
    ""symbol"": ""Kč"",
    ""alternate_symbols"": [],
    ""subunit"": ""Haléř"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""203"",
    ""smallest_denomination"": 100
  },
  ""djf"": {
    ""priority"": 100,
    ""iso_code"": ""DJF"",
    ""name"": ""Djiboutian Franc"",
    ""symbol"": ""Fdj"",
    ""alternate_symbols"": [],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""262"",
    ""smallest_denomination"": 100
  },
  ""dkk"": {
    ""priority"": 100,
    ""iso_code"": ""DKK"",
    ""name"": ""Danish Krone"",
    ""symbol"": ""kr."",
    ""disambiguate_symbol"": ""DKK"",
    ""alternate_symbols"": ["",-""],
    ""subunit"": ""Øre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""208"",
    ""smallest_denomination"": 50
  },
  ""dop"": {
    ""priority"": 100,
    ""iso_code"": ""DOP"",
    ""name"": ""Dominican Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""RD$"",
    ""alternate_symbols"": [""RD$""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""214"",
    ""smallest_denomination"": 100
  },
  ""dzd"": {
    ""priority"": 100,
    ""iso_code"": ""DZD"",
    ""name"": ""Algerian Dinar"",
    ""symbol"": ""د.ج"",
    ""alternate_symbols"": [""DA""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""012"",
    ""smallest_denomination"": 100
  },
  ""egp"": {
    ""priority"": 100,
    ""iso_code"": ""EGP"",
    ""name"": ""Egyptian Pound"",
    ""symbol"": ""ج.م"",
    ""alternate_symbols"": [""LE"", ""E£"", ""L.E.""],
    ""subunit"": ""Piastre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""818"",
    ""smallest_denomination"": 25
  },
  ""ern"": {
    ""priority"": 100,
    ""iso_code"": ""ERN"",
    ""name"": ""Eritrean Nakfa"",
    ""symbol"": ""Nfk"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""232"",
    ""smallest_denomination"": 1
  },
  ""etb"": {
    ""priority"": 100,
    ""iso_code"": ""ETB"",
    ""name"": ""Ethiopian Birr"",
    ""symbol"": ""Br"",
    ""disambiguate_symbol"": ""ETB"",
    ""alternate_symbols"": [],
    ""subunit"": ""Santim"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""230"",
    ""smallest_denomination"": 1
  },
  ""eur"": {
    ""priority"": 2,
    ""iso_code"": ""EUR"",
    ""name"": ""Euro"",
    ""symbol"": ""€"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20AC;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""978"",
    ""smallest_denomination"": 1
  },
  ""fjd"": {
    ""priority"": 100,
    ""iso_code"": ""FJD"",
    ""name"": ""Fijian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""FJ$"",
    ""alternate_symbols"": [""FJ$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""242"",
    ""smallest_denomination"": 5
  },
  ""fkp"": {
    ""priority"": 100,
    ""iso_code"": ""FKP"",
    ""name"": ""Falkland Pound"",
    ""symbol"": ""£"",
    ""disambiguate_symbol"": ""FK£"",
    ""alternate_symbols"": [""FK£""],
    ""subunit"": ""Penny"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""238"",
    ""smallest_denomination"": 1
  },
  ""gbp"": {
    ""priority"": 3,
    ""iso_code"": ""GBP"",
    ""name"": ""British Pound"",
    ""symbol"": ""£"",
    ""alternate_symbols"": [],
    ""subunit"": ""Penny"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""826"",
    ""smallest_denomination"": 1
  },
  ""gel"": {
    ""priority"": 100,
    ""iso_code"": ""GEL"",
    ""name"": ""Georgian Lari"",
    ""symbol"": ""ლ"",
    ""alternate_symbols"": [""lari""],
    ""subunit"": ""Tetri"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""981"",
    ""smallest_denomination"": 1
  },
  ""ghs"": {
    ""priority"": 100,
    ""iso_code"": ""GHS"",
    ""name"": ""Ghanaian Cedi"",
    ""symbol"": ""₵"",
    ""alternate_symbols"": [""GH¢"", ""GH₵""],
    ""subunit"": ""Pesewa"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B5;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""936"",
    ""smallest_denomination"": 1
  },
  ""gip"": {
    ""priority"": 100,
    ""iso_code"": ""GIP"",
    ""name"": ""Gibraltar Pound"",
    ""symbol"": ""£"",
    ""disambiguate_symbol"": ""GIP"",
    ""alternate_symbols"": [],
    ""subunit"": ""Penny"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""292"",
    ""smallest_denomination"": 1
  },
  ""gmd"": {
    ""priority"": 100,
    ""iso_code"": ""GMD"",
    ""name"": ""Gambian Dalasi"",
    ""symbol"": ""D"",
    ""alternate_symbols"": [],
    ""subunit"": ""Butut"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""270"",
    ""smallest_denomination"": 1
  },
  ""gnf"": {
    ""priority"": 100,
    ""iso_code"": ""GNF"",
    ""name"": ""Guinean Franc"",
    ""symbol"": ""Fr"",
    ""disambiguate_symbol"": ""FG"",
    ""alternate_symbols"": [""FG"", ""GFr""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""324"",
    ""smallest_denomination"": 100
  },
  ""gtq"": {
    ""priority"": 100,
    ""iso_code"": ""GTQ"",
    ""name"": ""Guatemalan Quetzal"",
    ""symbol"": ""Q"",
    ""alternate_symbols"": [],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""320"",
    ""smallest_denomination"": 1
  },
  ""gyd"": {
    ""priority"": 100,
    ""iso_code"": ""GYD"",
    ""name"": ""Guyanese Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""G$"",
    ""alternate_symbols"": [""G$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""328"",
    ""smallest_denomination"": 100
  },
  ""hkd"": {
    ""priority"": 100,
    ""iso_code"": ""HKD"",
    ""name"": ""Hong Kong Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""HK$"",
    ""alternate_symbols"": [""HK$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""344"",
    ""smallest_denomination"": 10
  },
  ""hnl"": {
    ""priority"": 100,
    ""iso_code"": ""HNL"",
    ""name"": ""Honduran Lempira"",
    ""symbol"": ""L"",
    ""disambiguate_symbol"": ""HNL"",
    ""alternate_symbols"": [],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""340"",
    ""smallest_denomination"": 5
  },
  ""hrk"": {
    ""priority"": 100,
    ""iso_code"": ""HRK"",
    ""name"": ""Croatian Kuna"",
    ""symbol"": ""kn"",
    ""alternate_symbols"": [],
    ""subunit"": ""Lipa"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""191"",
    ""smallest_denomination"": 1
  },
  ""htg"": {
    ""priority"": 100,
    ""iso_code"": ""HTG"",
    ""name"": ""Haitian Gourde"",
    ""symbol"": ""G"",
    ""alternate_symbols"": [],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""332"",
    ""smallest_denomination"": 5
  },
  ""huf"": {
    ""priority"": 100,
    ""iso_code"": ""HUF"",
    ""name"": ""Hungarian Forint"",
    ""symbol"": ""Ft"",
    ""alternate_symbols"": [],
    ""subunit"": ""Fillér"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""348"",
    ""smallest_denomination"": 500
  },
  ""idr"": {
    ""priority"": 100,
    ""iso_code"": ""IDR"",
    ""name"": ""Indonesian Rupiah"",
    ""symbol"": ""Rp"",
    ""alternate_symbols"": [],
    ""subunit"": ""Sen"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""360"",
    ""smallest_denomination"": 5000
  },
  ""ils"": {
    ""priority"": 100,
    ""iso_code"": ""ILS"",
    ""name"": ""Israeli New Sheqel"",
    ""symbol"": ""₪"",
    ""alternate_symbols"": [""ש״ח"", ""NIS""],
    ""subunit"": ""Agora"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20AA;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""376"",
    ""smallest_denomination"": 10
  },
  ""inr"": {
    ""priority"": 100,
    ""iso_code"": ""INR"",
    ""name"": ""Indian Rupee"",
    ""symbol"": ""₹"",
    ""alternate_symbols"": [""Rs"", ""৳"", ""૱"", ""௹"", ""रु"", ""₨""],
    ""subunit"": ""Paisa"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20b9;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""356"",
    ""smallest_denomination"": 50
  },
  ""iqd"": {
    ""priority"": 100,
    ""iso_code"": ""IQD"",
    ""name"": ""Iraqi Dinar"",
    ""symbol"": ""ع.د"",
    ""alternate_symbols"": [],
    ""subunit"": ""Fils"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""368"",
    ""smallest_denomination"": 50000
  },
  ""irr"": {
    ""priority"": 100,
    ""iso_code"": ""IRR"",
    ""name"": ""Iranian Rial"",
    ""symbol"": ""﷼"",
    ""alternate_symbols"": [],
    ""subunit"": null,
    ""subunit_to_unit"": 1,
    ""symbol_first"": true,
    ""html_entity"": ""&#xFDFC;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""364"",
    ""smallest_denomination"": 5000
  },
  ""isk"": {
    ""priority"": 100,
    ""iso_code"": ""ISK"",
    ""name"": ""Icelandic Króna"",
    ""symbol"": ""kr"",
    ""alternate_symbols"": [""Íkr""],
    ""subunit"": null,
    ""subunit_to_unit"": 1,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""352"",
    ""smallest_denomination"": 1
  },
  ""jmd"": {
    ""priority"": 100,
    ""iso_code"": ""JMD"",
    ""name"": ""Jamaican Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""J$"",
    ""alternate_symbols"": [""J$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""388"",
    ""smallest_denomination"": 1
  },
  ""jod"": {
    ""priority"": 100,
    ""iso_code"": ""JOD"",
    ""name"": ""Jordanian Dinar"",
    ""symbol"": ""د.ا"",
    ""alternate_symbols"": [""JD""],
    ""subunit"": ""Fils"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""400"",
    ""smallest_denomination"": 5
  },
  ""jpy"": {
    ""priority"": 6,
    ""iso_code"": ""JPY"",
    ""name"": ""Japanese Yen"",
    ""symbol"": ""¥"",
    ""alternate_symbols"": [""円"", ""圓""],
    ""subunit"": null,
    ""subunit_to_unit"": 1,
    ""symbol_first"": true,
    ""html_entity"": ""&#x00A5;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""392"",
    ""smallest_denomination"": 1
  },
  ""kes"": {
    ""priority"": 100,
    ""iso_code"": ""KES"",
    ""name"": ""Kenyan Shilling"",
    ""symbol"": ""KSh"",
    ""alternate_symbols"": [""Sh""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""404"",
    ""smallest_denomination"": 50
  },
  ""kgs"": {
    ""priority"": 100,
    ""iso_code"": ""KGS"",
    ""name"": ""Kyrgyzstani Som"",
    ""symbol"": ""som"",
    ""alternate_symbols"": [""сом""],
    ""subunit"": ""Tyiyn"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""417"",
    ""smallest_denomination"": 1
  },
  ""khr"": {
    ""priority"": 100,
    ""iso_code"": ""KHR"",
    ""name"": ""Cambodian Riel"",
    ""symbol"": ""៛"",
    ""alternate_symbols"": [],
    ""subunit"": ""Sen"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x17DB;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""116"",
    ""smallest_denomination"": 5000
  },
  ""kmf"": {
    ""priority"": 100,
    ""iso_code"": ""KMF"",
    ""name"": ""Comorian Franc"",
    ""symbol"": ""Fr"",
    ""disambiguate_symbol"": ""CF"",
    ""alternate_symbols"": [""CF""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""174"",
    ""smallest_denomination"": 100
  },
  ""kpw"": {
    ""priority"": 100,
    ""iso_code"": ""KPW"",
    ""name"": ""North Korean Won"",
    ""symbol"": ""₩"",
    ""alternate_symbols"": [],
    ""subunit"": ""Chŏn"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x20A9;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""408"",
    ""smallest_denomination"": 1
  },
  ""krw"": {
    ""priority"": 100,
    ""iso_code"": ""KRW"",
    ""name"": ""South Korean Won"",
    ""symbol"": ""₩"",
    ""subunit"": null,
    ""subunit_to_unit"": 1,
    ""alternate_symbols"": [],
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A9;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""410"",
    ""smallest_denomination"": 1
  },
  ""kwd"": {
    ""priority"": 100,
    ""iso_code"": ""KWD"",
    ""name"": ""Kuwaiti Dinar"",
    ""symbol"": ""د.ك"",
    ""alternate_symbols"": [""K.D.""],
    ""subunit"": ""Fils"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""414"",
    ""smallest_denomination"": 5
  },
  ""kyd"": {
    ""priority"": 100,
    ""iso_code"": ""KYD"",
    ""name"": ""Cayman Islands Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""CI$"",
    ""alternate_symbols"": [""CI$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""136"",
    ""smallest_denomination"": 1
  },
  ""kzt"": {
    ""priority"": 100,
    ""iso_code"": ""KZT"",
    ""name"": ""Kazakhstani Tenge"",
    ""symbol"": ""〒"",
    ""alternate_symbols"": [],
    ""subunit"": ""Tiyn"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""398"",
    ""smallest_denomination"": 100
  },
  ""lak"": {
    ""priority"": 100,
    ""iso_code"": ""LAK"",
    ""name"": ""Lao Kip"",
    ""symbol"": ""₭"",
    ""alternate_symbols"": [""₭N""],
    ""subunit"": ""Att"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x20AD;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""418"",
    ""smallest_denomination"": 10
  },
  ""lbp"": {
    ""priority"": 100,
    ""iso_code"": ""LBP"",
    ""name"": ""Lebanese Pound"",
    ""symbol"": ""ل.ل"",
    ""alternate_symbols"": [""£"", ""L£""],
    ""subunit"": ""Piastre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""422"",
    ""smallest_denomination"": 25000
  },
  ""lkr"": {
    ""priority"": 100,
    ""iso_code"": ""LKR"",
    ""name"": ""Sri Lankan Rupee"",
    ""symbol"": ""₨"",
    ""disambiguate_symbol"": ""SLRs"",
    ""alternate_symbols"": [""රු"", ""ரூ"", ""SLRs"", ""/-""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x0BF9;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""144"",
    ""smallest_denomination"": 100
  },
  ""lrd"": {
    ""priority"": 100,
    ""iso_code"": ""LRD"",
    ""name"": ""Liberian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""L$"",
    ""alternate_symbols"": [""L$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""430"",
    ""smallest_denomination"": 5
  },
  ""lsl"": {
    ""priority"": 100,
    ""iso_code"": ""LSL"",
    ""name"": ""Lesotho Loti"",
    ""symbol"": ""L"",
    ""disambiguate_symbol"": ""M"",
    ""alternate_symbols"": [""M""],
    ""subunit"": ""Sente"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""426"",
    ""smallest_denomination"": 1
  },
  ""ltl"": {
    ""priority"": 100,
    ""iso_code"": ""LTL"",
    ""name"": ""Lithuanian Litas"",
    ""symbol"": ""Lt"",
    ""alternate_symbols"": [],
    ""subunit"": ""Centas"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""440"",
    ""smallest_denomination"": 1
  },
  ""lvl"": {
    ""priority"": 100,
    ""iso_code"": ""LVL"",
    ""name"": ""Latvian Lats"",
    ""symbol"": ""Ls"",
    ""alternate_symbols"": [],
    ""subunit"": ""Santīms"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""428"",
    ""smallest_denomination"": 1
  },
  ""lyd"": {
    ""priority"": 100,
    ""iso_code"": ""LYD"",
    ""name"": ""Libyan Dinar"",
    ""symbol"": ""ل.د"",
    ""alternate_symbols"": [""LD""],
    ""subunit"": ""Dirham"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""434"",
    ""smallest_denomination"": 50
  },
  ""mad"": {
    ""priority"": 100,
    ""iso_code"": ""MAD"",
    ""name"": ""Moroccan Dirham"",
    ""symbol"": ""د.م."",
    ""alternate_symbols"": [],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""504"",
    ""smallest_denomination"": 1
  },
  ""mdl"": {
    ""priority"": 100,
    ""iso_code"": ""MDL"",
    ""name"": ""Moldovan Leu"",
    ""symbol"": ""L"",
    ""alternate_symbols"": [""lei""],
    ""subunit"": ""Ban"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""498"",
    ""smallest_denomination"": 1
  },
  ""mga"": {
    ""priority"": 100,
    ""iso_code"": ""MGA"",
    ""name"": ""Malagasy Ariary"",
    ""symbol"": ""Ar"",
    ""alternate_symbols"": [],
    ""subunit"": ""Iraimbilanja"",
    ""subunit_to_unit"": 5,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""969"",
    ""smallest_denomination"": 1
  },
  ""mkd"": {
    ""priority"": 100,
    ""iso_code"": ""MKD"",
    ""name"": ""Macedonian Denar"",
    ""symbol"": ""ден"",
    ""alternate_symbols"": [],
    ""subunit"": ""Deni"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""807"",
    ""smallest_denomination"": 100
  },
  ""mmk"": {
    ""priority"": 100,
    ""iso_code"": ""MMK"",
    ""name"": ""Myanmar Kyat"",
    ""symbol"": ""K"",
    ""disambiguate_symbol"": ""MMK"",
    ""alternate_symbols"": [],
    ""subunit"": ""Pya"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""104"",
    ""smallest_denomination"": 50
  },
  ""mnt"": {
    ""priority"": 100,
    ""iso_code"": ""MNT"",
    ""name"": ""Mongolian Tögrög"",
    ""symbol"": ""₮"",
    ""alternate_symbols"": [],
    ""subunit"": ""Möngö"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x20AE;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""496"",
    ""smallest_denomination"": 2000
  },
  ""mop"": {
    ""priority"": 100,
    ""iso_code"": ""MOP"",
    ""name"": ""Macanese Pataca"",
    ""symbol"": ""P"",
    ""alternate_symbols"": [""MOP$""],
    ""subunit"": ""Avo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""446"",
    ""smallest_denomination"": 10
  },
  ""mro"": {
    ""priority"": 100,
    ""iso_code"": ""MRO"",
    ""name"": ""Mauritanian Ouguiya"",
    ""symbol"": ""UM"",
    ""alternate_symbols"": [],
    ""subunit"": ""Khoums"",
    ""subunit_to_unit"": 5,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""478"",
    ""smallest_denomination"": 1
  },
  ""mur"": {
    ""priority"": 100,
    ""iso_code"": ""MUR"",
    ""name"": ""Mauritian Rupee"",
    ""symbol"": ""₨"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A8;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""480"",
    ""smallest_denomination"": 100
  },
  ""mvr"": {
    ""priority"": 100,
    ""iso_code"": ""MVR"",
    ""name"": ""Maldivian Rufiyaa"",
    ""symbol"": ""MVR"",
    ""alternate_symbols"": [""MRF"", ""Rf"", ""/-"", ""ރ""],
    ""subunit"": ""Laari"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""462"",
    ""smallest_denomination"": 1
  },
  ""mwk"": {
    ""priority"": 100,
    ""iso_code"": ""MWK"",
    ""name"": ""Malawian Kwacha"",
    ""symbol"": ""MK"",
    ""alternate_symbols"": [],
    ""subunit"": ""Tambala"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""454"",
    ""smallest_denomination"": 1
  },
  ""mxn"": {
    ""priority"": 100,
    ""iso_code"": ""MXN"",
    ""name"": ""Mexican Peso"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""MEX$"",
    ""alternate_symbols"": [""MEX$""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""484"",
    ""smallest_denomination"": 5
  },
  ""myr"": {
    ""priority"": 100,
    ""iso_code"": ""MYR"",
    ""name"": ""Malaysian Ringgit"",
    ""symbol"": ""RM"",
    ""alternate_symbols"": [],
    ""subunit"": ""Sen"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""458"",
    ""smallest_denomination"": 5
  },
  ""mzn"": {
    ""priority"": 100,
    ""iso_code"": ""MZN"",
    ""name"": ""Mozambican Metical"",
    ""symbol"": ""MTn"",
    ""alternate_symbols"": [""MZN""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""943"",
    ""smallest_denomination"": 1
  },
  ""nad"": {
    ""priority"": 100,
    ""iso_code"": ""NAD"",
    ""name"": ""Namibian Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""N$"",
    ""alternate_symbols"": [""N$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""516"",
    ""smallest_denomination"": 5
  },
  ""ngn"": {
    ""priority"": 100,
    ""iso_code"": ""NGN"",
    ""name"": ""Nigerian Naira"",
    ""symbol"": ""₦"",
    ""alternate_symbols"": [],
    ""subunit"": ""Kobo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A6;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""566"",
    ""smallest_denomination"": 50
  },
  ""nio"": {
    ""priority"": 100,
    ""iso_code"": ""NIO"",
    ""name"": ""Nicaraguan Córdoba"",
    ""symbol"": ""C$"",
    ""alternate_symbols"": [],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""558"",
    ""smallest_denomination"": 5
  },
  ""nok"": {
    ""priority"": 100,
    ""iso_code"": ""NOK"",
    ""name"": ""Norwegian Krone"",
    ""symbol"": ""kr"",
    ""disambiguate_symbol"": ""NOK"",
    ""alternate_symbols"": ["",-""],
    ""subunit"": ""Øre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""kr"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""578"",
    ""smallest_denomination"": 100
  },
  ""npr"": {
    ""priority"": 100,
    ""iso_code"": ""NPR"",
    ""name"": ""Nepalese Rupee"",
    ""symbol"": ""₨"",
    ""disambiguate_symbol"": ""NPR"",
    ""alternate_symbols"": [""Rs"", ""रू""],
    ""subunit"": ""Paisa"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A8;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""524"",
    ""smallest_denomination"": 1
  },
  ""nzd"": {
    ""priority"": 100,
    ""iso_code"": ""NZD"",
    ""name"": ""New Zealand Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""NZ$"",
    ""alternate_symbols"": [""NZ$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""554"",
    ""smallest_denomination"": 10
  },
  ""omr"": {
    ""priority"": 100,
    ""iso_code"": ""OMR"",
    ""name"": ""Omani Rial"",
    ""symbol"": ""ر.ع."",
    ""alternate_symbols"": [],
    ""subunit"": ""Baisa"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": true,
    ""html_entity"": ""&#xFDFC;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""512"",
    ""smallest_denomination"": 5
  },
  ""pab"": {
    ""priority"": 100,
    ""iso_code"": ""PAB"",
    ""name"": ""Panamanian Balboa"",
    ""symbol"": ""B/."",
    ""alternate_symbols"": [],
    ""subunit"": ""Centésimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""590"",
    ""smallest_denomination"": 1
  },
  ""pen"": {
    ""priority"": 100,
    ""iso_code"": ""PEN"",
    ""name"": ""Peruvian Nuevo Sol"",
    ""symbol"": ""S/."",
    ""alternate_symbols"": [],
    ""subunit"": ""Céntimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""S/."",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""604"",
    ""smallest_denomination"": 1
  },
  ""pgk"": {
    ""priority"": 100,
    ""iso_code"": ""PGK"",
    ""name"": ""Papua New Guinean Kina"",
    ""symbol"": ""K"",
    ""disambiguate_symbol"": ""PGK"",
    ""alternate_symbols"": [],
    ""subunit"": ""Toea"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""598"",
    ""smallest_denomination"": 5
  },
  ""php"": {
    ""priority"": 100,
    ""iso_code"": ""PHP"",
    ""name"": ""Philippine Peso"",
    ""symbol"": ""₱"",
    ""alternate_symbols"": [""PHP"", ""PhP"", ""P""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""608"",
    ""smallest_denomination"": 1
  },
  ""pkr"": {
    ""priority"": 100,
    ""iso_code"": ""PKR"",
    ""name"": ""Pakistani Rupee"",
    ""symbol"": ""₨"",
    ""disambiguate_symbol"": ""PKR"",
    ""alternate_symbols"": [""Rs""],
    ""subunit"": ""Paisa"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A8;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""586"",
    ""smallest_denomination"": 100
  },
  ""pln"": {
    ""priority"": 100,
    ""iso_code"": ""PLN"",
    ""name"": ""Polish Złoty"",
    ""symbol"": ""zł"",
    ""alternate_symbols"": [],
    ""subunit"": ""Grosz"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""z&#322;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": "" "",
    ""iso_numeric"": ""985"",
    ""smallest_denomination"": 1
  },
  ""pyg"": {
    ""priority"": 100,
    ""iso_code"": ""PYG"",
    ""name"": ""Paraguayan Guaraní"",
    ""symbol"": ""₲"",
    ""alternate_symbols"": [],
    ""subunit"": ""Céntimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B2;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""600"",
    ""smallest_denomination"": 5000
  },
  ""qar"": {
    ""priority"": 100,
    ""iso_code"": ""QAR"",
    ""name"": ""Qatari Riyal"",
    ""symbol"": ""ر.ق"",
    ""alternate_symbols"": [""QR""],
    ""subunit"": ""Dirham"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#xFDFC;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""634"",
    ""smallest_denomination"": 1
  },
  ""ron"": {
    ""priority"": 100,
    ""iso_code"": ""RON"",
    ""name"": ""Romanian Leu"",
    ""symbol"": ""Lei"",
    ""alternate_symbols"": [],
    ""subunit"": ""Bani"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""946"",
    ""smallest_denomination"": 1
  },
  ""rsd"": {
    ""priority"": 100,
    ""iso_code"": ""RSD"",
    ""name"": ""Serbian Dinar"",
    ""symbol"": ""РСД"",
    ""alternate_symbols"": [""RSD"", ""din"", ""дин""],
    ""subunit"": ""Para"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""941"",
    ""smallest_denomination"": 100
  },
  ""rub"": {
    ""priority"": 100,
    ""iso_code"": ""RUB"",
    ""name"": ""Russian Ruble"",
    ""symbol"": ""₽"",
    ""alternate_symbols"": [""руб."", ""р.""],
    ""subunit"": ""Kopeck"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x20BD;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""643"",
    ""smallest_denomination"": 1
  },
  ""rwf"": {
    ""priority"": 100,
    ""iso_code"": ""RWF"",
    ""name"": ""Rwandan Franc"",
    ""symbol"": ""FRw"",
    ""alternate_symbols"": [""RF"", ""R₣""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""646"",
    ""smallest_denomination"": 100
  },
  ""sar"": {
    ""priority"": 100,
    ""iso_code"": ""SAR"",
    ""name"": ""Saudi Riyal"",
    ""symbol"": ""ر.س"",
    ""alternate_symbols"": [""SR"", ""﷼""],
    ""subunit"": ""Hallallah"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#xFDFC;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""682"",
    ""smallest_denomination"": 5
  },
  ""sbd"": {
    ""priority"": 100,
    ""iso_code"": ""SBD"",
    ""name"": ""Solomon Islands Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""SI$"",
    ""alternate_symbols"": [""SI$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""090"",
    ""smallest_denomination"": 10
  },
  ""scr"": {
    ""priority"": 100,
    ""iso_code"": ""SCR"",
    ""name"": ""Seychellois Rupee"",
    ""symbol"": ""₨"",
    ""disambiguate_symbol"": ""SRe"",
    ""alternate_symbols"": [""SRe"", ""SR""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x20A8;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""690"",
    ""smallest_denomination"": 1
  },
  ""sdg"": {
    ""priority"": 100,
    ""iso_code"": ""SDG"",
    ""name"": ""Sudanese Pound"",
    ""symbol"": ""£"",
    ""disambiguate_symbol"": ""SDG"",
    ""alternate_symbols"": [],
    ""subunit"": ""Piastre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""938"",
    ""smallest_denomination"": 1
  },
  ""sek"": {
    ""priority"": 100,
    ""iso_code"": ""SEK"",
    ""name"": ""Swedish Krona"",
    ""symbol"": ""kr"",
    ""disambiguate_symbol"": ""SEK"",
    ""alternate_symbols"": ["":-""],
    ""subunit"": ""Öre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": "" "",
    ""iso_numeric"": ""752"",
    ""smallest_denomination"": 100
  },
  ""sgd"": {
    ""priority"": 100,
    ""iso_code"": ""SGD"",
    ""name"": ""Singapore Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""S$"",
    ""alternate_symbols"": [""S$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""702"",
    ""smallest_denomination"": 1
  },
  ""shp"": {
    ""priority"": 100,
    ""iso_code"": ""SHP"",
    ""name"": ""Saint Helenian Pound"",
    ""symbol"": ""£"",
    ""disambiguate_symbol"": ""SHP"",
    ""alternate_symbols"": [],
    ""subunit"": ""Penny"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""654"",
    ""smallest_denomination"": 1
  },
  ""skk"": {
    ""priority"": 100,
    ""iso_code"": ""SKK"",
    ""name"": ""Slovak Koruna"",
    ""symbol"": ""Sk"",
    ""alternate_symbols"": [],
    ""subunit"": ""Halier"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""703"",
    ""smallest_denomination"": 50
  },
  ""sll"": {
    ""priority"": 100,
    ""iso_code"": ""SLL"",
    ""name"": ""Sierra Leonean Leone"",
    ""symbol"": ""Le"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""694"",
    ""smallest_denomination"": 1000
  },
  ""sos"": {
    ""priority"": 100,
    ""iso_code"": ""SOS"",
    ""name"": ""Somali Shilling"",
    ""symbol"": ""Sh"",
    ""alternate_symbols"": [""Sh.So""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""706"",
    ""smallest_denomination"": 1
  },
  ""srd"": {
    ""priority"": 100,
    ""iso_code"": ""SRD"",
    ""name"": ""Surinamese Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""SRD"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""968"",
    ""smallest_denomination"": 1
  },
  ""ssp"": {
    ""priority"": 100,
    ""iso_code"": ""SSP"",
    ""name"": ""South Sudanese Pound"",
    ""symbol"": ""£"",
    ""disambiguate_symbol"": ""SSP"",
    ""alternate_symbols"": [],
    ""subunit"": ""piaster"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""728"",
    ""smallest_denomination"": 5
  },
  ""std"": {
    ""priority"": 100,
    ""iso_code"": ""STD"",
    ""name"": ""São Tomé and Príncipe Dobra"",
    ""symbol"": ""Db"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cêntimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""678"",
    ""smallest_denomination"": 10000
  },
  ""svc"": {
    ""priority"": 100,
    ""iso_code"": ""SVC"",
    ""name"": ""Salvadoran Colón"",
    ""symbol"": ""₡"",
    ""alternate_symbols"": [""¢""],
    ""subunit"": ""Centavo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20A1;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""222"",
    ""smallest_denomination"": 1
  },
  ""syp"": {
    ""priority"": 100,
    ""iso_code"": ""SYP"",
    ""name"": ""Syrian Pound"",
    ""symbol"": ""£S"",
    ""alternate_symbols"": [""£"", ""ل.س"", ""LS"", ""الليرة السورية""],
    ""subunit"": ""Piastre"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x00A3;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""760"",
    ""smallest_denomination"": 100
  },
  ""szl"": {
    ""priority"": 100,
    ""iso_code"": ""SZL"",
    ""name"": ""Swazi Lilangeni"",
    ""symbol"": ""E"",
    ""disambiguate_symbol"": ""SZL"",
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""748"",
    ""smallest_denomination"": 1
  },
  ""thb"": {
    ""priority"": 100,
    ""iso_code"": ""THB"",
    ""name"": ""Thai Baht"",
    ""symbol"": ""฿"",
    ""alternate_symbols"": [],
    ""subunit"": ""Satang"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x0E3F;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""764"",
    ""smallest_denomination"": 1
  },
  ""tjs"": {
    ""priority"": 100,
    ""iso_code"": ""TJS"",
    ""name"": ""Tajikistani Somoni"",
    ""symbol"": ""ЅМ"",
    ""alternate_symbols"": [],
    ""subunit"": ""Diram"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""972"",
    ""smallest_denomination"": 1
  },
  ""tmt"": {
    ""priority"": 100,
    ""iso_code"": ""TMT"",
    ""name"": ""Turkmenistani Manat"",
    ""symbol"": ""T"",
    ""alternate_symbols"": [],
    ""subunit"": ""Tenge"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""934"",
    ""smallest_denomination"": 1
  },
  ""tnd"": {
    ""priority"": 100,
    ""iso_code"": ""TND"",
    ""name"": ""Tunisian Dinar"",
    ""symbol"": ""د.ت"",
    ""alternate_symbols"": [""TD"", ""DT""],
    ""subunit"": ""Millime"",
    ""subunit_to_unit"": 1000,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""788"",
    ""smallest_denomination"": 10
  },
  ""top"": {
    ""priority"": 100,
    ""iso_code"": ""TOP"",
    ""name"": ""Tongan Paʻanga"",
    ""symbol"": ""T$"",
    ""alternate_symbols"": [""PT""],
    ""subunit"": ""Seniti"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""776"",
    ""smallest_denomination"": 1
  },
  ""try"": {
    ""priority"": 100,
    ""iso_code"": ""TRY"",
    ""name"": ""Turkish Lira"",
    ""symbol"": ""₺"",
    ""alternate_symbols"": [""TL""],
    ""subunit"": ""kuruş"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#8378;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""949"",
    ""smallest_denomination"": 1
  },
  ""ttd"": {
    ""priority"": 100,
    ""iso_code"": ""TTD"",
    ""name"": ""Trinidad and Tobago Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""TT$"",
    ""alternate_symbols"": [""TT$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""780"",
    ""smallest_denomination"": 1
  },
  ""twd"": {
    ""priority"": 100,
    ""iso_code"": ""TWD"",
    ""name"": ""New Taiwan Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""NT$"",
    ""alternate_symbols"": [""NT$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""901"",
    ""smallest_denomination"": 50
  },
  ""tzs"": {
    ""priority"": 100,
    ""iso_code"": ""TZS"",
    ""name"": ""Tanzanian Shilling"",
    ""symbol"": ""Sh"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""834"",
    ""smallest_denomination"": 5000
  },
  ""uah"": {
    ""priority"": 100,
    ""iso_code"": ""UAH"",
    ""name"": ""Ukrainian Hryvnia"",
    ""symbol"": ""₴"",
    ""alternate_symbols"": [],
    ""subunit"": ""Kopiyka"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#x20B4;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""980"",
    ""smallest_denomination"": 1
  },
  ""ugx"": {
    ""priority"": 100,
    ""iso_code"": ""UGX"",
    ""name"": ""Ugandan Shilling"",
    ""symbol"": ""USh"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""800"",
    ""smallest_denomination"": 1000
  },
  ""usd"": {
    ""priority"": 1,
    ""iso_code"": ""USD"",
    ""name"": ""United States Dollar"",
    ""symbol"": ""$"",
    ""alternate_symbols"": [""US$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""840"",
    ""smallest_denomination"": 1
  },
  ""uyu"": {
    ""priority"": 100,
    ""iso_code"": ""UYU"",
    ""name"": ""Uruguayan Peso"",
    ""symbol"": ""$"",
    ""alternate_symbols"": [""$U""],
    ""subunit"": ""Centésimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20B1;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""858"",
    ""smallest_denomination"": 100
  },
  ""uzs"": {
    ""priority"": 100,
    ""iso_code"": ""UZS"",
    ""name"": ""Uzbekistani Som"",
    ""symbol"": null,
    ""alternate_symbols"": [],
    ""subunit"": ""Tiyin"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""860"",
    ""smallest_denomination"": 100
  },
  ""vef"": {
    ""priority"": 100,
    ""iso_code"": ""VEF"",
    ""name"": ""Venezuelan Bolívar"",
    ""symbol"": ""Bs"",
    ""alternate_symbols"": [""Bs.F""],
    ""subunit"": ""Céntimo"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""937"",
    ""smallest_denomination"": 1
  },
  ""vnd"": {
    ""priority"": 100,
    ""iso_code"": ""VND"",
    ""name"": ""Vietnamese Đồng"",
    ""symbol"": ""₫"",
    ""alternate_symbols"": [],
    ""subunit"": ""Hào"",
    ""subunit_to_unit"": 1,
    ""symbol_first"": true,
    ""html_entity"": ""&#x20AB;"",
    ""decimal_mark"": "","",
    ""thousands_separator"": ""."",
    ""iso_numeric"": ""704"",
    ""smallest_denomination"": 100
  },
  ""vuv"": {
    ""priority"": 100,
    ""iso_code"": ""VUV"",
    ""name"": ""Vanuatu Vatu"",
    ""symbol"": ""Vt"",
    ""alternate_symbols"": [],
    ""subunit"": null,
    ""subunit_to_unit"": 1,
    ""symbol_first"": true,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""548"",
    ""smallest_denomination"": 1
  },
  ""wst"": {
    ""priority"": 100,
    ""iso_code"": ""WST"",
    ""name"": ""Samoan Tala"",
    ""symbol"": ""T"",
    ""disambiguate_symbol"": ""WS$"",
    ""alternate_symbols"": [""WS$"", ""SAT"", ""ST""],
    ""subunit"": ""Sene"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""882"",
    ""smallest_denomination"": 10
  },
  ""xaf"": {
    ""priority"": 100,
    ""iso_code"": ""XAF"",
    ""name"": ""Central African Cfa Franc"",
    ""symbol"": ""Fr"",
    ""disambiguate_symbol"": ""FCFA"",
    ""alternate_symbols"": [""FCFA""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""950"",
    ""smallest_denomination"": 100
  },
  ""xag"": {
    ""priority"": 100,
    ""iso_code"": ""XAG"",
    ""name"": ""Silver (Troy Ounce)"",
    ""symbol"": ""oz t"",
    ""disambiguate_symbol"": ""XAG"",
    ""alternate_symbols"": [],
    ""subunit"": ""oz"",
    ""subunit_to_unit"": 1,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""961""
  },
  ""xau"": {
    ""priority"": 100,
    ""iso_code"": ""XAU"",
    ""name"": ""Gold (Troy Ounce)"",
    ""symbol"": ""oz t"",
    ""disambiguate_symbol"": ""XAU"",
    ""alternate_symbols"": [],
    ""subunit"": ""oz"",
    ""subunit_to_unit"": 1,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""959""
  },
  ""xcd"": {
    ""priority"": 100,
    ""iso_code"": ""XCD"",
    ""name"": ""East Caribbean Dollar"",
    ""symbol"": ""$"",
    ""disambiguate_symbol"": ""EX$"",
    ""alternate_symbols"": [""EC$""],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""951"",
    ""smallest_denomination"": 1
  },
  ""xdr"": {
    ""priority"": 100,
    ""iso_code"": ""XDR"",
    ""name"": ""Special Drawing Rights"",
    ""symbol"": ""SDR"",
    ""alternate_symbols"": [""XDR""],
    ""subunit"": """",
    ""subunit_to_unit"": 1,
    ""symbol_first"": false,
    ""html_entity"": ""$"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""960""
  },
  ""xof"": {
    ""priority"": 100,
    ""iso_code"": ""XOF"",
    ""name"": ""West African Cfa Franc"",
    ""symbol"": ""Fr"",
    ""disambiguate_symbol"": ""CFA"",
    ""alternate_symbols"": [""CFA""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""952"",
    ""smallest_denomination"": 100
  },
  ""xpf"": {
    ""priority"": 100,
    ""iso_code"": ""XPF"",
    ""name"": ""Cfp Franc"",
    ""symbol"": ""Fr"",
    ""alternate_symbols"": [""F""],
    ""subunit"": ""Centime"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""953"",
    ""smallest_denomination"": 100
  },
  ""yer"": {
    ""priority"": 100,
    ""iso_code"": ""YER"",
    ""name"": ""Yemeni Rial"",
    ""symbol"": ""﷼"",
    ""alternate_symbols"": [],
    ""subunit"": ""Fils"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": ""&#xFDFC;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""886"",
    ""smallest_denomination"": 100
  },
  ""zar"": {
    ""priority"": 100,
    ""iso_code"": ""ZAR"",
    ""name"": ""South African Rand"",
    ""symbol"": ""R"",
    ""alternate_symbols"": [],
    ""subunit"": ""Cent"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": true,
    ""html_entity"": ""&#x0052;"",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""710"",
    ""smallest_denomination"": 10
  },
  ""zmk"": {
    ""priority"": 100,
    ""iso_code"": ""ZMK"",
    ""name"": ""Zambian Kwacha"",
    ""symbol"": ""ZK"",
    ""disambiguate_symbol"": ""ZMK"",
    ""alternate_symbols"": [],
    ""subunit"": ""Ngwee"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""894"",
    ""smallest_denomination"": 5
  },
  ""zmw"": {
    ""priority"": 100,
    ""iso_code"": ""ZMW"",
    ""name"": ""Zambian Kwacha"",
    ""symbol"": ""ZK"",
    ""disambiguate_symbol"": ""ZMW"",
    ""alternate_symbols"": [],
    ""subunit"": ""Ngwee"",
    ""subunit_to_unit"": 100,
    ""symbol_first"": false,
    ""html_entity"": """",
    ""decimal_mark"": ""."",
    ""thousands_separator"": "","",
    ""iso_numeric"": ""967"",
    ""smallest_denomination"": 5
  }
}";
			#endregion

			var currencyDict = JsonSerializer.Deserialize<Dictionary<string, WvCurrency>>(currencyJson);

			foreach (var property in currencyDict.Keys)
			{
				//var objectJson = JsonSerializer.Serialize(property);
				//var currency = JsonSerializer.Deserialize<WvCurrency>(objectJson);
                result.Add(currencyDict[property]);
			}
			result = result.OrderBy(x => x.Priority).ToList();

			return result;
		}

		public static WvCurrency GetCurrency(string currencyCode)
		{
			return GetAllCurrency().FirstOrDefault(x => x.IsoCode.ToLowerInvariant() == currencyCode.ToLowerInvariant());
		}

		public static List<WvCurrencyType> GetAllCurrencyType()
		{
			var result = new List<WvCurrencyType>();
			var getAllCurrency = GetAllCurrency();
			foreach (var currency in getAllCurrency)
			{
				var decimalDigits = 2;
				if (currency.SubUnitToUnit == 1000)
				{
					decimalDigits = 3;
				}

				var symbol = currency.IsoCode.ToUpperInvariant();
				if (currency.AlternateSymbols.Count > 0)
				{
					symbol = currency.AlternateSymbols[0];
				}

				var symPlacement = CurrencySymbolPlacement.After;
				if (currency.SymbolFirst)
				{
					symPlacement = CurrencySymbolPlacement.Before;
				}

				WvCurrencyType model = new WvCurrencyType()
				{
					Code = currency.IsoCode.ToUpperInvariant(),
					DecimalDigits = decimalDigits,
					Name = currency.Name,
					NamePlural = currency.Name,
					Rounding = 0,
					SymbolNative = currency.Symbol,
					Symbol = symbol,
					SymbolPlacement = symPlacement
				};
				result.Add(model);
			}

			return result;
		}

		public static WvCurrencyType GetCurrencyType(string currencyCode)
		{
			WvCurrencyType result = null;
			var currency = GetCurrency(currencyCode);

			if (currency != null)
			{
				var decimalDigits = 2;
				if (currency.SubUnitToUnit == 1000)
				{
					decimalDigits = 3;
				}

				var symbol = currency.IsoCode.ToUpperInvariant();
				if (currency.AlternateSymbols.Count > 0)
				{
					symbol = currency.AlternateSymbols[0];
				}

				var symPlacement = CurrencySymbolPlacement.After;
				if (currency.SymbolFirst)
				{
					symPlacement = CurrencySymbolPlacement.Before;
				}

				WvCurrencyType model = new WvCurrencyType()
				{
					Code = currency.IsoCode.ToUpperInvariant(),
					DecimalDigits = decimalDigits,
					Name = currency.Name,
					NamePlural = currency.Name,
					Rounding = 0,
					SymbolNative = currency.Symbol,
					Symbol = symbol,
					SymbolPlacement = symPlacement
				};

				return model;
			}
			return result;
		}

		public static List<WvIcon> fontAwesomeIcons = null;
		public static List<WvIcon> FontAwesomeIcons
		{
			get
			{
				if (fontAwesomeIcons == null)
					fontAwesomeIcons = GetAllFontAwesomeIcons().OrderBy(x => x.Name).ToList();

				return fontAwesomeIcons;
			}
		}

		public static List<WvIcon> GetAllFontAwesomeIcons()
		{

			var icons = new List<WvIcon>();

			#region << Icon Json >>
			var json = @"
{
	""fab fa-buromobelexperte"": ""&#xf37f; B\u00fcrom\u00f6bel-Experte GmbH & Co. KG. brands"",
	""fas fa-burn"": ""&#xf46a; Burn solid"",
	""fas fa-bullseye"": ""&#xf140; Bullseye solid"",
	""fas fa-bullhorn"": ""&#xf0a1; bullhorn solid"",
	""fas fa-building"": ""&#xf1ad; Building solid"",
	""far fa-building"": ""&#xf1ad; Building regular"",
	""fas fa-bug"": ""&#xf188; Bug solid"",
	""fab fa-btc"": ""&#xf15a; BTC brands"",
	""fas fa-briefcase-medical"": ""&#xf469; Medical Briefcase solid"",
	""fas fa-briefcase"": ""&#xf0b1; Briefcase solid"",
	""fas fa-braille"": ""&#xf2a1; Braille solid"",
	""fas fa-boxes"": ""&#xf468; Boxes solid"",
	""fas fa-box-open"": ""&#xf49e; Box Open solid"",
	""fas fa-box"": ""&#xf466; Box solid"",
	""fas fa-bowling-ball"": ""&#xf436; Bowling Ball solid"",
	""fas fa-bookmark"": ""&#xf02e; bookmark solid"",
	""far fa-bookmark"": ""&#xf02e; bookmark regular"",
	""fas fa-book"": ""&#xf02d; book solid"",
	""fas fa-bomb"": ""&#xf1e2; Bomb solid"",
	""fas fa-bolt"": ""&#xf0e7; Lightning Bolt solid"",
	""fas fa-bold"": ""&#xf032; bold solid"",
	""fab fa-bluetooth-b"": ""&#xf294; Bluetooth brands"",
	""fab fa-bluetooth"": ""&#xf293; Bluetooth brands"",
	""fab fa-blogger-b"": ""&#xf37d; Blogger B brands"",
	""fab fa-blogger"": ""&#xf37c; Blogger brands"",
	""fas fa-blind"": ""&#xf29d; Blind solid"",
	""fab fa-blackberry"": ""&#xf37b; BlackBerry brands"",
	""fab fa-black-tie"": ""&#xf27e; Font Awesome Black Tie brands"",
	""fab fa-bity"": ""&#xf37a; Bity brands"",
	""fab fa-bitcoin"": ""&#xf379; Bitcoin brands"",
	""fab fa-bitbucket"": ""&#xf171; Bitbucket brands"",
	""fas fa-birthday-cake"": ""&#xf1fd; Birthday Cake solid"",
	""fas fa-binoculars"": ""&#xf1e5; Binoculars solid"",
	""fab fa-bimobject"": ""&#xf378; BIMobject brands"",
	""fas fa-bicycle"": ""&#xf206; Bicycle solid"",
	""fas fa-bell-slash"": ""&#xf1f6; Bell Slash solid"",
	""far fa-bell-slash"": ""&#xf1f6; Bell Slash regular"",
	""fas fa-bell"": ""&#xf0f3; bell solid"",
	""far fa-bell"": ""&#xf0f3; bell regular"",
	""fab fa-behance-square"": ""&#xf1b5; Behance Square brands"",
	""fab fa-behance"": ""&#xf1b4; Behance brands"",
	""fas fa-beer"": ""&#xf0fc; beer solid"",
	""fas fa-bed"": ""&#xf236; Bed solid"",
	""fas fa-battery-three-quarters"": ""&#xf241; Battery 3\/4 Full solid"",
	""fas fa-battery-quarter"": ""&#xf243; Battery 1\/4 Full solid"",
	""fas fa-battery-half"": ""&#xf242; Battery 1\/2 Full solid"",
	""fas fa-battery-full"": ""&#xf240; Battery Full solid"",
	""fas fa-battery-empty"": ""&#xf244; Battery Empty solid"",
	""fas fa-bath"": ""&#xf2cd; Bath solid"",
	""fas fa-basketball-ball"": ""&#xf434; Basketball Ball solid"",
	""fas fa-baseball-ball"": ""&#xf433; Baseball Ball solid"",
	""fas fa-bars"": ""&#xf0c9; Bars solid"",
	""fas fa-barcode"": ""&#xf02a; barcode solid"",
	""fab fa-bandcamp"": ""&#xf2d5; Bandcamp brands"",
	""fas fa-band-aid"": ""&#xf462; Band-Aid solid"",
	""fas fa-ban"": ""&#xf05e; ban solid"",
	""fas fa-balance-scale"": ""&#xf24e; Balance Scale solid"",
	""fas fa-backward"": ""&#xf04a; backward solid"",
	""fab fa-aws"": ""&#xf375; Amazon Web Services (AWS) brands"",
	""fab fa-aviato"": ""&#xf421; Aviato brands"",
	""fab fa-avianex"": ""&#xf374; avianex brands"",
	""fab fa-autoprefixer"": ""&#xf41c; Autoprefixer brands"",
	""fas fa-audio-description"": ""&#xf29e; Audio Description solid"",
	""fab fa-audible"": ""&#xf373; Audible brands"",
	""fas fa-at"": ""&#xf1fa; At solid"",
	""fab fa-asymmetrik"": ""&#xf372; Asymmetrik, Ltd. brands"",
	""fas fa-asterisk"": ""&#xf069; asterisk solid"",
	""fas fa-assistive-listening-systems"": ""&#xf2a2; Assistive Listening Systems solid"",
	""fas fa-arrows-alt-v"": ""&#xf338; Alternate Arrows Vertical solid"",
	""fas fa-arrows-alt-h"": ""&#xf337; Alternate Arrows Horizontal solid"",
	""fas fa-arrows-alt"": ""&#xf0b2; Alternate Arrows solid"",
	""fas fa-arrow-up"": ""&#xf062; arrow-up solid"",
	""fas fa-arrow-right"": ""&#xf061; arrow-right solid"",
	""fas fa-arrow-left"": ""&#xf060; arrow-left solid"",
	""fas fa-arrow-down"": ""&#xf063; arrow-down solid"",
	""fas fa-arrow-circle-up"": ""&#xf0aa; Arrow Circle Up solid"",
	""fas fa-arrow-circle-right"": ""&#xf0a9; Arrow Circle Right solid"",
	""fas fa-arrow-circle-left"": ""&#xf0a8; Arrow Circle Left solid"",
	""fas fa-arrow-circle-down"": ""&#xf0ab; Arrow Circle Down solid"",
	""fas fa-arrow-alt-circle-up"": ""&#xf35b; Alternate Arrow Circle Up solid"",
	""far fa-arrow-alt-circle-up"": ""&#xf35b; Alternate Arrow Circle Up regular"",
	""fas fa-arrow-alt-circle-right"": ""&#xf35a; Alternate Arrow Circle Right solid"",
	""far fa-arrow-alt-circle-right"": ""&#xf35a; Alternate Arrow Circle Right regular"",
	""fas fa-arrow-alt-circle-left"": ""&#xf359; Alternate Arrow Circle Left solid"",
	""far fa-arrow-alt-circle-left"": ""&#xf359; Alternate Arrow Circle Left regular"",
	""fas fa-arrow-alt-circle-down"": ""&#xf358; Alternate Arrow Circle Down solid"",
	""far fa-arrow-alt-circle-down"": ""&#xf358; Alternate Arrow Circle Down regular"",
	""fas fa-archive"": ""&#xf187; Archive solid"",
	""fab fa-apple-pay"": ""&#xf415; Apple Pay brands"",
	""fab fa-apple"": ""&#xf179; Apple brands"",
	""fab fa-apper"": ""&#xf371; Apper Systems AB brands"",
	""fab fa-app-store-ios"": ""&#xf370; iOS App Store brands"",
	""fab fa-app-store"": ""&#xf36f; App Store brands"",
	""fab fa-angular"": ""&#xf420; Angular brands"",
	""fab fa-angrycreative"": ""&#xf36e; Angry Creative brands"",
	""fas fa-angle-up"": ""&#xf106; angle-up solid"",
	""fas fa-angle-right"": ""&#xf105; angle-right solid"",
	""fas fa-angle-left"": ""&#xf104; angle-left solid"",
	""fas fa-angle-down"": ""&#xf107; angle-down solid"",
	""fas fa-angle-double-up"": ""&#xf102; Angle Double Up solid"",
	""fas fa-angle-double-right"": ""&#xf101; Angle Double Right solid"",
	""fas fa-angle-double-left"": ""&#xf100; Angle Double Left solid"",
	""fas fa-angle-double-down"": ""&#xf103; Angle Double Down solid"",
	""fab fa-angellist"": ""&#xf209; AngelList brands"",
	""fab fa-android"": ""&#xf17b; Android brands"",
	""fas fa-anchor"": ""&#xf13d; Anchor solid"",
	""fab fa-amilia"": ""&#xf36d; Amilia brands"",
	""fas fa-american-sign-language-interpreting"": ""&#xf2a3; American Sign Language Interpreting solid"",
	""fas fa-ambulance"": ""&#xf0f9; ambulance solid"",
	""fab fa-amazon-pay"": ""&#xf42c; Amazon Pay brands"",
	""fab fa-amazon"": ""&#xf270; Amazon brands"",
	""fas fa-allergies"": ""&#xf461; Allergies solid"",
	""fas fa-align-right"": ""&#xf038; align-right solid"",
	""fas fa-align-left"": ""&#xf036; align-left solid"",
	""fas fa-align-justify"": ""&#xf039; align-justify solid"",
	""fas fa-align-center"": ""&#xf037; align-center solid"",
	""fab fa-algolia"": ""&#xf36c; Algolia brands"",
	""fab fa-affiliatetheme"": ""&#xf36b; affiliatetheme brands"",
	""fab fa-adversal"": ""&#xf36a; Adversal brands"",
	""fab fa-adn"": ""&#xf170; App.net brands"",
	""fas fa-adjust"": ""&#xf042; adjust solid"",
	""fas fa-address-card"": ""&#xf2bb; Address Card solid"",
	""far fa-address-card"": ""&#xf2bb; Address Card regular"",
	""fas fa-address-book"": ""&#xf2b9; Address Book solid"",
	""far fa-address-book"": ""&#xf2b9; Address Book regular"",
	""fab fa-accusoft"": ""&#xf369; Accusoft brands"",
	""fab fa-accessible-icon"": ""&#xf368; Accessible Icon brands"",
	""fab fa-500px"": ""&#xf26e; 500px brands"",
	""fab fa-youtube-square"": ""&#xf431; YouTube Square brands"",
	""fab fa-youtube"": ""&#xf167; YouTube brands"",
	""fab fa-yoast"": ""&#xf2b1; Yoast brands"",
	""fas fa-yen-sign"": ""&#xf157; Yen Sign solid"",
	""fab fa-yelp"": ""&#xf1e9; Yelp brands"",
	""fab fa-yandex-international"": ""&#xf414; Yandex International brands"",
	""fab fa-yandex"": ""&#xf413; Yandex brands"",
	""fab fa-yahoo"": ""&#xf19e; Yahoo Logo brands"",
	""fab fa-y-combinator"": ""&#xf23b; Y Combinator brands"",
	""fab fa-xing-square"": ""&#xf169; Xing Square brands"",
	""fab fa-xing"": ""&#xf168; Xing brands"",
	""fab fa-xbox"": ""&#xf412; Xbox brands"",
	""fas fa-x-ray"": ""&#xf497; X-Ray solid"",
	""fas fa-wrench"": ""&#xf0ad; Wrench solid"",
	""fab fa-wpforms"": ""&#xf298; WPForms brands"",
	""fab fa-wpexplorer"": ""&#xf2de; WPExplorer brands"",
	""fab fa-wpbeginner"": ""&#xf297; WPBeginner brands"",
	""fab fa-wordpress-simple"": ""&#xf411; Wordpress Simple brands"",
	""fab fa-wordpress"": ""&#xf19a; WordPress Logo brands"",
	""fas fa-won-sign"": ""&#xf159; Won Sign solid"",
	""fab fa-wolf-pack-battalion"": ""&#xf514; Wolf Pack-battalion brands"",
	""fas fa-wine-glass"": ""&#xf4e3; Wine Glass solid"",
	""fab fa-windows"": ""&#xf17a; Windows brands"",
	""fas fa-window-restore"": ""&#xf2d2; Window Restore solid"",
	""far fa-window-restore"": ""&#xf2d2; Window Restore regular"",
	""fas fa-window-minimize"": ""&#xf2d1; Window Minimize solid"",
	""far fa-window-minimize"": ""&#xf2d1; Window Minimize regular"",
	""fas fa-window-maximize"": ""&#xf2d0; Window Maximize solid"",
	""far fa-window-maximize"": ""&#xf2d0; Window Maximize regular"",
	""fas fa-window-close"": ""&#xf410; Window Close solid"",
	""far fa-window-close"": ""&#xf410; Window Close regular"",
	""fab fa-wikipedia-w"": ""&#xf266; Wikipedia W brands"",
	""fas fa-wifi"": ""&#xf1eb; WiFi solid"",
	""fab fa-whmcs"": ""&#xf40d; WHMCS brands"",
	""fas fa-wheelchair"": ""&#xf193; Wheelchair solid"",
	""fab fa-whatsapp-square"": ""&#xf40c; What's App Square brands"",
	""fab fa-whatsapp"": ""&#xf232; What's App brands"",
	""fab fa-weixin"": ""&#xf1d7; Weixin (WeChat) brands"",
	""fas fa-weight"": ""&#xf496; Weight solid"",
	""fab fa-weibo"": ""&#xf18a; Weibo brands"",
	""fas fa-warehouse"": ""&#xf494; Warehouse solid"",
	""fab fa-vuejs"": ""&#xf41f; Vue.js brands"",
	""fas fa-volume-up"": ""&#xf028; volume-up solid"",
	""fas fa-volume-off"": ""&#xf026; volume-off solid"",
	""fas fa-volume-down"": ""&#xf027; volume-down solid"",
	""fas fa-volleyball-ball"": ""&#xf45f; Volleyball Ball solid"",
	""fab fa-vnv"": ""&#xf40b; VNV brands"",
	""fab fa-vk"": ""&#xf189; VK brands"",
	""fab fa-vine"": ""&#xf1ca; Vine brands"",
	""fab fa-vimeo-v"": ""&#xf27d; Vimeo brands"",
	""fab fa-vimeo-square"": ""&#xf194; Vimeo Square brands"",
	""fab fa-vimeo"": ""&#xf40a; Vimeo brands"",
	""fas fa-video-slash"": ""&#xf4e2; Video Slash solid"",
	""fas fa-video"": ""&#xf03d; Video solid"",
	""fab fa-viber"": ""&#xf409; Viber brands"",
	""fas fa-vials"": ""&#xf493; Vials solid"",
	""fas fa-vial"": ""&#xf492; Vial solid"",
	""fab fa-viadeo-square"": ""&#xf2aa; Viadeo Square brands"",
	""fab fa-viadeo"": ""&#xf2a9; Viadeo brands"",
	""fab fa-viacoin"": ""&#xf237; Viacoin brands"",
	""fas fa-venus-mars"": ""&#xf228; Venus Mars solid"",
	""fas fa-venus-double"": ""&#xf226; Venus Double solid"",
	""fas fa-venus"": ""&#xf221; Venus solid"",
	""fab fa-vaadin"": ""&#xf408; Vaadin brands"",
	""fas fa-utensils"": ""&#xf2e7; Utensils solid"",
	""fas fa-utensil-spoon"": ""&#xf2e5; Utensil Spoon solid"",
	""fab fa-ussunnah"": ""&#xf407; us-Sunnah Foundation brands"",
	""fas fa-users-cog"": ""&#xf509; Users Cog solid"",
	""fas fa-users"": ""&#xf0c0; Users solid"",
	""fas fa-user-times"": ""&#xf235; Remove User solid"",
	""fas fa-user-tie"": ""&#xf508; User Tie solid"",
	""fas fa-user-tag"": ""&#xf507; User Tag solid"",
	""fas fa-user-slash"": ""&#xf506; User Slash solid"",
	""fas fa-user-shield"": ""&#xf505; User Shield solid"",
	""fas fa-user-secret"": ""&#xf21b; User Secret solid"",
	""fas fa-user-plus"": ""&#xf234; Add User solid"",
	""fas fa-user-ninja"": ""&#xf504; User Ninja solid"",
	""fas fa-user-minus"": ""&#xf503; User Minus solid"",
	""fas fa-user-md"": ""&#xf0f0; user-md solid"",
	""fas fa-user-lock"": ""&#xf502; User Lock solid"",
	""fas fa-user-graduate"": ""&#xf501; User Graduate solid"",
	""fas fa-user-friends"": ""&#xf500; User Friends solid"",
	""fas fa-user-edit"": ""&#xf4ff; User Edit solid"",
	""fas fa-user-cog"": ""&#xf4fe; User Cog solid"",
	""fas fa-user-clock"": ""&#xf4fd; User Clock solid"",
	""fas fa-user-circle"": ""&#xf2bd; User Circle solid"",
	""far fa-user-circle"": ""&#xf2bd; User Circle regular"",
	""fas fa-user-check"": ""&#xf4fc; User Check solid"",
	""fas fa-user-astronaut"": ""&#xf4fb; User Astronaut solid"",
	""fas fa-user-alt-slash"": ""&#xf4fa; User Alt-slash solid"",
	""fas fa-user-alt"": ""&#xf406; Alternate User solid"",
	""fas fa-user"": ""&#xf007; User solid"",
	""far fa-user"": ""&#xf007; User regular"",
	""fab fa-usb"": ""&#xf287; USB brands"",
	""fas fa-upload"": ""&#xf093; Upload solid"",
	""fab fa-untappd"": ""&#xf405; Untappd brands"",
	""fas fa-unlock-alt"": ""&#xf13e; Alternate Unlock solid"",
	""fas fa-unlock"": ""&#xf09c; unlock solid"",
	""fas fa-unlink"": ""&#xf127; unlink solid"",
	""fas fa-university"": ""&#xf19c; University solid"",
	""fas fa-universal-access"": ""&#xf29a; Universal Access solid"",
	""fab fa-uniregistry"": ""&#xf404; Uniregistry brands"",
	""fas fa-undo-alt"": ""&#xf2ea; Alternate Undo solid"",
	""fas fa-undo"": ""&#xf0e2; Undo solid"",
	""fas fa-underline"": ""&#xf0cd; Underline solid"",
	""fas fa-umbrella"": ""&#xf0e9; Umbrella solid"",
	""fab fa-uikit"": ""&#xf403; UIkit brands"",
	""fab fa-uber"": ""&#xf402; Uber brands"",
	""fab fa-typo3"": ""&#xf42b; Typo3 brands"",
	""fab fa-twitter-square"": ""&#xf081; Twitter Square brands"",
	""fab fa-twitter"": ""&#xf099; Twitter brands"",
	""fab fa-twitch"": ""&#xf1e8; Twitch brands"",
	""fas fa-tv"": ""&#xf26c; Television solid"",
	""fab fa-tumblr-square"": ""&#xf174; Tumblr Square brands"",
	""fab fa-tumblr"": ""&#xf173; Tumblr brands"",
	""fas fa-tty"": ""&#xf1e4; TTY solid"",
	""fas fa-truck-moving"": ""&#xf4df; Truck Moving solid"",
	""fas fa-truck-loading"": ""&#xf4de; Truck Loading solid"",
	""fas fa-truck"": ""&#xf0d1; truck solid"",
	""fas fa-trophy"": ""&#xf091; trophy solid"",
	""fab fa-tripadvisor"": ""&#xf262; TripAdvisor brands"",
	""fab fa-trello"": ""&#xf181; Trello brands"",
	""fas fa-tree"": ""&#xf1bb; Tree solid"",
	""fas fa-trash-alt"": ""&#xf2ed; Alternate Trash solid"",
	""far fa-trash-alt"": ""&#xf2ed; Alternate Trash regular"",
	""fas fa-trash"": ""&#xf1f8; Trash solid"",
	""fas fa-transgender-alt"": ""&#xf225; Alternate Transgender solid"",
	""fas fa-transgender"": ""&#xf224; Transgender solid"",
	""fas fa-train"": ""&#xf238; Train solid"",
	""fas fa-trademark"": ""&#xf25c; Trademark solid"",
	""fab fa-trade-federation"": ""&#xf513; Trade Federation brands"",
	""fas fa-toggle-on"": ""&#xf205; Toggle On solid"",
	""fas fa-toggle-off"": ""&#xf204; Toggle Off solid"",
	""fas fa-tint"": ""&#xf043; tint solid"",
	""fas fa-times-circle"": ""&#xf057; Times Circle solid"",
	""far fa-times-circle"": ""&#xf057; Times Circle regular"",
	""fas fa-times"": ""&#xf00d; Times solid"",
	""fas fa-ticket-alt"": ""&#xf3ff; Alternate Ticket solid"",
	""fas fa-thumbtack"": ""&#xf08d; Thumbtack solid"",
	""fas fa-thumbs-up"": ""&#xf164; thumbs-up solid"",
	""far fa-thumbs-up"": ""&#xf164; thumbs-up regular"",
	""fas fa-thumbs-down"": ""&#xf165; thumbs-down solid"",
	""far fa-thumbs-down"": ""&#xf165; thumbs-down regular"",
	""fas fa-thermometer-three-quarters"": ""&#xf2c8; Thermometer 3\/4 Full solid"",
	""fas fa-thermometer-quarter"": ""&#xf2ca; Thermometer 1\/4 Full solid"",
	""fas fa-thermometer-half"": ""&#xf2c9; Thermometer 1\/2 Full solid"",
	""fas fa-thermometer-full"": ""&#xf2c7; Thermometer Full solid"",
	""fas fa-thermometer-empty"": ""&#xf2cb; Thermometer Empty solid"",
	""fas fa-thermometer"": ""&#xf491; Thermometer solid"",
	""fab fa-themeisle"": ""&#xf2b2; ThemeIsle brands"",
	""fas fa-th-list"": ""&#xf00b; th-list solid"",
	""fas fa-th-large"": ""&#xf009; th-large solid"",
	""fas fa-th"": ""&#xf00a; th solid"",
	""fas fa-text-width"": ""&#xf035; text-width solid"",
	""fas fa-text-height"": ""&#xf034; text-height solid"",
	""fas fa-terminal"": ""&#xf120; Terminal solid"",
	""fab fa-tencent-weibo"": ""&#xf1d5; Tencent Weibo brands"",
	""fab fa-telegram-plane"": ""&#xf3fe; Telegram Plane brands"",
	""fab fa-telegram"": ""&#xf2c6; Telegram brands"",
	""fab fa-teamspeak"": ""&#xf4f9; TeamSpeak brands"",
	""fas fa-taxi"": ""&#xf1ba; Taxi solid"",
	""fas fa-tasks"": ""&#xf0ae; Tasks solid"",
	""fas fa-tape"": ""&#xf4db; Tape solid"",
	""fas fa-tags"": ""&#xf02c; tags solid"",
	""fas fa-tag"": ""&#xf02b; tag solid"",
	""fas fa-tachometer-alt"": ""&#xf3fd; Alternate Tachometer solid"",
	""fas fa-tablets"": ""&#xf490; Tablets solid"",
	""fas fa-tablet-alt"": ""&#xf3fa; Alternate Tablet solid"",
	""fas fa-tablet"": ""&#xf10a; tablet solid"",
	""fas fa-table-tennis"": ""&#xf45d; Table Tennis solid"",
	""fas fa-table"": ""&#xf0ce; table solid"",
	""fas fa-syringe"": ""&#xf48e; Syringe solid"",
	""fas fa-sync-alt"": ""&#xf2f1; Alternate Sync solid"",
	""fas fa-sync"": ""&#xf021; Sync solid"",
	""fab fa-supple"": ""&#xf3f9; Supple brands"",
	""fas fa-superscript"": ""&#xf12b; superscript solid"",
	""fab fa-superpowers"": ""&#xf2dd; Superpowers brands"",
	""fas fa-sun"": ""&#xf185; Sun solid"",
	""far fa-sun"": ""&#xf185; Sun regular"",
	""fas fa-suitcase"": ""&#xf0f2; Suitcase solid"",
	""fas fa-subway"": ""&#xf239; Subway solid"",
	""fas fa-subscript"": ""&#xf12c; subscript solid"",
	""fab fa-stumbleupon-circle"": ""&#xf1a3; StumbleUpon Circle brands"",
	""fab fa-stumbleupon"": ""&#xf1a4; StumbleUpon Logo brands"",
	""fab fa-studiovinari"": ""&#xf3f8; Studio Vinari brands"",
	""fab fa-stripe-s"": ""&#xf42a; Stripe S brands"",
	""fab fa-stripe"": ""&#xf429; Stripe brands"",
	""fas fa-strikethrough"": ""&#xf0cc; Strikethrough solid"",
	""fas fa-street-view"": ""&#xf21d; Street View solid"",
	""fab fa-strava"": ""&#xf428; Strava brands"",
	""fas fa-stopwatch"": ""&#xf2f2; Stopwatch solid"",
	""fas fa-stop-circle"": ""&#xf28d; Stop Circle solid"",
	""far fa-stop-circle"": ""&#xf28d; Stop Circle regular"",
	""fas fa-stop"": ""&#xf04d; stop solid"",
	""fas fa-sticky-note"": ""&#xf249; Sticky Note solid"",
	""far fa-sticky-note"": ""&#xf249; Sticky Note regular"",
	""fab fa-sticker-mule"": ""&#xf3f7; Sticker Mule brands"",
	""fas fa-stethoscope"": ""&#xf0f1; Stethoscope solid"",
	""fas fa-step-forward"": ""&#xf051; step-forward solid"",
	""fas fa-step-backward"": ""&#xf048; step-backward solid"",
	""fab fa-steam-symbol"": ""&#xf3f6; Steam Symbol brands"",
	""fab fa-steam-square"": ""&#xf1b7; Steam Square brands"",
	""fab fa-steam"": ""&#xf1b6; Steam brands"",
	""fab fa-staylinked"": ""&#xf3f5; StayLinked brands"",
	""fas fa-star-half"": ""&#xf089; star-half solid"",
	""far fa-star-half"": ""&#xf089; star-half regular"",
	""fas fa-star"": ""&#xf005; Star solid"",
	""far fa-star"": ""&#xf005; Star regular"",
	""fab fa-stack-overflow"": ""&#xf16c; Stack Overflow brands"",
	""fab fa-stack-exchange"": ""&#xf18d; Stack Exchange brands"",
	""fas fa-square-full"": ""&#xf45c; Square Full solid"",
	""fas fa-square"": ""&#xf0c8; Square solid"",
	""far fa-square"": ""&#xf0c8; Square regular"",
	""fab fa-spotify"": ""&#xf1bc; Spotify brands"",
	""fas fa-spinner"": ""&#xf110; Spinner solid"",
	""fab fa-speakap"": ""&#xf3f3; Speakap brands"",
	""fas fa-space-shuttle"": ""&#xf197; Space Shuttle solid"",
	""fab fa-soundcloud"": ""&#xf1be; SoundCloud brands"",
	""fas fa-sort-up"": ""&#xf0de; Sort Up (Ascending) solid"",
	""fas fa-sort-numeric-up"": ""&#xf163; Sort Numeric Up solid"",
	""fas fa-sort-numeric-down"": ""&#xf162; Sort Numeric Down solid"",
	""fas fa-sort-down"": ""&#xf0dd; Sort Down (Descending) solid"",
	""fas fa-sort-amount-up"": ""&#xf161; Sort Amount Up solid"",
	""fas fa-sort-amount-down"": ""&#xf160; Sort Amount Down solid"",
	""fas fa-sort-alpha-up"": ""&#xf15e; Sort Alpha Up solid"",
	""fas fa-sort-alpha-down"": ""&#xf15d; Sort Alpha Down solid"",
	""fas fa-sort"": ""&#xf0dc; Sort solid"",
	""fas fa-snowflake"": ""&#xf2dc; Snowflake solid"",
	""far fa-snowflake"": ""&#xf2dc; Snowflake regular"",
	""fab fa-snapchat-square"": ""&#xf2ad; Snapchat Square brands"",
	""fab fa-snapchat-ghost"": ""&#xf2ac; Snapchat Ghost brands"",
	""fab fa-snapchat"": ""&#xf2ab; Snapchat brands"",
	""fas fa-smoking"": ""&#xf48d; Smoking solid"",
	""fas fa-smile"": ""&#xf118; Smile solid"",
	""far fa-smile"": ""&#xf118; Smile regular"",
	""fab fa-slideshare"": ""&#xf1e7; Slideshare brands"",
	""fas fa-sliders-h"": ""&#xf1de; Horizontal Sliders solid"",
	""fab fa-slack-hash"": ""&#xf3ef; Slack Hashtag brands"",
	""fab fa-slack"": ""&#xf198; Slack Logo brands"",
	""fab fa-skype"": ""&#xf17e; Skype brands"",
	""fab fa-skyatlas"": ""&#xf216; skyatlas brands"",
	""fab fa-sith"": ""&#xf512; Sith brands"",
	""fas fa-sitemap"": ""&#xf0e8; Sitemap solid"",
	""fab fa-sistrix"": ""&#xf3ee; SISTRIX brands"",
	""fab fa-simplybuilt"": ""&#xf215; SimplyBuilt brands"",
	""fas fa-signal"": ""&#xf012; signal solid"",
	""fas fa-sign-out-alt"": ""&#xf2f5; Alternate Sign Out solid"",
	""fas fa-sign-language"": ""&#xf2a7; Sign Language solid"",
	""fas fa-sign-in-alt"": ""&#xf2f6; Alternate Sign In solid"",
	""fas fa-sign"": ""&#xf4d9; Sign solid"",
	""fas fa-shower"": ""&#xf2cc; Shower solid"",
	""fas fa-shopping-cart"": ""&#xf07a; shopping-cart solid"",
	""fas fa-shopping-basket"": ""&#xf291; Shopping Basket solid"",
	""fas fa-shopping-bag"": ""&#xf290; Shopping Bag solid"",
	""fab fa-shirtsinbulk"": ""&#xf214; Shirts in Bulk brands"",
	""fas fa-shipping-fast"": ""&#xf48b; Shipping Fast solid"",
	""fas fa-ship"": ""&#xf21a; Ship solid"",
	""fas fa-shield-alt"": ""&#xf3ed; Alternate Shield solid"",
	""fas fa-shekel-sign"": ""&#xf20b; Shekel Sign solid"",
	""fas fa-share-square"": ""&#xf14d; Share Square solid"",
	""far fa-share-square"": ""&#xf14d; Share Square regular"",
	""fas fa-share-alt-square"": ""&#xf1e1; Alternate Share Square solid"",
	""fas fa-share-alt"": ""&#xf1e0; Alternate Share solid"",
	""fas fa-share"": ""&#xf064; Share solid"",
	""fab fa-servicestack"": ""&#xf3ec; Servicestack brands"",
	""fas fa-server"": ""&#xf233; Server solid"",
	""fab fa-sellsy"": ""&#xf213; Sellsy brands"",
	""fab fa-sellcast"": ""&#xf2da; Sellcast brands"",
	""fas fa-seedling"": ""&#xf4d8; Seedling solid"",
	""fab fa-searchengin"": ""&#xf3eb; Searchengin brands"",
	""fas fa-search-plus"": ""&#xf00e; Search Plus solid"",
	""fas fa-search-minus"": ""&#xf010; Search Minus solid"",
	""fas fa-search"": ""&#xf002; Search solid"",
	""fab fa-scribd"": ""&#xf28a; Scribd brands"",
	""fab fa-schlix"": ""&#xf3ea; SCHLIX brands"",
	""fas fa-save"": ""&#xf0c7; Save solid"",
	""far fa-save"": ""&#xf0c7; Save regular"",
	""fab fa-sass"": ""&#xf41e; Sass brands"",
	""fab fa-safari"": ""&#xf267; Safari brands"",
	""fas fa-rupee-sign"": ""&#xf156; Indian Rupee Sign solid"",
	""fas fa-ruble-sign"": ""&#xf158; Ruble Sign solid"",
	""fas fa-rss-square"": ""&#xf143; RSS Square solid"",
	""fas fa-rss"": ""&#xf09e; rss solid"",
	""fab fa-rockrms"": ""&#xf3e9; Rockrms brands"",
	""fab fa-rocketchat"": ""&#xf3e8; Rocket.Chat brands"",
	""fas fa-rocket"": ""&#xf135; rocket solid"",
	""fas fa-road"": ""&#xf018; road solid"",
	""fas fa-ribbon"": ""&#xf4d6; Ribbon solid"",
	""fas fa-retweet"": ""&#xf079; Retweet solid"",
	""fab fa-resolving"": ""&#xf3e7; Resolving brands"",
	""fab fa-researchgate"": ""&#xf4f8; Researchgate brands"",
	""fab fa-replyd"": ""&#xf3e6; replyd brands"",
	""fas fa-reply-all"": ""&#xf122; reply-all solid"",
	""fas fa-reply"": ""&#xf3e5; Reply solid"",
	""fab fa-renren"": ""&#xf18b; Renren brands"",
	""fab fa-rendact"": ""&#xf3e4; Rendact brands"",
	""fas fa-registered"": ""&#xf25d; Registered Trademark solid"",
	""far fa-registered"": ""&#xf25d; Registered Trademark regular"",
	""fas fa-redo-alt"": ""&#xf2f9; Alternate Redo solid"",
	""fas fa-redo"": ""&#xf01e; Redo solid"",
	""fab fa-reddit-square"": ""&#xf1a2; reddit Square brands"",
	""fab fa-reddit-alien"": ""&#xf281; reddit Alien brands"",
	""fab fa-reddit"": ""&#xf1a1; reddit Logo brands"",
	""fab fa-red-river"": ""&#xf3e3; red river brands"",
	""fas fa-recycle"": ""&#xf1b8; Recycle solid"",
	""fab fa-rebel"": ""&#xf1d0; Rebel Alliance brands"",
	""fab fa-readme"": ""&#xf4d5; ReadMe brands"",
	""fab fa-react"": ""&#xf41b; React brands"",
	""fab fa-ravelry"": ""&#xf2d9; Ravelry brands"",
	""fas fa-random"": ""&#xf074; random solid"",
	""fab fa-r-project"": ""&#xf4f7; R Project brands"",
	""fas fa-quote-right"": ""&#xf10e; quote-right solid"",
	""fas fa-quote-left"": ""&#xf10d; quote-left solid"",
	""fab fa-quora"": ""&#xf2c4; Quora brands"",
	""fab fa-quinscape"": ""&#xf459; QuinScape brands"",
	""fas fa-quidditch"": ""&#xf458; Quidditch solid"",
	""fas fa-question-circle"": ""&#xf059; Question Circle solid"",
	""far fa-question-circle"": ""&#xf059; Question Circle regular"",
	""fas fa-question"": ""&#xf128; Question solid"",
	""fas fa-qrcode"": ""&#xf029; qrcode solid"",
	""fab fa-qq"": ""&#xf1d6; QQ brands"",
	""fab fa-python"": ""&#xf3e2; Python brands"",
	""fas fa-puzzle-piece"": ""&#xf12e; Puzzle Piece solid"",
	""fab fa-pushed"": ""&#xf3e1; Pushed brands"",
	""fab fa-product-hunt"": ""&#xf288; Product Hunt brands"",
	""fas fa-procedures"": ""&#xf487; Procedures solid"",
	""fas fa-print"": ""&#xf02f; print solid"",
	""fas fa-prescription-bottle-alt"": ""&#xf486; Alternate Prescription Bottle solid"",
	""fas fa-prescription-bottle"": ""&#xf485; Prescription Bottle solid"",
	""fas fa-power-off"": ""&#xf011; Power Off solid"",
	""fas fa-pound-sign"": ""&#xf154; Pound Sign solid"",
	""fas fa-portrait"": ""&#xf3e0; Portrait solid"",
	""fas fa-poo"": ""&#xf2fe; Poo solid"",
	""fas fa-podcast"": ""&#xf2ce; Podcast solid"",
	""fas fa-plus-square"": ""&#xf0fe; Plus Square solid"",
	""far fa-plus-square"": ""&#xf0fe; Plus Square regular"",
	""fas fa-plus-circle"": ""&#xf055; Plus Circle solid"",
	""fas fa-plus"": ""&#xf067; plus solid"",
	""fas fa-plug"": ""&#xf1e6; Plug solid"",
	""fab fa-playstation"": ""&#xf3df; PlayStation brands"",
	""fas fa-play-circle"": ""&#xf144; Play Circle solid"",
	""far fa-play-circle"": ""&#xf144; Play Circle regular"",
	""fas fa-play"": ""&#xf04b; play solid"",
	""fas fa-plane"": ""&#xf072; plane solid"",
	""fab fa-pinterest-square"": ""&#xf0d3; Pinterest Square brands"",
	""fab fa-pinterest-p"": ""&#xf231; Pinterest P brands"",
	""fab fa-pinterest"": ""&#xf0d2; Pinterest brands"",
	""fas fa-pills"": ""&#xf484; Pills solid"",
	""fas fa-piggy-bank"": ""&#xf4d3; Piggy Bank solid"",
	""fab fa-pied-piper-pp"": ""&#xf1a7; Pied Piper PP Logo (Old) brands"",
	""fab fa-pied-piper-hat"": ""&#xf4e5; Pied Piper-hat brands"",
	""fab fa-pied-piper-alt"": ""&#xf1a8; Alternate Pied Piper Logo brands"",
	""fab fa-pied-piper"": ""&#xf2ae; Pied Piper Logo brands"",
	""fab fa-php"": ""&#xf457; PHP brands"",
	""fas fa-phone-volume"": ""&#xf2a0; Phone Volume solid"",
	""fas fa-phone-square"": ""&#xf098; Phone Square solid"",
	""fas fa-phone-slash"": ""&#xf3dd; Phone Slash solid"",
	""fas fa-phone"": ""&#xf095; Phone solid"",
	""fab fa-phoenix-squadron"": ""&#xf511; Phoenix Squadron brands"",
	""fab fa-phoenix-framework"": ""&#xf3dc; Phoenix Framework brands"",
	""fab fa-phabricator"": ""&#xf3db; Phabricator brands"",
	""fab fa-periscope"": ""&#xf3da; Periscope brands"",
	""fas fa-percent"": ""&#xf295; Percent solid"",
	""fas fa-people-carry"": ""&#xf4ce; People Carry solid"",
	""fas fa-pencil-alt"": ""&#xf303; Alternate Pencil solid"",
	""fas fa-pen-square"": ""&#xf14b; Pen Square solid"",
	""fab fa-paypal"": ""&#xf1ed; Paypal brands"",
	""fas fa-paw"": ""&#xf1b0; Paw solid"",
	""fas fa-pause-circle"": ""&#xf28b; Pause Circle solid"",
	""far fa-pause-circle"": ""&#xf28b; Pause Circle regular"",
	""fas fa-pause"": ""&#xf04c; pause solid"",
	""fab fa-patreon"": ""&#xf3d9; Patreon brands"",
	""fas fa-paste"": ""&#xf0ea; Paste solid"",
	""fas fa-paragraph"": ""&#xf1dd; paragraph solid"",
	""fas fa-parachute-box"": ""&#xf4cd; Parachute Box solid"",
	""fas fa-paperclip"": ""&#xf0c6; Paperclip solid"",
	""fas fa-paper-plane"": ""&#xf1d8; Paper Plane solid"",
	""far fa-paper-plane"": ""&#xf1d8; Paper Plane regular"",
	""fas fa-pallet"": ""&#xf482; Pallet solid"",
	""fab fa-palfed"": ""&#xf3d8; Palfed brands"",
	""fas fa-paint-brush"": ""&#xf1fc; Paint Brush solid"",
	""fab fa-pagelines"": ""&#xf18c; Pagelines brands"",
	""fab fa-page4"": ""&#xf3d7; page4 Corporation brands"",
	""fas fa-outdent"": ""&#xf03b; Outdent solid"",
	""fab fa-osi"": ""&#xf41a; Open Source Initiative brands"",
	""fab fa-optin-monster"": ""&#xf23c; Optin Monster brands"",
	""fab fa-opera"": ""&#xf26a; Opera brands"",
	""fab fa-openid"": ""&#xf19b; OpenID brands"",
	""fab fa-opencart"": ""&#xf23d; OpenCart brands"",
	""fab fa-old-republic"": ""&#xf510; Old Republic brands"",
	""fab fa-odnoklassniki-square"": ""&#xf264; Odnoklassniki Square brands"",
	""fab fa-odnoklassniki"": ""&#xf263; Odnoklassniki brands"",
	""fas fa-object-ungroup"": ""&#xf248; Object Ungroup solid"",
	""far fa-object-ungroup"": ""&#xf248; Object Ungroup regular"",
	""fas fa-object-group"": ""&#xf247; Object Group solid"",
	""far fa-object-group"": ""&#xf247; Object Group regular"",
	""fab fa-nutritionix"": ""&#xf3d6; Nutritionix brands"",
	""fab fa-ns8"": ""&#xf3d5; NS8 brands"",
	""fab fa-npm"": ""&#xf3d4; npm brands"",
	""fas fa-notes-medical"": ""&#xf481; Medical Notes solid"",
	""fab fa-node-js"": ""&#xf3d3; Node.js JS brands"",
	""fab fa-node"": ""&#xf419; Node.js brands"",
	""fab fa-nintendo-switch"": ""&#xf418; Nintendo Switch brands"",
	""fas fa-newspaper"": ""&#xf1ea; Newspaper solid"",
	""far fa-newspaper"": ""&#xf1ea; Newspaper regular"",
	""fas fa-neuter"": ""&#xf22c; Neuter solid"",
	""fab fa-napster"": ""&#xf3d2; Napster brands"",
	""fas fa-music"": ""&#xf001; Music solid"",
	""fas fa-mouse-pointer"": ""&#xf245; Mouse Pointer solid"",
	""fas fa-motorcycle"": ""&#xf21c; Motorcycle solid"",
	""fas fa-moon"": ""&#xf186; Moon solid"",
	""far fa-moon"": ""&#xf186; Moon regular"",
	""fas fa-money-bill-alt"": ""&#xf3d1; Money Bill Alternate solid"",
	""far fa-money-bill-alt"": ""&#xf3d1; Money Bill Alternate regular"",
	""fab fa-monero"": ""&#xf3d0; Monero brands"",
	""fab fa-modx"": ""&#xf285; MODX brands"",
	""fas fa-mobile-alt"": ""&#xf3cd; Alternate Mobile solid"",
	""fas fa-mobile"": ""&#xf10b; Mobile Phone solid"",
	""fab fa-mizuni"": ""&#xf3cc; Mizuni brands"",
	""fab fa-mixcloud"": ""&#xf289; Mixcloud brands"",
	""fab fa-mix"": ""&#xf3cb; Mix brands"",
	""fas fa-minus-square"": ""&#xf146; Minus Square solid"",
	""far fa-minus-square"": ""&#xf146; Minus Square regular"",
	""fas fa-minus-circle"": ""&#xf056; Minus Circle solid"",
	""fas fa-minus"": ""&#xf068; minus solid"",
	""fab fa-microsoft"": ""&#xf3ca; Microsoft brands"",
	""fas fa-microphone-slash"": ""&#xf131; Microphone Slash solid"",
	""fas fa-microphone"": ""&#xf130; microphone solid"",
	""fas fa-microchip"": ""&#xf2db; Microchip solid"",
	""fas fa-mercury"": ""&#xf223; Mercury solid"",
	""fas fa-meh"": ""&#xf11a; Meh solid"",
	""far fa-meh"": ""&#xf11a; Meh regular"",
	""fab fa-meetup"": ""&#xf2e0; Meetup brands"",
	""fab fa-medrt"": ""&#xf3c8; MRT brands"",
	""fas fa-medkit"": ""&#xf0fa; medkit solid"",
	""fab fa-medium-m"": ""&#xf3c7; Medium M brands"",
	""fab fa-medium"": ""&#xf23a; Medium brands"",
	""fab fa-medapps"": ""&#xf3c6; MedApps brands"",
	""fab fa-maxcdn"": ""&#xf136; MaxCDN brands"",
	""fab fa-mastodon"": ""&#xf4f6; Mastodon brands"",
	""fas fa-mars-stroke-v"": ""&#xf22a; Mars Stroke Vertical solid"",
	""fas fa-mars-stroke-h"": ""&#xf22b; Mars Stroke Horizontal solid"",
	""fas fa-mars-stroke"": ""&#xf229; Mars Stroke solid"",
	""fas fa-mars-double"": ""&#xf227; Mars Double solid"",
	""fas fa-mars"": ""&#xf222; Mars solid"",
	""fas fa-map-signs"": ""&#xf277; Map Signs solid"",
	""fas fa-map-pin"": ""&#xf276; Map Pin solid"",
	""fas fa-map-marker-alt"": ""&#xf3c5; Map Marker Alternate solid"",
	""fas fa-map-marker"": ""&#xf041; map-marker solid"",
	""fas fa-map"": ""&#xf279; Map solid"",
	""far fa-map"": ""&#xf279; Map regular"",
	""fab fa-mandalorian"": ""&#xf50f; Mandalorian brands"",
	""fas fa-male"": ""&#xf183; Male solid"",
	""fas fa-magnet"": ""&#xf076; magnet solid"",
	""fas fa-magic"": ""&#xf0d0; magic solid"",
	""fab fa-magento"": ""&#xf3c4; Magento brands"",
	""fab fa-lyft"": ""&#xf3c3; lyft brands"",
	""fas fa-low-vision"": ""&#xf2a8; Low Vision solid"",
	""fas fa-long-arrow-alt-up"": ""&#xf30c; Alternate Long Arrow Up solid"",
	""fas fa-long-arrow-alt-right"": ""&#xf30b; Alternate Long Arrow Right solid"",
	""fas fa-long-arrow-alt-left"": ""&#xf30a; Alternate Long Arrow Left solid"",
	""fas fa-long-arrow-alt-down"": ""&#xf309; Alternate Long Arrow Down solid"",
	""fas fa-lock-open"": ""&#xf3c1; Lock Open solid"",
	""fas fa-lock"": ""&#xf023; lock solid"",
	""fas fa-location-arrow"": ""&#xf124; location-arrow solid"",
	""fas fa-list-ul"": ""&#xf0ca; list-ul solid"",
	""fas fa-list-ol"": ""&#xf0cb; list-ol solid"",
	""fas fa-list-alt"": ""&#xf022; List Alternate solid"",
	""far fa-list-alt"": ""&#xf022; List Alternate regular"",
	""fas fa-list"": ""&#xf03a; List solid"",
	""fas fa-lira-sign"": ""&#xf195; Turkish Lira Sign solid"",
	""fab fa-linux"": ""&#xf17c; Linux brands"",
	""fab fa-linode"": ""&#xf2b8; Linode brands"",
	""fab fa-linkedin-in"": ""&#xf0e1; LinkedIn In brands"",
	""fab fa-linkedin"": ""&#xf08c; LinkedIn brands"",
	""fas fa-link"": ""&#xf0c1; Link solid"",
	""fab fa-line"": ""&#xf3c0; Line brands"",
	""fas fa-lightbulb"": ""&#xf0eb; Lightbulb solid"",
	""far fa-lightbulb"": ""&#xf0eb; Lightbulb regular"",
	""fas fa-life-ring"": ""&#xf1cd; Life Ring solid"",
	""far fa-life-ring"": ""&#xf1cd; Life Ring regular"",
	""fas fa-level-up-alt"": ""&#xf3bf; Level Up Alternate solid"",
	""fas fa-level-down-alt"": ""&#xf3be; Level Down Alternate solid"",
	""fab fa-less"": ""&#xf41d; Less brands"",
	""fas fa-lemon"": ""&#xf094; Lemon solid"",
	""far fa-lemon"": ""&#xf094; Lemon regular"",
	""fab fa-leanpub"": ""&#xf212; Leanpub brands"",
	""fas fa-leaf"": ""&#xf06c; leaf solid"",
	""fab fa-lastfm-square"": ""&#xf203; last.fm Square brands"",
	""fab fa-lastfm"": ""&#xf202; last.fm brands"",
	""fab fa-laravel"": ""&#xf3bd; Laravel brands"",
	""fas fa-laptop"": ""&#xf109; Laptop solid"",
	""fas fa-language"": ""&#xf1ab; Language solid"",
	""fab fa-korvue"": ""&#xf42f; KORVUE brands"",
	""fab fa-kickstarter-k"": ""&#xf3bc; Kickstarter K brands"",
	""fab fa-kickstarter"": ""&#xf3bb; Kickstarter brands"",
	""fab fa-keycdn"": ""&#xf3ba; KeyCDN brands"",
	""fas fa-keyboard"": ""&#xf11c; Keyboard solid"",
	""far fa-keyboard"": ""&#xf11c; Keyboard regular"",
	""fab fa-keybase"": ""&#xf4f5; Keybase brands"",
	""fas fa-key"": ""&#xf084; key solid"",
	""fab fa-jsfiddle"": ""&#xf1cc; jsFiddle brands"",
	""fab fa-js-square"": ""&#xf3b9; JavaScript (JS) Square brands"",
	""fab fa-js"": ""&#xf3b8; JavaScript (JS) brands"",
	""fab fa-joomla"": ""&#xf1aa; Joomla Logo brands"",
	""fab fa-joget"": ""&#xf3b7; Joget brands"",
	""fab fa-jenkins"": ""&#xf3b6; Jenkis brands"",
	""fab fa-jedi-order"": ""&#xf50e; Jedi Order brands"",
	""fab fa-java"": ""&#xf4e4; Java brands"",
	""fab fa-itunes-note"": ""&#xf3b5; Itunes Note brands"",
	""fab fa-itunes"": ""&#xf3b4; iTunes brands"",
	""fas fa-italic"": ""&#xf033; italic solid"",
	""fab fa-ioxhost"": ""&#xf208; ioxhost brands"",
	""fab fa-internet-explorer"": ""&#xf26b; Internet-explorer brands"",
	""fab fa-instagram"": ""&#xf16d; Instagram brands"",
	""fas fa-info-circle"": ""&#xf05a; Info Circle solid"",
	""fas fa-info"": ""&#xf129; Info solid"",
	""fas fa-industry"": ""&#xf275; Industry solid"",
	""fas fa-indent"": ""&#xf03c; Indent solid"",
	""fas fa-inbox"": ""&#xf01c; inbox solid"",
	""fab fa-imdb"": ""&#xf2d8; IMDB brands"",
	""fas fa-images"": ""&#xf302; Images solid"",
	""far fa-images"": ""&#xf302; Images regular"",
	""fas fa-image"": ""&#xf03e; Image solid"",
	""far fa-image"": ""&#xf03e; Image regular"",
	""fas fa-id-card-alt"": ""&#xf47f; Identification Card Alternate solid"",
	""fas fa-id-card"": ""&#xf2c2; Identification Card solid"",
	""far fa-id-card"": ""&#xf2c2; Identification Card regular"",
	""fas fa-id-badge"": ""&#xf2c1; Identification Badge solid"",
	""far fa-id-badge"": ""&#xf2c1; Identification Badge regular"",
	""fas fa-i-cursor"": ""&#xf246; I Beam Cursor solid"",
	""fab fa-hubspot"": ""&#xf3b2; HubSpot brands"",
	""fab fa-html5"": ""&#xf13b; HTML 5 Logo brands"",
	""fab fa-houzz"": ""&#xf27c; Houzz brands"",
	""fas fa-hourglass-start"": ""&#xf251; Hourglass Start solid"",
	""fas fa-hourglass-half"": ""&#xf252; Hourglass Half solid"",
	""fas fa-hourglass-end"": ""&#xf253; Hourglass End solid"",
	""fas fa-hourglass"": ""&#xf254; Hourglass solid"",
	""far fa-hourglass"": ""&#xf254; Hourglass regular"",
	""fab fa-hotjar"": ""&#xf3b1; Hotjar brands"",
	""fas fa-hospital-symbol"": ""&#xf47e; Hospital Symbol solid"",
	""fas fa-hospital-alt"": ""&#xf47d; Hospital Alternate solid"",
	""fas fa-hospital"": ""&#xf0f8; hospital solid"",
	""far fa-hospital"": ""&#xf0f8; hospital regular"",
	""fab fa-hooli"": ""&#xf427; Hooli brands"",
	""fas fa-home"": ""&#xf015; home solid"",
	""fas fa-hockey-puck"": ""&#xf453; Hockey Puck solid"",
	""fas fa-history"": ""&#xf1da; History solid"",
	""fab fa-hire-a-helper"": ""&#xf3b0; HireAHelper brands"",
	""fab fa-hips"": ""&#xf452; Hips brands"",
	""fas fa-heartbeat"": ""&#xf21e; Heartbeat solid"",
	""fas fa-heart"": ""&#xf004; Heart solid"",
	""far fa-heart"": ""&#xf004; Heart regular"",
	""fas fa-headphones"": ""&#xf025; headphones solid"",
	""fas fa-heading"": ""&#xf1dc; heading solid"",
	""fas fa-hdd"": ""&#xf0a0; HDD solid"",
	""far fa-hdd"": ""&#xf0a0; HDD regular"",
	""fas fa-hashtag"": ""&#xf292; Hashtag solid"",
	""fas fa-handshake"": ""&#xf2b5; Handshake solid"",
	""far fa-handshake"": ""&#xf2b5; Handshake regular"",
	""fas fa-hands-helping"": ""&#xf4c4; Helping Hands solid"",
	""fas fa-hands"": ""&#xf4c2; Hands solid"",
	""fas fa-hand-spock"": ""&#xf259; Spock (Hand) solid"",
	""far fa-hand-spock"": ""&#xf259; Spock (Hand) regular"",
	""fas fa-hand-scissors"": ""&#xf257; Scissors (Hand) solid"",
	""far fa-hand-scissors"": ""&#xf257; Scissors (Hand) regular"",
	""fas fa-hand-rock"": ""&#xf255; Rock (Hand) solid"",
	""far fa-hand-rock"": ""&#xf255; Rock (Hand) regular"",
	""fas fa-hand-pointer"": ""&#xf25a; Pointer (Hand) solid"",
	""far fa-hand-pointer"": ""&#xf25a; Pointer (Hand) regular"",
	""fas fa-hand-point-up"": ""&#xf0a6; Hand Pointing Up solid"",
	""far fa-hand-point-up"": ""&#xf0a6; Hand Pointing Up regular"",
	""fas fa-hand-point-right"": ""&#xf0a4; Hand Pointing Right solid"",
	""far fa-hand-point-right"": ""&#xf0a4; Hand Pointing Right regular"",
	""fas fa-hand-point-left"": ""&#xf0a5; Hand Pointing Left solid"",
	""far fa-hand-point-left"": ""&#xf0a5; Hand Pointing Left regular"",
	""fas fa-hand-point-down"": ""&#xf0a7; Hand Pointing Down solid"",
	""far fa-hand-point-down"": ""&#xf0a7; Hand Pointing Down regular"",
	""fas fa-hand-peace"": ""&#xf25b; Peace (Hand) solid"",
	""far fa-hand-peace"": ""&#xf25b; Peace (Hand) regular"",
	""fas fa-hand-paper"": ""&#xf256; Paper (Hand) solid"",
	""far fa-hand-paper"": ""&#xf256; Paper (Hand) regular"",
	""fas fa-hand-lizard"": ""&#xf258; Lizard (Hand) solid"",
	""far fa-hand-lizard"": ""&#xf258; Lizard (Hand) regular"",
	""fas fa-hand-holding-usd"": ""&#xf4c0; Hand Holding US Dollar solid"",
	""fas fa-hand-holding-heart"": ""&#xf4be; Hand Holding Heart solid"",
	""fas fa-hand-holding"": ""&#xf4bd; Hand Holding solid"",
	""fab fa-hacker-news-square"": ""&#xf3af; Hacker News Square brands"",
	""fab fa-hacker-news"": ""&#xf1d4; Hacker News brands"",
	""fas fa-h-square"": ""&#xf0fd; H Square solid"",
	""fab fa-gulp"": ""&#xf3ae; Gulp brands"",
	""fab fa-grunt"": ""&#xf3ad; Grunt brands"",
	""fab fa-gripfire"": ""&#xf3ac; Gripfire, Inc. brands"",
	""fab fa-grav"": ""&#xf2d6; Grav brands"",
	""fab fa-gratipay"": ""&#xf184; Gratipay (Gittip) brands"",
	""fas fa-graduation-cap"": ""&#xf19d; Graduation Cap solid"",
	""fab fa-google-wallet"": ""&#xf1ee; Google Wallet brands"",
	""fab fa-google-plus-square"": ""&#xf0d4; Google Plus Square brands"",
	""fab fa-google-plus-g"": ""&#xf0d5; Google Plus G brands"",
	""fab fa-google-plus"": ""&#xf2b3; Google Plus brands"",
	""fab fa-google-play"": ""&#xf3ab; Google Play brands"",
	""fab fa-google-drive"": ""&#xf3aa; Google Drive brands"",
	""fab fa-google"": ""&#xf1a0; Google Logo brands"",
	""fab fa-goodreads-g"": ""&#xf3a9; Goodreads G brands"",
	""fab fa-goodreads"": ""&#xf3a8; Goodreads brands"",
	""fas fa-golf-ball"": ""&#xf450; Golf Ball solid"",
	""fab fa-gofore"": ""&#xf3a7; Gofore brands"",
	""fas fa-globe"": ""&#xf0ac; Globe solid"",
	""fab fa-glide-g"": ""&#xf2a6; Glide G brands"",
	""fab fa-glide"": ""&#xf2a5; Glide brands"",
	""fas fa-glass-martini"": ""&#xf000; Martini Glass solid"",
	""fab fa-gitter"": ""&#xf426; Gitter brands"",
	""fab fa-gitlab"": ""&#xf296; GitLab brands"",
	""fab fa-gitkraken"": ""&#xf3a6; GitKraken brands"",
	""fab fa-github-square"": ""&#xf092; GitHub Square brands"",
	""fab fa-github-alt"": ""&#xf113; Alternate GitHub brands"",
	""fab fa-github"": ""&#xf09b; GitHub brands"",
	""fab fa-git-square"": ""&#xf1d2; Git Square brands"",
	""fab fa-git"": ""&#xf1d3; Git brands"",
	""fas fa-gift"": ""&#xf06b; gift solid"",
	""fab fa-gg-circle"": ""&#xf261; GG Currency Circle brands"",
	""fab fa-gg"": ""&#xf260; GG Currency brands"",
	""fab fa-get-pocket"": ""&#xf265; Get Pocket brands"",
	""fas fa-genderless"": ""&#xf22d; Genderless solid"",
	""fas fa-gem"": ""&#xf3a5; Gem solid"",
	""far fa-gem"": ""&#xf3a5; Gem regular"",
	""fas fa-gavel"": ""&#xf0e3; Gavel solid"",
	""fas fa-gamepad"": ""&#xf11b; Gamepad solid"",
	""fab fa-galactic-senate"": ""&#xf50d; Galactic Senate brands"",
	""fab fa-galactic-republic"": ""&#xf50c; Galactic Republic brands"",
	""fas fa-futbol"": ""&#xf1e3; Futbol solid"",
	""far fa-futbol"": ""&#xf1e3; Futbol regular"",
	""fab fa-fulcrum"": ""&#xf50b; Fulcrum brands"",
	""fas fa-frown"": ""&#xf119; Frown solid"",
	""far fa-frown"": ""&#xf119; Frown regular"",
	""fab fa-freebsd"": ""&#xf3a4; FreeBSD brands"",
	""fab fa-free-code-camp"": ""&#xf2c5; Free Code Camp brands"",
	""fab fa-foursquare"": ""&#xf180; Foursquare brands"",
	""fas fa-forward"": ""&#xf04e; forward solid"",
	""fab fa-forumbee"": ""&#xf211; Forumbee brands"",
	""fab fa-fort-awesome-alt"": ""&#xf3a3; Alternate Fort Awesome brands"",
	""fab fa-fort-awesome"": ""&#xf286; Fort Awesome brands"",
	""fas fa-football-ball"": ""&#xf44e; Football Ball solid"",
	""fab fa-fonticons-fi"": ""&#xf3a2; Fonticons Fi brands"",
	""fab fa-fonticons"": ""&#xf280; Fonticons brands"",
	""far fa-font-awesome-logo-full"": ""&#xf4e6; Font Awesome Full Logo regular"",
	""fas fa-font-awesome-logo-full"": ""&#xf4e6; Font Awesome Full Logo solid"",
	""fab fa-font-awesome-logo-full"": ""&#xf4e6; Font Awesome Full Logo brands"",
	""fab fa-font-awesome-flag"": ""&#xf425; Font Awesome Flag brands"",
	""fab fa-font-awesome-alt"": ""&#xf35c; Alternate Font Awesome brands"",
	""fab fa-font-awesome"": ""&#xf2b4; Font Awesome brands"",
	""fas fa-font"": ""&#xf031; font solid"",
	""fas fa-folder-open"": ""&#xf07c; Folder Open solid"",
	""far fa-folder-open"": ""&#xf07c; Folder Open regular"",
	""fas fa-folder"": ""&#xf07b; Folder solid"",
	""far fa-folder"": ""&#xf07b; Folder regular"",
	""fab fa-fly"": ""&#xf417; Fly brands"",
	""fab fa-flipboard"": ""&#xf44d; Flipboard brands"",
	""fab fa-flickr"": ""&#xf16e; Flickr brands"",
	""fas fa-flask"": ""&#xf0c3; Flask solid"",
	""fas fa-flag-checkered"": ""&#xf11e; flag-checkered solid"",
	""fas fa-flag"": ""&#xf024; flag solid"",
	""far fa-flag"": ""&#xf024; flag regular"",
	""fab fa-firstdraft"": ""&#xf3a1; firstdraft brands"",
	""fab fa-first-order-alt"": ""&#xf50a; Alternate First Order brands"",
	""fab fa-first-order"": ""&#xf2b0; First Order brands"",
	""fas fa-first-aid"": ""&#xf479; First Aid solid"",
	""fab fa-firefox"": ""&#xf269; Firefox brands"",
	""fas fa-fire-extinguisher"": ""&#xf134; fire-extinguisher solid"",
	""fas fa-fire"": ""&#xf06d; fire solid"",
	""fas fa-filter"": ""&#xf0b0; Filter solid"",
	""fas fa-film"": ""&#xf008; Film solid"",
	""fas fa-file-word"": ""&#xf1c2; Word File solid"",
	""far fa-file-word"": ""&#xf1c2; Word File regular"",
	""fas fa-file-video"": ""&#xf1c8; Video File solid"",
	""far fa-file-video"": ""&#xf1c8; Video File regular"",
	""fas fa-file-powerpoint"": ""&#xf1c4; Powerpoint File solid"",
	""far fa-file-powerpoint"": ""&#xf1c4; Powerpoint File regular"",
	""fas fa-file-pdf"": ""&#xf1c1; PDF File solid"",
	""far fa-file-pdf"": ""&#xf1c1; PDF File regular"",
	""fas fa-file-medical-alt"": ""&#xf478; Medical File Alternate solid"",
	""fas fa-file-medical"": ""&#xf477; Medical File solid"",
	""fas fa-file-image"": ""&#xf1c5; Image File solid"",
	""far fa-file-image"": ""&#xf1c5; Image File regular"",
	""fas fa-file-excel"": ""&#xf1c3; Excel File solid"",
	""far fa-file-excel"": ""&#xf1c3; Excel File regular"",
	""fas fa-file-code"": ""&#xf1c9; Code File solid"",
	""far fa-file-code"": ""&#xf1c9; Code File regular"",
	""fas fa-file-audio"": ""&#xf1c7; Audio File solid"",
	""far fa-file-audio"": ""&#xf1c7; Audio File regular"",
	""fas fa-file-archive"": ""&#xf1c6; Archive File solid"",
	""far fa-file-archive"": ""&#xf1c6; Archive File regular"",
	""fas fa-file-alt"": ""&#xf15c; Alternate File solid"",
	""far fa-file-alt"": ""&#xf15c; Alternate File regular"",
	""fas fa-file"": ""&#xf15b; File solid"",
	""far fa-file"": ""&#xf15b; File regular"",
	""fas fa-fighter-jet"": ""&#xf0fb; fighter-jet solid"",
	""fas fa-female"": ""&#xf182; Female solid"",
	""fas fa-fax"": ""&#xf1ac; Fax solid"",
	""fas fa-fast-forward"": ""&#xf050; fast-forward solid"",
	""fas fa-fast-backward"": ""&#xf049; fast-backward solid"",
	""fab fa-facebook-square"": ""&#xf082; Facebook Square brands"",
	""fab fa-facebook-messenger"": ""&#xf39f; Facebook Messenger brands"",
	""fab fa-facebook-f"": ""&#xf39e; Facebook F brands"",
	""fab fa-facebook"": ""&#xf09a; Facebook brands"",
	""fas fa-eye-slash"": ""&#xf070; Eye Slash solid"",
	""far fa-eye-slash"": ""&#xf070; Eye Slash regular"",
	""fas fa-eye-dropper"": ""&#xf1fb; Eye Dropper solid"",
	""fas fa-eye"": ""&#xf06e; Eye solid"",
	""far fa-eye"": ""&#xf06e; Eye regular"",
	""fas fa-external-link-square-alt"": ""&#xf360; Alternate External Link Square solid"",
	""fas fa-external-link-alt"": ""&#xf35d; Alternate External Link solid"",
	""fab fa-expeditedssl"": ""&#xf23e; ExpeditedSSL brands"",
	""fas fa-expand-arrows-alt"": ""&#xf31e; Alternate Expand Arrows solid"",
	""fas fa-expand"": ""&#xf065; Expand solid"",
	""fas fa-exclamation-triangle"": ""&#xf071; Exclamation Triangle solid"",
	""fas fa-exclamation-circle"": ""&#xf06a; Exclamation Circle solid"",
	""fas fa-exclamation"": ""&#xf12a; exclamation solid"",
	""fas fa-exchange-alt"": ""&#xf362; Alternate Exchange solid"",
	""fas fa-euro-sign"": ""&#xf153; Euro Sign solid"",
	""fab fa-etsy"": ""&#xf2d7; Etsy brands"",
	""fab fa-ethereum"": ""&#xf42e; Ethereum brands"",
	""fab fa-erlang"": ""&#xf39d; Erlang brands"",
	""fas fa-eraser"": ""&#xf12d; eraser solid"",
	""fab fa-envira"": ""&#xf299; Envira Gallery brands"",
	""fas fa-envelope-square"": ""&#xf199; Envelope Square solid"",
	""fas fa-envelope-open"": ""&#xf2b6; Envelope Open solid"",
	""far fa-envelope-open"": ""&#xf2b6; Envelope Open regular"",
	""fas fa-envelope"": ""&#xf0e0; Envelope solid"",
	""far fa-envelope"": ""&#xf0e0; Envelope regular"",
	""fab fa-empire"": ""&#xf1d1; Galactic Empire brands"",
	""fab fa-ember"": ""&#xf423; Ember brands"",
	""fas fa-ellipsis-v"": ""&#xf142; Vertical Ellipsis solid"",
	""fas fa-ellipsis-h"": ""&#xf141; Horizontal Ellipsis solid"",
	""fab fa-elementor"": ""&#xf430; Elementor brands"",
	""fas fa-eject"": ""&#xf052; eject solid"",
	""fas fa-edit"": ""&#xf044; Edit solid"",
	""far fa-edit"": ""&#xf044; Edit regular"",
	""fab fa-edge"": ""&#xf282; Edge Browser brands"",
	""fab fa-ebay"": ""&#xf4f4; eBay brands"",
	""fab fa-earlybirds"": ""&#xf39a; Earlybirds brands"",
	""fab fa-dyalog"": ""&#xf399; Dyalog brands"",
	""fab fa-drupal"": ""&#xf1a9; Drupal Logo brands"",
	""fab fa-dropbox"": ""&#xf16b; Dropbox brands"",
	""fab fa-dribbble-square"": ""&#xf397; Dribbble Square brands"",
	""fab fa-dribbble"": ""&#xf17d; Dribbble brands"",
	""fab fa-draft2digital"": ""&#xf396; Draft2digital brands"",
	""fas fa-download"": ""&#xf019; Download solid"",
	""fas fa-dove"": ""&#xf4ba; Dove solid"",
	""fas fa-dot-circle"": ""&#xf192; Dot Circle solid"",
	""far fa-dot-circle"": ""&#xf192; Dot Circle regular"",
	""fas fa-donate"": ""&#xf4b9; Donate solid"",
	""fas fa-dolly-flatbed"": ""&#xf474; Dolly Flatbed solid"",
	""fas fa-dolly"": ""&#xf472; Dolly solid"",
	""fas fa-dollar-sign"": ""&#xf155; Dollar Sign solid"",
	""fab fa-docker"": ""&#xf395; Docker brands"",
	""fab fa-dochub"": ""&#xf394; DocHub brands"",
	""fas fa-dna"": ""&#xf471; DNA solid"",
	""fab fa-discourse"": ""&#xf393; Discourse brands"",
	""fab fa-discord"": ""&#xf392; Discord brands"",
	""fab fa-digital-ocean"": ""&#xf391; Digital Ocean brands"",
	""fab fa-digg"": ""&#xf1a6; Digg Logo brands"",
	""fas fa-diagnoses"": ""&#xf470; Diagnoses solid"",
	""fab fa-deviantart"": ""&#xf1bd; deviantART brands"",
	""fas fa-desktop"": ""&#xf108; Desktop solid"",
	""fab fa-deskpro"": ""&#xf38f; Deskpro brands"",
	""fab fa-deploydog"": ""&#xf38e; deploy.dog brands"",
	""fab fa-delicious"": ""&#xf1a5; Delicious Logo brands"",
	""fas fa-deaf"": ""&#xf2a4; Deaf solid"",
	""fas fa-database"": ""&#xf1c0; Database solid"",
	""fab fa-dashcube"": ""&#xf210; DashCube brands"",
	""fab fa-d-and-d"": ""&#xf38d; Dungeons & Dragons brands"",
	""fab fa-cuttlefish"": ""&#xf38c; Cuttlefish brands"",
	""fas fa-cut"": ""&#xf0c4; Cut solid"",
	""fas fa-cubes"": ""&#xf1b3; Cubes solid"",
	""fas fa-cube"": ""&#xf1b2; Cube solid"",
	""fab fa-css3-alt"": ""&#xf38b; Alternate CSS3 Logo brands"",
	""fab fa-css3"": ""&#xf13c; CSS 3 Logo brands"",
	""fas fa-crosshairs"": ""&#xf05b; Crosshairs solid"",
	""fas fa-crop"": ""&#xf125; crop solid"",
	""fas fa-credit-card"": ""&#xf09d; Credit Card solid"",
	""far fa-credit-card"": ""&#xf09d; Credit Card regular"",
	""fab fa-creative-commons-share"": ""&#xf4f2; Creative Commons Share brands"",
	""fab fa-creative-commons-sampling-plus"": ""&#xf4f1; Creative Commons Sampling + brands"",
	""fab fa-creative-commons-sampling"": ""&#xf4f0; Creative Commons Sampling brands"",
	""fab fa-creative-commons-sa"": ""&#xf4ef; Creative Commons Share Alike brands"",
	""fab fa-creative-commons-remix"": ""&#xf4ee; Creative Commons Remix brands"",
	""fab fa-creative-commons-pd-alt"": ""&#xf4ed; Creative Commons Public Domain Alternate brands"",
	""fab fa-creative-commons-pd"": ""&#xf4ec; Creative Commons Public Domain brands"",
	""fab fa-creative-commons-nd"": ""&#xf4eb; Creative Commons No Derivative Works brands"",
	""fab fa-creative-commons-nc-jp"": ""&#xf4ea; Creative Commons Noncommercial (Yen Sign) brands"",
	""fab fa-creative-commons-nc-eu"": ""&#xf4e9; Creative Commons Noncommercial (Euro Sign) brands"",
	""fab fa-creative-commons-nc"": ""&#xf4e8; Creative Commons Noncommercial brands"",
	""fab fa-creative-commons-by"": ""&#xf4e7; Creative Commons Attribution brands"",
	""fab fa-creative-commons"": ""&#xf25e; Creative Commons brands"",
	""fab fa-cpanel"": ""&#xf388; cPanel brands"",
	""fas fa-couch"": ""&#xf4b8; Couch solid"",
	""fas fa-copyright"": ""&#xf1f9; Copyright solid"",
	""far fa-copyright"": ""&#xf1f9; Copyright regular"",
	""fas fa-copy"": ""&#xf0c5; Copy solid"",
	""far fa-copy"": ""&#xf0c5; Copy regular"",
	""fab fa-contao"": ""&#xf26d; Contao brands"",
	""fab fa-connectdevelop"": ""&#xf20e; Connect Develop brands"",
	""fas fa-compress"": ""&#xf066; Compress solid"",
	""fas fa-compass"": ""&#xf14e; Compass solid"",
	""far fa-compass"": ""&#xf14e; Compass regular"",
	""fas fa-comments"": ""&#xf086; comments solid"",
	""far fa-comments"": ""&#xf086; comments regular"",
	""fas fa-comment-slash"": ""&#xf4b3; Comment Slash solid"",
	""fas fa-comment-dots"": ""&#xf4ad; Comment Dots solid"",
	""far fa-comment-dots"": ""&#xf4ad; Comment Dots regular"",
	""fas fa-comment-alt"": ""&#xf27a; Alternate Comment solid"",
	""far fa-comment-alt"": ""&#xf27a; Alternate Comment regular"",
	""fas fa-comment"": ""&#xf075; comment solid"",
	""far fa-comment"": ""&#xf075; comment regular"",
	""fas fa-columns"": ""&#xf0db; Columns solid"",
	""fas fa-cogs"": ""&#xf085; cogs solid"",
	""fas fa-cog"": ""&#xf013; cog solid"",
	""fas fa-coffee"": ""&#xf0f4; Coffee solid"",
	""fab fa-codiepie"": ""&#xf284; Codie Pie brands"",
	""fab fa-codepen"": ""&#xf1cb; Codepen brands"",
	""fas fa-code-branch"": ""&#xf126; Code Branch solid"",
	""fas fa-code"": ""&#xf121; Code solid"",
	""fab fa-cloudversify"": ""&#xf385; cloudversify brands"",
	""fab fa-cloudsmith"": ""&#xf384; Cloudsmith brands"",
	""fab fa-cloudscale"": ""&#xf383; cloudscale.ch brands"",
	""fas fa-cloud-upload-alt"": ""&#xf382; Cloud Upload Alternate solid"",
	""fas fa-cloud-download-alt"": ""&#xf381; Cloud Download Alternate solid"",
	""fas fa-cloud"": ""&#xf0c2; Cloud solid"",
	""fas fa-closed-captioning"": ""&#xf20a; Closed Captioning solid"",
	""far fa-closed-captioning"": ""&#xf20a; Closed Captioning regular"",
	""fas fa-clone"": ""&#xf24d; Clone solid"",
	""far fa-clone"": ""&#xf24d; Clone regular"",
	""fas fa-clock"": ""&#xf017; Clock solid"",
	""far fa-clock"": ""&#xf017; Clock regular"",
	""fas fa-clipboard-list"": ""&#xf46d; Clipboard List solid"",
	""fas fa-clipboard-check"": ""&#xf46c; Clipboard Check solid"",
	""fas fa-clipboard"": ""&#xf328; Clipboard solid"",
	""far fa-clipboard"": ""&#xf328; Clipboard regular"",
	""fas fa-circle-notch"": ""&#xf1ce; Circle Notched solid"",
	""fas fa-circle"": ""&#xf111; Circle solid"",
	""far fa-circle"": ""&#xf111; Circle regular"",
	""fab fa-chrome"": ""&#xf268; Chrome brands"",
	""fas fa-child"": ""&#xf1ae; Child solid"",
	""fas fa-chevron-up"": ""&#xf077; chevron-up solid"",
	""fas fa-chevron-right"": ""&#xf054; chevron-right solid"",
	""fas fa-chevron-left"": ""&#xf053; chevron-left solid"",
	""fas fa-chevron-down"": ""&#xf078; chevron-down solid"",
	""fas fa-chevron-circle-up"": ""&#xf139; Chevron Circle Up solid"",
	""fas fa-chevron-circle-right"": ""&#xf138; Chevron Circle Right solid"",
	""fas fa-chevron-circle-left"": ""&#xf137; Chevron Circle Left solid"",
	""fas fa-chevron-circle-down"": ""&#xf13a; Chevron Circle Down solid"",
	""fas fa-chess-rook"": ""&#xf447; Chess Rook solid"",
	""fas fa-chess-queen"": ""&#xf445; Chess Queen solid"",
	""fas fa-chess-pawn"": ""&#xf443; Chess Pawn solid"",
	""fas fa-chess-knight"": ""&#xf441; Chess Knight solid"",
	""fas fa-chess-king"": ""&#xf43f; Chess King solid"",
	""fas fa-chess-board"": ""&#xf43c; Chess Board solid"",
	""fas fa-chess-bishop"": ""&#xf43a; Chess Bishop solid"",
	""fas fa-chess"": ""&#xf439; Chess solid"",
	""fas fa-check-square"": ""&#xf14a; Check Square solid"",
	""far fa-check-square"": ""&#xf14a; Check Square regular"",
	""fas fa-check-circle"": ""&#xf058; Check Circle solid"",
	""far fa-check-circle"": ""&#xf058; Check Circle regular"",
	""fas fa-check"": ""&#xf00c; Check solid"",
	""fas fa-chart-pie"": ""&#xf200; Pie Chart solid"",
	""fas fa-chart-line"": ""&#xf201; Line Chart solid"",
	""fas fa-chart-bar"": ""&#xf080; Bar Chart solid"",
	""far fa-chart-bar"": ""&#xf080; Bar Chart regular"",
	""fas fa-chart-area"": ""&#xf1fe; Area Chart solid"",
	""fas fa-certificate"": ""&#xf0a3; certificate solid"",
	""fab fa-centercode"": ""&#xf380; Centercode brands"",
	""fab fa-cc-visa"": ""&#xf1f0; Visa Credit Card brands"",
	""fab fa-cc-stripe"": ""&#xf1f5; Stripe Credit Card brands"",
	""fab fa-cc-paypal"": ""&#xf1f4; Paypal Credit Card brands"",
	""fab fa-cc-mastercard"": ""&#xf1f1; MasterCard Credit Card brands"",
	""fab fa-cc-jcb"": ""&#xf24b; JCB Credit Card brands"",
	""fab fa-cc-discover"": ""&#xf1f2; Discover Credit Card brands"",
	""fab fa-cc-diners-club"": ""&#xf24c; Diner's Club Credit Card brands"",
	""fab fa-cc-apple-pay"": ""&#xf416; Apple Pay Credit Card brands"",
	""fab fa-cc-amex"": ""&#xf1f3; American Express Credit Card brands"",
	""fab fa-cc-amazon-pay"": ""&#xf42d; Amazon Pay Credit Card brands"",
	""fas fa-cart-plus"": ""&#xf217; Add to Shopping Cart solid"",
	""fas fa-cart-arrow-down"": ""&#xf218; Shopping Cart Arrow Down solid"",
	""fas fa-caret-up"": ""&#xf0d8; Caret Up solid"",
	""fas fa-caret-square-up"": ""&#xf151; Caret Square Up solid"",
	""far fa-caret-square-up"": ""&#xf151; Caret Square Up regular"",
	""fas fa-caret-square-right"": ""&#xf152; Caret Square Right solid"",
	""far fa-caret-square-right"": ""&#xf152; Caret Square Right regular"",
	""fas fa-caret-square-left"": ""&#xf191; Caret Square Left solid"",
	""far fa-caret-square-left"": ""&#xf191; Caret Square Left regular"",
	""fas fa-caret-square-down"": ""&#xf150; Caret Square Down solid"",
	""far fa-caret-square-down"": ""&#xf150; Caret Square Down regular"",
	""fas fa-caret-right"": ""&#xf0da; Caret Right solid"",
	""fas fa-caret-left"": ""&#xf0d9; Caret Left solid"",
	""fas fa-caret-down"": ""&#xf0d7; Caret Down solid"",
	""fas fa-car"": ""&#xf1b9; Car solid"",
	""fas fa-capsules"": ""&#xf46b; Capsules solid"",
	""fas fa-camera-retro"": ""&#xf083; Retro Camera solid"",
	""fas fa-camera"": ""&#xf030; camera solid"",
	""fas fa-calendar-times"": ""&#xf273; Calendar Times solid"",
	""far fa-calendar-times"": ""&#xf273; Calendar Times regular"",
	""fas fa-calendar-plus"": ""&#xf271; Calendar Plus solid"",
	""far fa-calendar-plus"": ""&#xf271; Calendar Plus regular"",
	""fas fa-calendar-minus"": ""&#xf272; Calendar Minus solid"",
	""far fa-calendar-minus"": ""&#xf272; Calendar Minus regular"",
	""fas fa-calendar-check"": ""&#xf274; Calendar Check solid"",
	""far fa-calendar-check"": ""&#xf274; Calendar Check regular"",
	""fas fa-calendar-alt"": ""&#xf073; Alternate Calendar solid"",
	""far fa-calendar-alt"": ""&#xf073; Alternate Calendar regular"",
	""fas fa-calendar"": ""&#xf133; Calendar solid"",
	""far fa-calendar"": ""&#xf133; Calendar regular"",
	""fas fa-calculator"": ""&#xf1ec; Calculator solid"",
	""fab fa-buysellads"": ""&#xf20d; BuySellAds brands"",
	""fas fa-bus"": ""&#xf207; Bus solid""
}";
			#endregion

			var iconsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

			foreach (var key in iconsDict.Keys)
			{
				var valueArray = iconsDict[key].Split("; ");
				var symbol = valueArray[0] + ";";
				var name = valueArray[1];

				var icon = new WvIcon()
				{
					Class = key,
					Symbol = symbol,
					Name = name
				};

				icons.Add(icon);
			}
			return icons;
		}

		public static string GetPathTypeIcon(string filePath)
		{
			var fontAwesomeIconName = "fa-file";
			if (filePath.EndsWith(".txt"))
			{
				fontAwesomeIconName = "fa-file-alt";
			}
			else if (filePath.EndsWith(".pdf"))
			{
				fontAwesomeIconName = "fa-file-pdf";
			}
			else if (filePath.EndsWith(".doc") || filePath.EndsWith(".docx"))
			{
				fontAwesomeIconName = "fa-file-word";
			}
			else if (filePath.EndsWith(".xls") || filePath.EndsWith(".xlsx"))
			{
				fontAwesomeIconName = "fa-file-excel";
			}
			else if (filePath.EndsWith(".ppt") || filePath.EndsWith(".pptx"))
			{
				fontAwesomeIconName = "fa-file-powerpoint";
			}
			else if (filePath.EndsWith(".gif") || filePath.EndsWith(".jpg")
				|| filePath.EndsWith(".jpeg") || filePath.EndsWith(".png")
				|| filePath.EndsWith(".bmp") || filePath.EndsWith(".tif"))
			{
				fontAwesomeIconName = "fa-file-image";
			}
			else if (filePath.EndsWith(".zip") || filePath.EndsWith(".zipx")
				 || filePath.EndsWith(".rar") || filePath.EndsWith(".tar")
				  || filePath.EndsWith(".gz") || filePath.EndsWith(".dmg")
					|| filePath.EndsWith(".iso"))
			{
				fontAwesomeIconName = "fa-file-archive";
			}
			else if (filePath.EndsWith(".wav") || filePath.EndsWith(".mp3")
				 || filePath.EndsWith(".fla") || filePath.EndsWith(".flac")
				  || filePath.EndsWith(".ra") || filePath.EndsWith(".rma")
					|| filePath.EndsWith(".aif") || filePath.EndsWith(".aiff")
					|| filePath.EndsWith(".aa") || filePath.EndsWith(".aac")
					 || filePath.EndsWith(".aax") || filePath.EndsWith(".ac3")
					  || filePath.EndsWith(".au") || filePath.EndsWith(".ogg")
						|| filePath.EndsWith(".avr") || filePath.EndsWith(".3ga")
						|| filePath.EndsWith(".mid") || filePath.EndsWith(".midi")
						 || filePath.EndsWith(".m4a") || filePath.EndsWith(".mp4a")
						  || filePath.EndsWith(".amz") || filePath.EndsWith(".mka")
							|| filePath.EndsWith(".asx") || filePath.EndsWith(".pcm")
							|| filePath.EndsWith(".m3u") || filePath.EndsWith(".wma")
							 || filePath.EndsWith(".xwma"))
			{
				fontAwesomeIconName = "fa-file-audio";
			}
			else if (filePath.EndsWith(".avi") || filePath.EndsWith(".mpg")
				 || filePath.EndsWith(".mp4") || filePath.EndsWith(".mkv")
				  || filePath.EndsWith(".mov") || filePath.EndsWith(".wmv")
					|| filePath.EndsWith(".vp6") || filePath.EndsWith(".264")
					|| filePath.EndsWith(".vid") || filePath.EndsWith(".rv")
					 || filePath.EndsWith(".webm") || filePath.EndsWith(".swf")
					  || filePath.EndsWith(".h264") || filePath.EndsWith(".flv")
						|| filePath.EndsWith(".mk3d") || filePath.EndsWith(".gifv")
						|| filePath.EndsWith(".oggv") || filePath.EndsWith(".3gp")
						 || filePath.EndsWith(".m4v") || filePath.EndsWith(".movie")
						  || filePath.EndsWith(".divx"))
			{
				fontAwesomeIconName = "fa-file-video";
			}
			else if (filePath.EndsWith(".c") || filePath.EndsWith(".cpp")
				 || filePath.EndsWith(".css") || filePath.EndsWith(".js")
				 || filePath.EndsWith(".py") || filePath.EndsWith(".git")
				  || filePath.EndsWith(".cs") || filePath.EndsWith(".cshtml")
				  || filePath.EndsWith(".xml") || filePath.EndsWith(".html")
					|| filePath.EndsWith(".ini") || filePath.EndsWith(".config")
					|| filePath.EndsWith(".json") || filePath.EndsWith(".h"))
			{
				fontAwesomeIconName = "fa-file-code";
			}
			else if (filePath.EndsWith(".exe") || filePath.EndsWith(".jar")
				 || filePath.EndsWith(".dll") || filePath.EndsWith(".bat")
				 || filePath.EndsWith(".pl") || filePath.EndsWith(".scr")
				  || filePath.EndsWith(".msi") || filePath.EndsWith(".app")
				  || filePath.EndsWith(".deb") || filePath.EndsWith(".apk")
					|| filePath.EndsWith(".jar") || filePath.EndsWith(".vb")
					|| filePath.EndsWith(".prg") || filePath.EndsWith(".sh"))
			{
				fontAwesomeIconName = "fa-cogs";
			}
			else if (filePath.EndsWith(".com") || filePath.EndsWith(".net")
				 || filePath.EndsWith(".org") || filePath.EndsWith(".edu")
				 || filePath.EndsWith(".gov") || filePath.EndsWith(".mil")
				  || filePath.EndsWith("/") || filePath.EndsWith(".html")
				  || filePath.EndsWith(".htm") || filePath.EndsWith(".xhtml")
					|| filePath.EndsWith(".jhtml") || filePath.EndsWith(".php")
					|| filePath.EndsWith(".php3") || filePath.EndsWith(".php4")
					|| filePath.EndsWith(".php5") || filePath.EndsWith(".phtml")
					|| filePath.EndsWith(".asp") || filePath.EndsWith(".aspx")
					|| filePath.EndsWith(".aspx") || filePath.EndsWith("?")
					|| filePath.EndsWith("#"))
			{
				fontAwesomeIconName = "fa-globe";
			}
			return fontAwesomeIconName;
		}

		public static string GetFileNameFromPath(string hreflink)
		{
			if (!hreflink.StartsWith("http"))
			{
				hreflink = "http://domain.com" + hreflink;
			}
			try
			{
				Uri uri = new Uri(hreflink);
				return Path.GetFileName(uri.LocalPath);
			}
			catch
			{
				return "unknown name";
			}
		}

		public static string GetSizeStringFromSize(int sizeKBInt)
		{
			var sizeString = "";
			if (sizeKBInt < 1024)
			{
				sizeString = sizeKBInt + " KB";
			}
			else if (sizeKBInt >= 1024 && sizeKBInt < Math.Pow(1024, 2))
			{
				sizeString = Math.Round((decimal)(sizeKBInt / 1024), 1) + " MB";
			}
			else
			{
				sizeString = Math.Round((decimal)(sizeKBInt / Math.Pow(1024, 2)), 1) + " GB";
			}

			return sizeString;
		}

		public static List<dynamic> GetCsvData(string csvData, bool hasHeader = true, string delimiterName = "")
		{
			var records = new List<dynamic>();
			using (TextReader reader = new StringReader(csvData))
			{
				var config = new CsvConfiguration(CultureInfo.InvariantCulture)
				{
					Encoding = Encoding.UTF8,
					IgnoreBlankLines = true,
					BadDataFound = null,
					TrimOptions = TrimOptions.Trim,
					HasHeaderRecord = hasHeader
				};

				if (hasHeader)
				{
					var headerList = GetCsvHeader(csvData, delimiterName);
					config.PrepareHeaderForMatch = (args) => headerList[args.FieldIndex];
				}
				switch (delimiterName)
				{
					case "tab":
						config.Delimiter = "\t";
						break;
					default:
						break;
				}

				using (var csvReader = new CsvReader(reader, config))
				{
					records = csvReader.GetRecords<dynamic>().ToList();
				}
			}
			return records;
		}

		public static List<string> GetCsvHeader(string csvData, string delimiterName = "")
		{
			var records = new List<dynamic>();
			var headerList = new List<string>();
			using (TextReader reader = new StringReader(csvData))
			{
				var config = new CsvConfiguration(CultureInfo.InvariantCulture)
				{
					Encoding = Encoding.UTF8,
					IgnoreBlankLines = true,
					BadDataFound = null,
					TrimOptions = TrimOptions.Trim,
					HasHeaderRecord = false
				};

				switch (delimiterName)
				{
					case "tab":
						config.Delimiter = "\t";
						break;
					default:
						break;
				}

				using (var csvReader = new CsvReader(reader, config))
				{
					records = csvReader.GetRecords<dynamic>().ToList();

					if (records.Count > 0)
					{
						//First row is headers -> we are getting them this way to bypass duplicate headers name problem
						foreach (KeyValuePair<string, object> prop in records[0])
						{
							string header = prop.Value.ToString().Trim();
							if (headerList.Contains(header))
							{
								var g = 2;
								var newHeader = header;
								while (true)
								{
									newHeader = header + g;
									if (!headerList.Contains(newHeader))
									{
										headerList.Add(newHeader);
										break;
									}
									g++;
								}
							}
							else
							{
								headerList.Add(header);
							}
						}
					}

				}
			}
			return headerList;

		}


		#endregion

	}
}
