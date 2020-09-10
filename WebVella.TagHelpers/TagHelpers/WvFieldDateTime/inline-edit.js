
//JS DateTime picker init method
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
function InitFlatPickrDateTimeInlineEdit(editWrapperSelector) {
	var defaultDate = $(editWrapperSelector).attr("data-default-date");
	if (defaultDate === "") {
		defaultDate = null;
	}
	var options = {
		time_24hr: true,
		defaultDate: defaultDate,
		dateFormat: flatPickrServerDateTimeFormat,
		//locale: BulgarianDateTimeLocale,
		enableTime: true,
		"static": true,
		minuteIncrement: 1,
		altInput: true,
		altFormat: flatPickrUiDateTimeFormat
	};

	if (SiteLang && SiteLang === "bg") {
		options.locale = BulgarianDateTimeLocale;
	}

	flatpickr(editWrapperSelector + " .form-control", options);
}

function DateTimeInlineEditGenerateSelectors(fieldId, fieldName, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.viewWrapper = "#view-" + fieldId;
	selectors.editWrapper = "#edit-" + fieldId;
	return selectors;
}

function DateTimeInlineEditPreEnableCallback(fieldId, fieldName, config) {
	var selectors = DateTimeInlineEditGenerateSelectors(fieldId, fieldName, config);
	//init pickr only when needed
	InitFlatPickrDateTimeInlineEdit(selectors.editWrapper, "datetime");
	$(selectors.viewWrapper).hide();
	$(selectors.editWrapper).show();
	$(selectors.editWrapper + " .form-control").focus();
}

function DateTimeInlineEditPreDisableCallback(fieldId, fieldName, config) {
	var selectors = DateTimeInlineEditGenerateSelectors(fieldId, fieldName, config);
	$(selectors.editWrapper + " .invalid-feedback").remove();
	$(selectors.editWrapper + " .form-control").removeClass("is-invalid");
	$(selectors.editWrapper + " .save .fa").addClass("fa-check").removeClass("fa-spin fa-spinner");
	$(selectors.editWrapper + " .save").attr("disabled", false);
	$(selectors.viewWrapper).show();
	$(selectors.editWrapper).hide();
	var calendarInstance = document.querySelector(selectors.editWrapper + " .form-control")._flatpickr;
	calendarInstance.destroy();
}

function DateTimeInlineEditInit(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = DateTimeInlineEditGenerateSelectors(fieldId, fieldName, config);
	//Init enable action click
	$(selectors.viewWrapper + " .action .btn").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		DateTimeInlineEditPreEnableCallback(fieldId, fieldName, config);
	});
	//Init enable action dblclick
	$(selectors.viewWrapper + " .form-control").on("dblclick", function (event) {
		event.stopPropagation();
		event.preventDefault();
		DateTimeInlineEditPreEnableCallback(fieldId, fieldName, config);
		//WebVellaTagHelpers.clearSelection();//double click causes text to be selected.
		setTimeout(function () {
			$(selectors.editWrapper + " .form-control").get(0).focus();
		}, 200);
	});
	//Disable inline edit action
	$(selectors.editWrapper + " .cancel").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		DateTimeInlineEditPreDisableCallback(fieldId, fieldName, config);
	});
	//Save inline changes
	$(selectors.editWrapper + " .save").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		var inputValue = $(selectors.editWrapper + " .form-control").val();
		if (!inputValue) {
			inputValue = null;
		}
		else if (!moment(inputValue).isValid()) {
			toastr.error("invalid date", 'Error!', { closeButton: true, tapToDismiss: true });
			return;
		}

		var submitObj = {};
		submitObj[fieldName] = inputValue;
		$(selectors.editWrapper + " .save .fa").removeClass("fa-check").addClass("fa-spin fa-spinner");
		$(selectors.editWrapper + " .save").attr("disabled", true);
		$(selectors.editWrapper + " .invalid-feedback").remove();
		$(selectors.editWrapper + " .form-control").removeClass("is-invalid");
		var apiUrl = config.api_url
		$.ajax({
			headers: {
				'Accept': 'application/json',
				'Content-Type': 'application/json'
			},
			url: apiUrl,
			type: 'PATCH',
			data: JSON.stringify(submitObj),
			success: function (response) {
				if (response.success) {
					DateTimeInlineEditInitSuccessCallback(response, fieldId, fieldName, config);
				}
				else {
					DateTimeInlineEditInitErrorCallback(response, fieldId, fieldName, config);
				}
			},
			error: function (jqXHR, textStatus, errorThrown) {
				var response = {};
				response.message = "";
				if (jqXHR && jqXHR.responseJSON) {
					response = jqXHR.responseJSON;
				}
				DateTimeInlineEditInitErrorCallback(response, fieldId, fieldName, config);
			}
		});
	});
}

function DateTimeInlineEditInitSuccessCallback(response, fieldId, fieldName, config) {
	var selectors = DateTimeInlineEditGenerateSelectors(fieldId, fieldName, config);
	var newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
	if (newValue !== null) {
		var formatedDate = moment(newValue).format("DD MMM YYYY HH:mm");
		$(selectors.viewWrapper + " .form-control").html(formatedDate);
	}
	else {
		$(selectors.viewWrapper + " .form-control").html(newValue);
	}
	$(selectors.viewWrapper + " .form-control").attr("title", newValue);
	$(selectors.editWrapper + " .form-control").val(newValue);
	$(selectors.editWrapper).attr("data-default-date", newValue);
	DateTimeInlineEditPreDisableCallback(fieldId, fieldName, config);
	toastr.success("The new value is successfully saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function DateTimeInlineEditInitErrorCallback(response, fieldId, fieldName, config) {
	var selectors = DateTimeInlineEditGenerateSelectors(fieldId, fieldName, config);
	$(selectors.editWrapper + " .form-control").addClass("is-invalid");
	var errorMessage = response.message;
	if (!errorMessage && response.errors && response.errors.length > 0) {
		errorMessage = response.errors[0].message;
	}

	$(selectors.editWrapper + " .input-group").after("<div class='invalid-feedback'>" + errorMessage + "</div>");
	$(selectors.editWrapper + " .invalid-feedback").show();
	$(selectors.editWrapper + " .save .fa").addClass("fa-check").removeClass("fa-spin fa-spinner");
	$(selectors.editWrapper + " .save").attr("disabled", false);
	toastr.error("An error occurred", 'Error!', { closeButton: true, tapToDismiss: true });
	console.log("error", response);
}