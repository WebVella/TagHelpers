using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
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
		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			//Excluded CSS
			//"/webvella-taghelpers/lib/flatpickr/flatpickr.min.css",
			//"/webvella-taghelpers/lib/spectrum/spectrum.min.css",
			//"/webvella-taghelpers/lib/select2/css/select2.min.css",

			List<string> CssFilesToImport = new List<string>{
				"/webvella-taghelpers/lib/twitter-bootstrap/css/bootstrap.css",
				
				"/webvella-taghelpers/lib/font-awesome/css/all.min.css",
				"/webvella-taghelpers/lib/toastr.js/toastr.min.css",
				
				"/webvella-taghelpers/css/button-colors.css",
				"/webvella-taghelpers/css/colors.css",
				"/webvella-taghelpers/css/styles.css"
			};


			//Excluded JS
			//"/webvella-taghelpers/lib/flatpickr/flatpickr.min.js",
			//"/webvella-taghelpers/lib/spectrum/spectrum.min.js",
			//"/webvella-taghelpers/lib/select2/js/select2.min.js",
			//"/webvella-taghelpers/lib/ckeditor/ckeditor.js",
			
			List<string> JsFilesToImport = new List<string>{
				"/webvella-taghelpers/lib/jquery/jquery.min.js",
				"/webvella-taghelpers/lib/twitter-bootstrap/js/bootstrap.bundle.min.js",
				"/webvella-taghelpers/lib/URI.js/URI.min.js",
				"/webvella-taghelpers/lib/moment.js/moment.min.js",
				"/webvella-taghelpers/lib/lodash.js/lodash.min.js",
				"/webvella-taghelpers/lib/decimal.js/decimal.min.js",
				"/webvella-taghelpers/lib/toastr.js/toastr.min.js",
				"/webvella-taghelpers/js/scripts.js"
			};


			foreach (var filePath in CssFilesToImport)
			{
				var isExluded = false;
				if(!string.IsNullOrWhiteSpace(ExcludeRegex)){
					Regex rgx = new Regex(ExcludeRegex);
					if(rgx.IsMatch(filePath)){
						isExluded = true;
					}
				}

				if(!isExluded){
					var linkEl = new TagBuilder("link");
					linkEl.Attributes.Add("href",filePath);
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
					scriptEl.Attributes.Add("src",filePath);
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
