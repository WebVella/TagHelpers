using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace WebVella.TagHelpers.Models
{
	public class WvChartMeta
	{

		[JsonPropertyName("type")]
		public WvChartType Type { get; set; } = WvChartType.Line;

		[JsonPropertyName("data")]
		public WvChartData Data { get; set; }

		//Hard coded for now
		//[JsonProperty(PropertyName = "options")]
		//public ErpChartOptions Options { get; set; }

		/// <summary>
		/// A chart options json.
		/// </summary>
		[JsonPropertyName("options")]
		public dynamic Options { get; set; }

		public WvChartMeta(WvChartType type, List<string> labels, List<WvChartDataset> datasets, bool ShowLegend)
		{
			Type = type;
			Data = new WvChartData()
			{
				Labels = labels,
				Datasets = datasets
			};

			var datasetsLabelsCount = 0;
			foreach (var dataset in datasets)
			{
				if (!string.IsNullOrWhiteSpace(dataset.Label))
				{
					datasetsLabelsCount++;
				}
				if (dataset.Fill == null && Type == WvChartType.Area)
				{
					dataset.Fill = true;
				}
				else if (dataset.Fill == null)
				{
					dataset.Fill = false;
				}
				if (dataset.BorderColor == null)
				{
					switch (Type)
					{
						case WvChartType.Line:
						case WvChartType.Area:
							dataset.BorderColor = "rgba(0, 150, 136,1)";
							break;
						default:
							dataset.BorderColor = new List<string>();
							foreach (var item in dataset.Data)
								((List<string>)dataset.BorderColor).Add("rgba(0, 150, 136,1)");
							break;
					}
				}
				else if (dataset.BorderColor != null && Type != WvChartType.Line && Type != WvChartType.Area &&
					((List<string>)dataset.BorderColor).Count == 1 && dataset.Data.Count > 1)
				{
					var borderColor = ((List<string>)dataset.BorderColor).First();
					dataset.BorderColor = new List<string>();
					foreach (var item in dataset.Data)
						((List<string>)dataset.BorderColor).Add(borderColor);
				}

				if (dataset.BackgroundColor == null)
				{

					switch (Type)
					{
						case WvChartType.Line:
						case WvChartType.Area:
							dataset.BackgroundColor = "rgba(0, 150, 136,0.15)";
							break;
						default:
							dataset.BackgroundColor = new List<string>();
							foreach (var item in dataset.Data)
								((List<string>)dataset.BackgroundColor).Add("rgba(0, 150, 136,0.15)");
							break;
					}
				}
				else if (dataset.BackgroundColor != null && Type != WvChartType.Line && Type != WvChartType.Area
					&& ((List<string>)dataset.BackgroundColor).Count == 1 && dataset.Data.Count > 1)
				{
					var bgColor = ((List<string>)dataset.BackgroundColor).First();
					dataset.BackgroundColor = new List<string>();
					foreach (var item in dataset.Data)
						((List<string>)dataset.BackgroundColor).Add(bgColor);
				}
			}

			var options = new Dictionary<String, Object>();

			#region << General >>
			{
				options["responsive"] = true;
				options["maintainAspectRatio"] = false;
				if (Type == WvChartType.Doughnut)
				{
					options["cutoutPercentage"] = 70;
				}
			}
			#endregion

			#region << Layout >>
			{
				var layout = new Dictionary<String, Object>();
				layout["padding"] = 0;
				options["layout"] = layout;
			}
			#endregion

			#region << Animation >>
			{
				var animation = new Dictionary<String, Object>();
				animation["duration"] = 0;
				options["animation"] = animation;
			}
			#endregion

			#region << Tooltips >>
			{
				var tooltips = new Dictionary<String, Object>();
				tooltips["enabled"] = false;
				//tooltips["displayColors"] = false;
				//tooltips["callbacks"] = "{{tooltipCallback}}";
				options["tooltips"] = tooltips;
			}
			#endregion

			#region << legend >>
			{
				var legend = new Dictionary<String, Object>();
				if (ShowLegend)
				{
					legend["display"] = true;
					switch (Type)
					{
						case WvChartType.Pie:
						case WvChartType.Doughnut:
							legend["position"] = "bottom";
							break;
						default:
							legend["position"] = "bottom left";
							break;
					}
				}
				else
					legend["display"] = false;

				options["legend"] = legend;
			}
			#endregion

			#region << Scales >>
			{
				var scales = new Dictionary<String, Object>();

				//xAxes
				var xAxes = new List<Dictionary<String, Object>>();
				var xAxesFirst = new Dictionary<String, Object>();
				if (Type == WvChartType.Pie || Type == WvChartType.Doughnut)
				{
					xAxesFirst["display"] = false;
				}
				xAxesFirst["beginAtZero"] = false;

				if (Type == WvChartType.Bar || Type == WvChartType.HorizontalBar)
					xAxesFirst["offset"] = true;
				else
					xAxesFirst["offset"] = false;

				var xAxesFirstGridLine = new Dictionary<String, Object>();
				xAxesFirstGridLine["display"] = false;
				xAxesFirst["gridLines"] = xAxesFirstGridLine;

				var xAxesFirstScaleLabel = new Dictionary<String, Object>();
				xAxesFirstScaleLabel["display"] = false;
				xAxesFirst["scaleLabel"] = xAxesFirstScaleLabel;

				var xAxesFirstTicks = new Dictionary<String, Object>();
				xAxesFirstTicks["beginAtZero"] = false;
				xAxesFirstTicks["min"] = 0;
				xAxesFirst["ticks"] = xAxesFirstTicks;

				xAxes.Add(xAxesFirst);

				//yAxes
				var yAxes = new List<Dictionary<String, Object>>();
				var yAxesFirst = new Dictionary<String, Object>();

				if (Type == WvChartType.Pie || Type == WvChartType.Doughnut)
				{
					yAxesFirst["display"] = false;
				}

				yAxesFirst["beginAtZero"] = false;

				if (Type == WvChartType.Bar || Type == WvChartType.HorizontalBar)
					yAxesFirst["offset"] = true;
				else
					yAxesFirst["offset"] = false;

				var yAxesFirstGridLine = new Dictionary<String, Object>();
				yAxesFirstGridLine["display"] = false;
				yAxesFirst["gridLines"] = yAxesFirstGridLine;

				var yAxesFirstScaleLabel = new Dictionary<String, Object>();
				yAxesFirstScaleLabel["display"] = false;
				yAxesFirst["scaleLabel"] = yAxesFirstScaleLabel;

				var yAxesFirstTicks = new Dictionary<String, Object>();
				yAxesFirstTicks["padding"] = 0;
				yAxesFirstTicks["display"] = true;
				yAxesFirstTicks["beginAtZero"] = false;
				yAxesFirstTicks["min"] = 0;
				yAxesFirst["ticks"] = yAxesFirstTicks;


				yAxes.Add(yAxesFirst);

				scales["yAxes"] = yAxes;
				scales["xAxes"] = xAxes;
				options["scales"] = scales;
			}
			#endregion

			switch (type)
			{
				case WvChartType.Line:
					break;
				case WvChartType.Bar:
					break;
				case WvChartType.Doughnut:
				case WvChartType.Pie:
					break;
				default:
					break;
			}

			Options = options;
		}
	}
}
