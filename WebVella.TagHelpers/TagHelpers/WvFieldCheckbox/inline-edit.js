function CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.viewWrapper = "#view-" + fieldId;
	selectors.editWrapper = "#edit-" + fieldId;
	return selectors;
}

function CheckboxInlineEditPreEnableCallback(fieldId, fieldName, config) {
	var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
	$(selectors.viewWrapper).hide();
	$(selectors.editWrapper).show();
}

function CheckboxInlineEditPreDisableCallback(fieldId, fieldName, config) {
	var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
	$(selectors.editWrapper + " .invalid-feedback").remove();
	$(selectors.editWrapper + " .form-control").removeClass("is-invalid");
	$(selectors.editWrapper + " .save .fa").addClass("fa-check").removeClass("fa-spin fa-spinner");
	$(selectors.editWrapper + " .save").attr("disabled", false);
	$(selectors.viewWrapper).show();
	$(selectors.editWrapper).hide();
}

function CheckboxInlineEditInit(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
	//Init enable action click
	document.querySelector(selectors.viewWrapper + " .action .btn").addEventListener("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		CheckboxInlineEditPreEnableCallback(fieldId, fieldName, config);
	});
	//Init enable action dblclick
	document.querySelector(selectors.viewWrapper + " .form-control").addEventListener("dblclick", function (event) {
		event.stopPropagation();
		event.preventDefault();
		CheckboxInlineEditPreEnableCallback(fieldId, fieldName, config);
		//WebVellaTagHelpers.clearSelection();//double click causes text to be selected.
		//setTimeout(function () {
		//	$(selectors.editWrapper + " .form-control").get(0).focus();
		//}, 200);
	});
	//Disable inline edit action
	document.querySelector(selectors.editWrapper + " .cancel").addEventListener("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		CheckboxInlineEditPreDisableCallback(fieldId, fieldName, config);
	});

	//Save inline changes
	document.querySelector(selectors.editWrapper + " .save").addEventListener("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		var inputValue = false;
		if ($(selectors.editWrapper + " .form-check-input").is(':checked')) {
			inputValue = true;
		}
		var submitObj = {};

		submitObj[fieldName] = inputValue;
		$(selectors.editWrapper + " .save .fa").removeClass("fa-check").addClass("fa-spin fa-spinner");
		$(selectors.editWrapper + " .save").attr("disabled", true);
		$(selectors.editWrapper + " .invalid-feedback").remove();
		$(selectors.editWrapper + " .form-control").removeClass("is-invalid");
		var apiUrl = config.api_url;

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
					CheckboxInlineEditInitSuccessCallback(response, fieldId, fieldName, config);
				}
				else {
					CheckboxInlineEditInitErrorCallback(response, fieldId, fieldName, config);
				}
			},
			error: function (jqXHR, textStatus, errorThrown) {
				var response = {};
				response.message = "";
				if (jqXHR && jqXHR.responseJSON) {
					response = jqXHR.responseJSON;
				}
				CheckboxInlineEditInitErrorCallback(response, fieldId, fieldName, config);
			}
		});
	});
}

function CheckboxInlineEditInitSuccessCallback(response, fieldId, fieldName, config) {
	var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
	var newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
	if (newValue === null) {
		$(selectors.viewWrapper + " .input-group-prepend .fa").removeClass("fa-check fa-question fa-times").addClass("fa-question");
		$(selectors.viewWrapper + " .form-control").html("");
		$("#edit-" + fieldId + " .form-check-input").prop('checked', false);
	}
	else if (newValue) {
		$(selectors.viewWrapper + " .input-group-prepend .fa").removeClass("fa-check fa-question fa-times").addClass("fa-check");
		$(selectors.viewWrapper + " .form-control").html(config.true_label);
		$(selectors.editWrapper + " .form-check-input").prop('checked', true);
	}
	else {
		$(selectors.viewWrapper + " .input-group-prepend .fa").removeClass("fa-check fa-question fa-times").addClass("fa-times");
		$(selectors.viewWrapper + " .form-control").html(config.false_label);
		$(selectors.editWrapper + " .form-check-input").prop('checked', false);
	}
	CheckboxInlineEditPreDisableCallback(fieldId, fieldName, config);
	toastr.success("The new value is successfully saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function CheckboxInlineEditInitErrorCallback(response, fieldId, fieldName, config) {
	var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
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
