using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WebVella.TagHelpers.Models;
using WebVella.TagHelpers.Utilities;


namespace WebVella.TagHelpers.TagHelpers
{
	[HtmlTargetElement("wv-echarts")]
	public class WvECharts : TagHelper
	{
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		[HtmlAttributeName("is-visible")]
		public bool isVisible { get; set; } = true;

		[HtmlAttributeName("mode")]
		public string Mode { get; set; } = "";

		[HtmlAttributeName("options")]
		public dynamic Options { get; set; } = null;

		[HtmlAttributeName("css-class")]
		public string CssClass { get; set; } = "";

		[HtmlAttributeName("width")]
		public string Width { get; set; } = "100%";

		[HtmlAttributeName("height")]
		public string Height { get; set; } = "300px";

		private Guid ElementId { get; set; }

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!isVisible)
			{
				output.SuppressOutput();
				return Task.CompletedTask;
			}

			ElementId = Guid.NewGuid();

			output.TagName = "div";
			output.AddCssClass($"wv-echarts {CssClass}");

			var wrapperEl = new TagBuilder("div");
			wrapperEl.Attributes.Add("id", "echarts-" + ElementId);
			wrapperEl.AddCssClass("echart");
			var style = "";
			if (!String.IsNullOrWhiteSpace(Width))
			{
				style += $"width:{Width};";
			}
			if (!String.IsNullOrWhiteSpace(Height))
			{
				style += $"height:{Height};";
			}
			wrapperEl.Attributes.Add("style", style);
			output.Content.AppendHtml(wrapperEl);

