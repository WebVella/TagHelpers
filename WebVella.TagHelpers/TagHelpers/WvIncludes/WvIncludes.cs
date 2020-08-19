using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-includes")]
	public class WvIncludes : TagHelper
	{
		[HtmlAttributeName("exclude-regex")]
		public string ExcludeRegex { get; set; } = "";

		[HtmlAttributeName("include-regex")]
		public string IncludeRegex { get; set; } = "";

		[HtmlAttributeName("cache-breaker")]
		public string CacheBreaker { get; set; } = "";

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			//Excluded CSS
			//"/_content/WebVella.TagHelpers/lib/flatpickr/flatpickr.min.css",
			//"/_content/WebVella.TagHelpers/lib/spectrum/spectrum.min.css",
			//"/_content/WebVella.TagHelpers/lib/select2/css/select2.min.css",

			List<string> CssFilesToImport = new List<string>{
				"/_content/WebVella.TagHelpers/lib/twitter-bootstrap/css/bootstrap.css",
				
				"/_content/WebVella.TagHelpers/lib/font-awesome/css/all.min.css",
				"/_content/WebVella.TagHelpers/lib/toastr.js/toastr.min.css",
				
				"/_content/WebVella.TagHelpers/css/button-colors.css",
				"/_content/WebVella.TagHelpers/css/colors.css",
				"/_content/WebVella.TagHelpers/css/styles.css"
			};


			//Excluded JS
			//"/_content/WebVella.TagHelpers/lib/flatpickr/flatpickr.min.js",
			//"/_content/WebVella.TagHelpers/lib/spectrum/spectrum.min.js",
			//"/_content/WebVella.TagHelpers/lib/select2/js/select2.min.js",
			//"/_content/WebVella.TagHelpers/lib/ckeditor/ckeditor.js",
			
			List<string> JsFilesToImport = new List<string>{
				"/_content/WebVella.TagHelpers/lib/ckeditor5/ckeditor.js",
				"/_content/WebVella.TagHelpers/lib/jquery/jquery.min.js",
				"/_content/WebVella.TagHelpers/lib/twitter-bootstrap/js/bootstrap.bundle.min.js",
				"/_content/WebVella.TagHelpers/lib/URI.js/URI.min.js",
				"/_content/WebVella.TagHelpers/lib/moment.js/moment.min.js",
				"/_content/WebVella.TagHelpers/lib/lodash.js/lodash.min.js",
				"/_content/WebVella.TagHelpers/lib/decimal.js/decimal.min.js",
				"/_content/WebVella.TagHelpers/lib/toastr.js/toastr.min.js",
				"/_content/WebVella.TagHelpers/js/scripts.js"
			};


			foreach (var filePath in CssFilesToImport)
			{
				var isExluded = false;

				if(!string.IsNullOrWhiteSpace(IncludeRegex)){
					Regex rgx = new Regex(IncludeRegex);
					if(!rgx.IsMatch(filePath)){
						isExluded = true;
					}
				}

				if(!string.IsNullOrWhiteSpace(ExcludeRegex)){
					Regex rgx = new Regex(ExcludeRegex);
					if(rgx.IsMatch(filePath)){
						isExluded = true;
					}
				}



				if(!isExluded){
					var linkEl = new TagBuilder("link");
					linkEl.Attributes.Add("href",$"{filePath}{(String.IsNullOrWhiteSpace(CacheBreaker) ? "" : $"?cb={CacheBreaker}")}");
					linkEl.Attributes.Add("type", "text/css");
					linkEl.Attributes.Add("rel", "stylesheet");
					output.PostContent.AppendHtml(linkEl);
					output.PostContent.AppendHtml("\r\n\t");
				}
			}


			foreach (var filePath in JsFilesToImport)
			{
				var isExluded = false;
				if(!string.IsNullOrWhiteSpace(ExcludeRegex)){
					Regex rgx = new Regex(ExcludeRegex);
					if(rgx.IsMatch(filePath)){
						isExluded = true;
					}
				}

				if(!isExluded){
					var scriptEl = new TagBuilder("script");
					scriptEl.Attributes.Add("src",$"{filePath}{(String.IsNullOrWhiteSpace(CacheBreaker) ? "" : $"?cb={CacheBreaker}")}");
					scriptEl.Attributes.Add("type", "text/javascript");
					output.PostContent.AppendHtml(scriptEl);
					output.PostContent.AppendHtml("\r\n\t");
				}
			}

			output.TagName = "";
			return Task.CompletedTask;
		}

	}
}
