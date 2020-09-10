function ColorInlineEditGenerateSelectors(fieldId, fieldName, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.viewWrapper = "#view-" + fieldId;
	selectors.editWrapper = "#edit-" + fieldId;
	selectors.inputControl = "#input-" + fieldId;
	return selectors;
}

function ColorInlineEditInputInit(fieldId) {
	var selectors = ColorInlineEditGenerateSelectors(fieldId);

	$(selectors.inputControl).spectrum({
		showPaletteOnly: true,
		showPalette:true,
		allowEmpty: true,
		hideAfterPaletteSelect:true,
		preferredFormat: "hex",
		palette: [
			['#B71C1C', '#F44336', '#FFEBEE', '#880E4F','#E91E63','#FCE4EC','#4A148C','#9C27B0','#F3E5F5'],
			['#311B92', '#673AB7', '#EDE7F6', '#1A237E','#3F51B5','#E8EAF6','#0D47A1','#2196F3','#E3F2FD'],
			['#01579B', '#03A9F4', '#E1F5FE', '#006064','#00BCD4','#E0F7FA','#004D40','#009688','#E0F2F1'],
			['#1B5E20', '#4CAF50', '#E8F5E9', '#33691E','#8BC34A','#F1F8E9','#827717','#CDDC39','#F9FBE7'],
			['#F57F17', '#FFEB3B', '#FFFDE7', '#FF6F00','#FFC107','#FFF8E1','#E65100','#FF9800','#FFF3E0'],
			['#BF360C', '#FF5722', '#FBE9E7', '#3E2723','#795548','#EFEBE9','#212121','#9E9E9E','#FAFAFA'],
			['#CCCCCC', '#263238', '#607D8B', '#ECEFF1','#FFFFFF','#000000','transparent']
		],
		change: function(color) {
			if(color.getAlpha() === 0){
				$(selectors.inputControl).val(null).trigger("change");
			}
		}
	});
}

function ColorInlineEditPreEnableCallback(fieldId, fieldName, config) {
	var selectors = ColorInlineEditGenerateSelectors(fieldId, fieldName, config);
	ColorInlineEditInputInit(fieldId);
	$(selectors.viewWrapper).hide();
	$(selectors.editWrapper).show();
	$(selectors.editWrapper + " .form-control").focus();
}

function ColorInlineEditPreDisableCallback(fieldId, fieldName, config) {
	var selectors = ColorInlineEditGenerateSelectors(fieldId, fieldName, config);
	$(selectors.editWrapper + " .invalid-feedback").remove();
	$(selectors.editWrapper + " .form-control").removeClass("is-invalid");
	$(selectors.editWrapper + " .save .fa").addClass("fa-check").removeClass("fa-spin fa-spinner");
	$(selectors.editWrapper + " .save").attr("disabled", false);
	$(selectors.viewWrapper).show();
	$(selectors.editWrapper).hide();
}

function ColorInlineEditInit(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = ColorInlineEditGenerateSelectors(fieldId, fieldName, config);
	//Init enable action click
	$(selectors.viewWrapper + " .action .btn").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		ColorInlineEditPreEnableCallback(fieldId, fieldName, config);
	});
	//Init enable action dblclick
	$(selectors.viewWrapper + " .form-control").on("dblclick", function (event) {
		event.stopPropagation();
		event.preventDefault();
		ColorInlineEditPreEnableCallback(fieldId, fieldName, config);
		//WebVellaTagHelpers.clearSelection();//double click causes text to be selected.
		setTimeout(function () {
			$(selectors.editWrapper + " .form-control").get(0).focus();
		}, 200);
	});
	$(selectors.editWrapper + " .form-control").keypress(function (e) {
		if (e.which === 13) {
			$(selectors.editWrapper + " .save").click();
		}
	});

	//Disable inline edit action
	$(selectors.editWrapper + " .cancel").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		ColorInlineEditPreDisableCallback(fieldId, fieldName, config);
	});
	//Save inline changes
	$(selectors.editWrapper + " .save").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		var inputValue = $(selectors.editWrapper + " .form-control").val();
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
					ColorInlineEditInitSuccessCallback(response, fieldId, fieldName, config);
				}
				else {
					ColorInlineEditInitErrorCallback(response, fieldId, fieldName, config);
				}
			},
			error: function (jqXHR, textStatus, errorThrown) {
				var response = {};
				response.message = "";
				if (jqXHR && jqXHR.responseJSON) {
					response = jqXHR.responseJSON;
				}
				ColorInlineEditInitErrorCallback(response, fieldId, fieldName, config);
			}
		});
	});
}

function ColorInlineEditInitSuccessCallback(response, fieldId, fieldName, config) {
	var selectors = ColorInlineEditGenerateSelectors(fieldId, fieldName, config);
	var newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
	
	$(selectors.viewWrapper + " .form-control").html(newValue);
	$(selectors.editWrapper + " .form-control").val(newValue);
	ColorInlineEditPreDisableCallback(fieldId, fieldName, config);
	if(newValue){
		$(selectors.viewWrapper + " .fa-square").css("color",newValue);
	}
	else{
		$(selectors.viewWrapper + " .fa-square").css("color",null);
	}
	toastr.success("The new value is successfully saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function ColorInlineEditInitErrorCallback(response, fieldId, fieldName, config) {
	var selectors = ColorInlineEditGenerateSelectors(fieldId, fieldName, config);
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