			dynamic options = null;
			//Default mode - just apply options
			if (String.IsNullOrWhiteSpace(Mode))
			{
				options = Options;
				if (String.IsNullOrWhiteSpace(Width))
					Width = "100%";
				if (String.IsNullOrWhiteSpace(Height))
					Width = "300px";
			}
			else if (Mode == "map")
			{
				options = new
				{
					series = new List<dynamic>{
						new {
							data = new List<dynamic>{
								new {
									name = "California",
									value = 1,
									itemStyle = new {
										normal = new {
											color = "#009688"
										}
									}
								},
								new {
									name = "Florida",
									value = 0
								}
							},
							type = "map",
							mapType = "USA",
							top = 0,
							left = 0,
							right = 0,
							markPoint = new {
								symbol = "none"
							},
							zoom = 1,
							scaleLimit = new {
								min = 1,
								max = 4
							},
							roam = true,
							itemStyle = new {
								emphasis = new {
									areaColor = "#ccc",
									color = "#ccc",
									label = new {
										color = "#333"
									}
								}
							}

						}
					}
				};
			}
			else if (Mode == "match")
			{

				options = new
				{
					radar = new
					{
						indicator = new List<dynamic>{
							new{
								name = "profile",
								max = 100
							},
							new{
								name = "time",
								max = 100
							},
							new{
								name = "location",
								max = 100
							},
							new{
								name = "experience",
								max = 100
							},
							new{
								name = "certification",
								max = 100
							},
						}
					},
					series = new List<dynamic>{
						new {
							type = "radar",
							top = 0,
							left = 0,
							right = 0,
							data = new List<dynamic>{
								new{
									areaStyle = new { color = "#009688", opacity = 0.25},
									lineStyle = new { color = "#009688"},
									value = new List<int>{ 55, 25, 5, 75, 34}
								}
							}
						}
					}
				};
			}
			else if (Mode == "expertise")
			{

				options = new
				{
					legend = new
					{
						bottom = 0,
						top = "auto",
						data = new List<dynamic>{
							new {
								name = "certification",
								icon = "circle"
							},
							new {
								name = "experience",
								icon = "circle"
							},
							new {
								name = "selection",
								icon = "circle"
							}
						}
					},
					radar = new
					{
						indicator = new List<dynamic>{
							new{
								name = "Securely Provision",
								max = 100
							},
							new{
								name = "Operate and Maintain",
								max = 100
							},
							new{
								name = "Oversee and Govern",
								max = 100
							},
							new{
								name = "Protect and Defend",
								max = 100
							},
							new{
								name = "Analyze",
								max = 100
							},
							new{
								name = "Collect and Operate",
								max = 100
							},
							new{
								name = "Investigate",
								max = 100
							},
						}
					},
					series = new List<dynamic>{
						new {
							type = "radar",
							top = 0,
							left = 0,
							right = 0,
							data = new List<dynamic>{
								new{
									areaStyle = new { color = "#009688", opacity = 0.25},
									lineStyle = new { color = "#009688"},
									itemStyle = new { color = "#009688"},
									value = new List<int>{ 55, 0, 0, 0, 34, 34, 0},
									name = "certification"
								},
								new{
									areaStyle = new { color = "#E91E63", opacity = 0.25},
									lineStyle = new { color = "#E91E63"},
									itemStyle = new { color = "#E91E63"},
									value = new List<int>{ 12, 33, 55, 12, 1, 33, 45},
									name = "experience"
								},
								new{
									areaStyle = new { color = "#2196F3", opacity = 0.25},
									lineStyle = new { color = "#2196F3"},
									itemStyle = new { color = "#2196F3"},
									value = new List<int>{ 85, 45, 34, 0, 0, 33, 44},
									name = "selection"
								}
							}
						}
					}
				};
			}
			else if (Mode == "calendar")
			{
				var calendarData = new List<dynamic>();
				for (int i = 0; i < 190; i++)
				{
					Random r = new Random();
					var number = r.Next(0, 3);
					var date = DateTime.ParseExact("2019-10-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).AddDays(i);
					calendarData.Add(new List<dynamic>{
						date.ToString("yyyy-MM-dd"),number
					});
				}

				options = new
				{
					visualMap = new
					{
						show = false,
						min = 0,
						max = 2,
						color = new List<string> { "#E57373", "#FFB74D", "#eee" }
					},
					calendar = new
					{
						range = new List<string> { "2019-10", "2020-4" },
						splitLine = new
						{
							lineStyle = new
							{
								color = "#fff",
								width = 3,
								shadowBlur = 0,
								opacity = 1
							}
						},
						itemStyle = new
						{
							borderColor = "#fff",
							borderWidth = 1,
							shadowBlur = 0,
							opacity = 1
						}
					},
					series = new List<dynamic>{
						new {
							type = "heatmap",
							coordinateSystem = "calendar",
							left = 0,
							right = 0,
							data = calendarData
						}
					}
				};
			}
			else if (Mode == "hits")
			{
				var calendarData = new List<dynamic>();
				var calendaryDays = new List<string>();
				var calendarValues = new List<int>();
				for (int i = 0; i < 190; i++)
				{
					Random r = new Random();
					var number = r.Next(0, 3);
					var date = DateTime.ParseExact("2019-10-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).AddDays(i);
					calendarData.Add(new List<dynamic>{
						date.ToString("yyyy-MM-dd"),number
					});

					calendaryDays.Add(date.ToString("MM/dd"));
					calendarValues.Add(r.Next(0, 300));
				}

				options = new
				{
					visualMap = new
					{
						show = false,
						min = 0,
						max = 300,
						color = new List<string> { "#009688", "#E0F2F1" }
					},
					xAxis = new List<dynamic>{
						new {
							data = calendaryDays
						}
					},
					yAxis = new List<dynamic>{
						new {
							splitLine = new {show = false}
						}
					},
					series = new List<dynamic>{
						new {
							type = "line",
							showSymbol = false,
							data = calendarValues
						}
					}
				};
			}

			#region << Init Echarts >>
			{
				var wvLibraryInitialized = false;
				var libraryItemsKey = "WebVella-" + "echarts";
				if (ViewContext.HttpContext.Items.ContainsKey(libraryItemsKey))
				{
					var tagHelperContext = (WvTagHelperContext)ViewContext.HttpContext.Items[libraryItemsKey];
					wvLibraryInitialized = tagHelperContext.Initialized;
				}

				if (!wvLibraryInitialized)
				{
					{
						var libJsEl = new TagBuilder("script");
						libJsEl.Attributes.Add("type", "text/javascript");
						libJsEl.Attributes.Add("src", "/_content/WebVella.TagHelpers/lib/echarts/echarts-en.min.js");
						output.PostContent.AppendHtml(libJsEl);
						output.PostContent.AppendHtml("\r\n\t");
					}

					ViewContext.HttpContext.Items[libraryItemsKey] = new WvTagHelperContext()
					{
						Initialized = true
					};
				}
			}
			#endregion

			#region << Add Inline Init Script for this instance >>
			var initScript = new TagBuilder("script");
			initScript.Attributes.Add("type", "text/javascript");

			var scriptTemplate = "";
			if (Mode == "map")
			{
				scriptTemplate = @"
						$(function(){
	var myChart = echarts.init(document.getElementById(""echarts-{{ELEMENTID}}""));

	var usaJson = {{USAJSON}}

    echarts.registerMap('USA', usaJson, {
        Alaska: {              
            left: -131,
            top: 25,
            width: 15
        },
        Hawaii: {
            left: -110,        
            top: 28,
            width: 5
        },
        'Puerto Rico': {       
            left: -76,
            top: 26,
            width: 2
        }
    });

    myChart.setOption({{OPTIONS}});

myChart.on('click', function (params) {
    var stateName = params.name;
	var options = myChart.getOption();
	var stateIndex = _.findIndex(options.series[0].data,function(state){return state.name === stateName});
	if(stateIndex > -1){
		var state = options.series[0].data[stateIndex];
		if(state.value === 1){
			state.value = 0;
			delete state.itemStyle;
		}
		else{
			state.value = 1;
			state.itemStyle = {};
			state.itemStyle.normal = {};
			state.itemStyle.normal.color = '#009688';
		}
	}
	else{
		var state = {
			name: stateName,
			value: 1,
			itemStyle:{
				normal:{
					color: '#009688'
				}
			}
		}
		options.series[0].data.push(state);
	}
	myChart.setOption(options);
});

	$(window).on('resize', function(){
        if(myChart != null && myChart != undefined){
            myChart.resize();
        }
    });

	});";
			}
			else
			{

				scriptTemplate = @"
						$(function(){
	var myChart = echarts.init(document.getElementById(""echarts-{{ELEMENTID}}""));

    myChart.setOption({{OPTIONS}});



	$(window).on('resize', function(){
        if(myChart != null && myChart != undefined){
            myChart.resize();
        }
    });

	});";
			}


			var fieldConfig = new WvFieldSelectConfig()
			{
			};
			var usaJson = WvHelpers.GetEmbeddedTextResource("usa.json", "WebVella.TagHelpers.TagHelpers.WvECharts", "WebVella.TagHelpers");
			scriptTemplate = scriptTemplate.Replace("{{OPTIONS}}", JsonSerializer.Serialize(options));
			scriptTemplate = scriptTemplate.Replace("{{USAJSON}}", usaJson);
			scriptTemplate = scriptTemplate.Replace("{{ELEMENTID}}", ElementId.ToString());

			initScript.InnerHtml.AppendHtml(scriptTemplate);

			output.PostContent.AppendHtml(initScript);
			#endregion


			return Task.CompletedTask;
		}
	}
}
