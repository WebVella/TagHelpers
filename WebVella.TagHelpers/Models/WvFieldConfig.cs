using Newtonsoft.Json;

namespace WebVella.TagHelpers.Models
{
	public class WvFieldBaseConfig
	{
		public WvFieldBaseConfig()
		{
			CanAddValues = null;
			ApiUrl = "";
			IsRequired = false;
		}

		[JsonProperty(PropertyName = "can_add_values")]
		public bool? CanAddValues { get; set; }

		[JsonProperty(PropertyName = "api_url")]
		public string ApiUrl { get; set; }

		[JsonProperty(PropertyName = "is_required")]
		public bool IsRequired { get; set; }
	}

	public enum HtmlUploadMode
	{
		[WvSelectOption(Label = "None")]
		None = 1,
		[WvSelectOption(Label = "Site Repository")]
		SiteRepository = 2
	}

	public enum HtmlToolbarMode
	{
		[WvSelectOption(Label = "Basic")]
		Basic = 1,
		[WvSelectOption(Label = "Standard")]
		Standard = 2,
		[WvSelectOption(Label = "Full")]
		Full = 3
	}

	public enum ImageResizeMode
	{
		[WvSelectOption(Label = "Pad")]
		Pad = 1,
		[WvSelectOption(Label = "BoxPad")]
		BoxPad = 2,
		[WvSelectOption(Label = "Crop")]
		Crop = 3,
		[WvSelectOption(Label = "Min")]
		Min = 4,
		[WvSelectOption(Label = "Max")]
		Max = 5,
		[WvSelectOption(Label = "Stretch")]
		Stretch = 6
	}

	public class WvFieldAutonumberConfig : WvFieldBaseConfig
	{
		public WvFieldAutonumberConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldAutonumberConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldCheckboxConfig : WvFieldBaseConfig
	{
		public WvFieldCheckboxConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			TrueLabel = "selected";
			FalseLabel = "not selected";
		}

		public WvFieldCheckboxConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			TrueLabel = "selected";
			FalseLabel = "not selected";
			ApiUrl = baseConfig.ApiUrl;
		}

		[JsonProperty(PropertyName = "true_label")]
		public string TrueLabel { get; set; }

