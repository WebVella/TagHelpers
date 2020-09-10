//JS DateTime picker init method
var dateTimePickerDictionary = {};
var flatPickrServerDateTimeFormat = "Y-m-dTH:i:S";//"Z";
//From the server dates will be received yyyy-MM-ddTHH:mm:ss.fff
var flatPickrUiDateTimeFormat = "d M Y H:i";


var BulgarianDateTimeLocale = {
   firstDayOfWeek: 1,
   weekdays: {
      shorthand: ["Нд", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"],
      longhand: [
         "Неделя",
         "Понеделник",
         "Вторник",
         "Сряда",
         "Четвъртък",
         "Петък",
         "Събота"
      ]
   },

   months: {
      shorthand: [
         "яну",
         "фев",
         "март",
         "апр",
         "май",
         "юни",
         "юли",
         "авг",
         "сеп",
         "окт",
         "ное",
         "дек"
      ],
      longhand: [
         "Януари",
         "Февруари",
         "Март",
         "Април",
         "Май",
         "Юни",
         "Юли",
         "Август",
         "Септември",
         "Октомври",
         "Ноември",
         "Декември"
      ]
   }
};


function InitFlatPickrDateTime(fieldId, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selector = "#input-" + fieldId;
	if (document.querySelector(selector)) {
		var inputGroulEl = $(selector).closest(".input-group");

		if (!config.is_required) {
			//Inject clear link
			inputGroulEl.append("<a href='#' class='clear-link d-none'><i class='fa fa-times'><i></a>");

			var clearLink = inputGroulEl.find(".clear-link");
			//Show clear link if value not null or empty
			if ($(selector).val()) {
				clearLink.removeClass("d-none");
			}

			clearLink.click(function (event) {
				event.preventDefault();
				var fp = document.querySelector(selector)._flatpickr;
				if (fp) {
					fp.clear();
				}
				else {
					$(selector).val(null);
				}
				clearLink.addClass("d-none");
			});
		}

		var fp = document.querySelector(selector)._flatpickr;
		if (!fp) {
			var options = {
				time_24hr: true,
				dateFormat: flatPickrServerDateTimeFormat,
				defaultDate: null,
				//locale: BulgarianDateTimeLocale,
				enableTime: true,
				"static": true,
				minuteIncrement: 1,
				altInput: true,
				altFormat: flatPickrUiDateTimeFormat,
				onChange: function (selectedDates) {
					if (!config.is_required && selectedDates && selectedDates.length > 0) {
						clearLink.removeClass("d-none");
					}
				}
			};
			if (SiteLang && SiteLang === "bg") {
				options.locale = BulgarianDateTimeLocale;
			}
			fp = flatpickr(selector, options);
			return fp;
		}
		return fp;

	}
}