		[JsonProperty(PropertyName = "false_label")]
		public string FalseLabel { get; set; }
	}

	public class WvFieldCurrencyConfig : WvFieldBaseConfig
	{
		public WvFieldCurrencyConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldCurrencyConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldDateConfig : WvFieldBaseConfig
	{
		public WvFieldDateConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldDateConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldDateTimeConfig : WvFieldBaseConfig
	{
		public WvFieldDateTimeConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldDateTimeConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldEmailConfig : WvFieldBaseConfig
	{
		public WvFieldEmailConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldEmailConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldFileConfig : WvFieldBaseConfig
	{

		public WvFieldFileConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			Accept = "";
			FileUploadApi = "/fs/upload";
			SrcPrefix = "/fs";
		}

		public WvFieldFileConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			Accept = "";
			ApiUrl = baseConfig.ApiUrl;
			FileUploadApi = "/fs/upload";
			SrcPrefix = "/fs";
		}

		[JsonProperty(PropertyName = "accept")]
		public string Accept { get; set; }

		[JsonProperty(PropertyName = "file_upload_api")]
		public string FileUploadApi { get; set; } = "/fs/upload";

		[JsonProperty(PropertyName = "src_prefix")]
		public string SrcPrefix { get; set; } = "/fs";
	}

	public class WvFieldHtmlConfig : WvFieldBaseConfig
	{
		public WvFieldHtmlConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			UploadMode = HtmlUploadMode.None;
			ToolbarMode = HtmlToolbarMode.Basic;
		}

		public WvFieldHtmlConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			UploadMode = HtmlUploadMode.None;
			ToolbarMode = HtmlToolbarMode.Basic;
			ApiUrl = baseConfig.ApiUrl;
		}

		[JsonProperty(PropertyName = "upload_mode")]
		public HtmlUploadMode UploadMode { get; set; }

		[JsonProperty(PropertyName = "toolbar_mode")]
		public HtmlToolbarMode ToolbarMode { get; set; }
	}

	public class WvFieldImageConfig : WvFieldBaseConfig
	{
		public WvFieldImageConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			Accept = "";
			Width = null;
			Height = null;
			ResizeAction = null;
			FileUploadApi = "/fs/upload";
			SrcPrefix = "/fs";
		}

		public WvFieldImageConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			Accept = "";
			Width = null;
			Height = null;
			ResizeAction = null;
			ApiUrl = baseConfig.ApiUrl;
			FileUploadApi = "/fs/upload";
			SrcPrefix = "/fs";
		}

		[JsonProperty(PropertyName = "accept")]
		public string Accept { get; set; }

		[JsonProperty(PropertyName = "width")]
		public int? Width { get; set; }

		[JsonProperty(PropertyName = "height")]
		public int? Height { get; set; }

		[JsonProperty(PropertyName = "resize_action")]
		public ImageResizeMode? ResizeAction { get; set; }

		[JsonProperty(PropertyName = "file_upload_api")]
		public string FileUploadApi { get; set; } = "/fs/upload";

		[JsonProperty(PropertyName = "src_prefix")]
		public string SrcPrefix { get; set; } = "/fs";
	}

	public class WvFieldTextareaConfig : WvFieldBaseConfig
	{
		public WvFieldTextareaConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldTextareaConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldMultiSelectConfig : WvFieldBaseConfig
	{
		public WvFieldMultiSelectConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			AjaxDatasourceApi = "/api/v3/en_US/eql-ds";
		}

		public WvFieldMultiSelectConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
			AjaxDatasourceApi = "/api/v3/en_US/eql-ds";
		}

		[JsonProperty(PropertyName = "ajax_datasource")]
		public WvSelectOptionsAjaxDatasource AjaxDatasource { get; set; } = null;

		[JsonProperty(PropertyName = "ajax_datasource_api")]
		public string AjaxDatasourceApi { get; set; } = "/api/v3/en_US/eql-ds";

		[JsonProperty(PropertyName = "select_match_type")]
		public WvSelectMatchType SelectMatchType { get; set; } = WvSelectMatchType.Contains;

		[JsonProperty(PropertyName = "placeholder")]
		public string Placeholder { get; set; } = "";

	}

	public class WvFieldNumberConfig : WvFieldBaseConfig
	{
		public WvFieldNumberConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldNumberConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldPasswordConfig : WvFieldBaseConfig
	{
		public WvFieldPasswordConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldPasswordConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldPercentConfig : WvFieldBaseConfig
	{
		public WvFieldPercentConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			DecimalDigits = null;
		}

		public WvFieldPercentConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
			DecimalDigits = null;
		}

		[JsonProperty(PropertyName = "decimal_digits")]
		public int? DecimalDigits { get; set; }
	}

	public class WvFieldPhoneConfig : WvFieldBaseConfig
	{
		public WvFieldPhoneConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldPhoneConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldGuidConfig : WvFieldBaseConfig
	{
		public WvFieldGuidConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldGuidConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldSelectConfig : WvFieldBaseConfig
	{
		public WvFieldSelectConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			AjaxDatasourceApi = "/api/v3/en_US/eql-ds";
		}

		public WvFieldSelectConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
			AjaxDatasourceApi = "/api/v3/en_US/eql-ds";
		}

		[JsonProperty(PropertyName = "is_invalid")]
		public bool IsInvalid { get; set; } = false;

		[JsonProperty(PropertyName = "ajax_datasource")]
		public WvSelectOptionsAjaxDatasource AjaxDatasource { get; set; } = null;

		[JsonProperty(PropertyName = "ajax_datasource_api")]
		public string AjaxDatasourceApi { get; set; } = "/api/v3/en_US/eql-ds";

		[JsonProperty(PropertyName = "select_match_type")]
		public WvSelectMatchType SelectMatchType { get; set; } = WvSelectMatchType.Contains;

		[JsonProperty(PropertyName = "placeholder")]
		public string Placeholder { get; set; } = "";

	}

	public class WvFieldTextConfig : WvFieldBaseConfig
	{
		public WvFieldTextConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldTextConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldColorConfig : WvFieldBaseConfig
	{
		public WvFieldColorConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldColorConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldCodeConfig : WvFieldBaseConfig
	{
		public WvFieldCodeConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			Language = "razor";
			Theme = "xcode";
			ReadOnly = false;
		}

		public WvFieldCodeConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
			Language = "razor";
			Theme = "xcode";
			ReadOnly = false;
		}

		[JsonProperty(PropertyName = "language")]
		public string Language { get; set; } = "razor";

		[JsonProperty(PropertyName = "theme")]
		public string Theme { get; set; } = "xcode";

		[JsonProperty(PropertyName = "read_only")]
		public bool ReadOnly { get; set; } = false;
	}

	public class WvFieldUrlConfig : WvFieldBaseConfig
	{
		public WvFieldUrlConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
		}

		public WvFieldUrlConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
		}
	}

	public class WvFieldDataCsvConfig : WvFieldBaseConfig
	{
		public WvFieldDataCsvConfig()
		{
			var baseConfig = new WvFieldBaseConfig();
			CanAddValues = baseConfig.CanAddValues;
			PreviewApiUrl = "/api/v3.0/en/p/core/ui/field-table-data/generate/preview";
		}

		public WvFieldDataCsvConfig(WvFieldBaseConfig baseConfig)
		{
			CanAddValues = baseConfig.CanAddValues;
			ApiUrl = baseConfig.ApiUrl;
			PreviewApiUrl = "/api/v3.0/en/p/core/ui/field-table-data/generate/preview";
		}

		[JsonProperty(PropertyName = "preview_api_url")]
		public string PreviewApiUrl { get; set; } = "/api/v3.0/en/p/core/ui/field-table-data/generate/preview";
	}
}
