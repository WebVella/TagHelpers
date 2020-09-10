
function IconFieldInlineEditGenerateSelectors(fieldId, fieldName,config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.viewWrapper = "#view-" + fieldId;
	selectors.editWrapper = "#edit-" + fieldId;
	selectors.inputEl = "#input-" + fieldId;
	selectors.viewOptionsListUl = selectors.viewWrapper + " .select2-selection__rendered";
	selectors.editWrapper = "#edit-" + fieldId;
	return selectors;
}

function IconFieldInlineEditFormat(icon) {
	var originalOption = icon.element;
	var iconClass = $(originalOption).data('icon');
	var color = $(originalOption).data('color');
	if (!color) {
		color = "#999";
	}
	if (!iconClass) {
		return icon.text;
	}
	return '<i class="fa ' + iconClass + '" style="color:' + color + '"></i> ' + icon.text;
}

function IconFieldInlineEditPreEnableCallback(fieldId, fieldName,config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = IconFieldInlineEditGenerateSelectors(fieldId, fieldName,config);

	var wvIconOptions = [];
	_.forEach(WvFontAwesomeIcons,function(rec){
		wvIconOptions.push({id:rec.class,text:rec.class,name:rec.name});
	});


	var selectInitObject = {
		data:wvIconOptions,
		//language: "bg",
		theme: 'bootstrap4',
		placeholder: 'not-selected',
		allowClear: !$(selectors.inputControl).prop('required'),
		closeOnSelect: true,
		width: 'element',
		escapeMarkup: function (markup) {
			return markup;
		},
		templateResult: function (state) {
			var $state = $(
				'<div class="erp-ta-icon-result"><div class="icon-wrapper"><i class="icon fa-fw ' + state.id + '"/></div><div class="meta"><div class="title">' + state.id + '</div><div class="entity go-gray">' + state.name + '</div></div>'
			);
			return $state;
		}
	};

	$(selectors.inputEl).select2(selectInitObject);
	if (config.is_invalid) {
		$(selectors.inputEl).closest(".input-group").find(".select2-selection").addClass("is-invalid");
	}

	$(selectors.inputEl).on('select2:select', function (e) {
		
		$(selectors.inputEl).closest(".input-group").find(".input-group-prepend .fa-fw").attr("class","fa-fw " + e.target.value);
	});

	$(selectors.inputEl).on("select2:unselecting", function (e) {
		$(this).data('state', 'unselected');
	}).on("select2:open", function (e) {
		if ($(this).data('state') === 'unselected') {
			$(this).removeData('state');
			$(selectors.inputEl).closest(".input-group").find(".input-group-prepend .fa-fw").attr("class","fa-fw ");
			var self = $(this);
			setTimeout(function () {
				self.select2('close');
			}, 0);
		}
	});

	$(selectors.viewWrapper).hide();
	$(selectors.editWrapper).show();
	$(selectors.inputEl).focus();
}

function IconFieldInlineEditPreDisableCallback(fieldId, fieldName,config) {
	var selectors = IconFieldInlineEditGenerateSelectors(fieldId, fieldName,config);
	$(selectors.editWrapper + " .invalid-feedback").remove();
	$(selectors.editWrapper + " .form-control").removeClass("is-invalid");
	$(selectors.editWrapper + " .save .fa").addClass("fa-check").removeClass("fa-spin fa-spinner");
	$(selectors.editWrapper + " .save").attr("disabled", false);
	var originalValue = $(selectors.inputEl).attr("data-original-value");
	originalValue = WebVellaTagHelpers.ProcessConfig(originalValue);
	$(selectors.inputEl).val(originalValue);
	$(selectors.inputEl).select2('destroy');
	$(selectors.viewWrapper).show();
	$(selectors.editWrapper).hide();
}

function IconFieldInlineEditInit(fieldId, fieldName,config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = IconFieldInlineEditGenerateSelectors(fieldId, fieldName,config);
	//Init enable action click
	$(selectors.viewWrapper + " .action .btn").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		IconFieldInlineEditPreEnableCallback(fieldId, fieldName,config);
	});
	//Init enable action dblclick
	$(selectors.viewWrapper + " .form-control").on("dblclick", function (event) {
		event.stopPropagation();
		event.preventDefault();
		IconFieldInlineEditPreEnableCallback(fieldId, fieldName,config);
		//WebVellaTagHelpers.clearSelection();//double click causes text to be selected.
		setTimeout(function () {
			$(selectors.editWrapper + " .form-control").get(0).focus();
		}, 200);
	});
	//Disable inline edit action
	$(selectors.editWrapper + " .cancel").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		IconFieldInlineEditPreDisableCallback(fieldId, fieldName,config);
	});
	//Save inline changes
	$(selectors.editWrapper + " .save").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		var inputValue = $(selectors.inputEl).val();
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
					IconFieldInlineEditInitSuccessCallback(response, fieldId, fieldName, inputValue, config);
				}
				else {
					IconFieldInlineEditInitErrorCallback(response, fieldId, fieldName,config);
				}
			},
			error: function (jqXHR, textStatus, errorThrown) {
				var response = {};
				response.message = "";
				if (jqXHR && jqXHR.responseJSON) {
					response = jqXHR.responseJSON;
				}
				IconFieldInlineEditInitErrorCallback(response, fieldId, fieldName,config);
			}
		});
	});
}

function IconFieldInlineEditInitSuccessCallback(response, fieldId, fieldName, inputValue, config) {
	var selectors = IconFieldInlineEditGenerateSelectors(fieldId, fieldName,config);
	var newValue = inputValue;

	if (!fieldName.startsWith("$")) {
		newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
	}

	$(selectors.viewWrapper + " .form-control").html('<i class="fa ' + newValue + '"></i>  ' + newValue);

	$(selectors.inputEl).val(newValue).attr("data-original-value", JSON.stringify(newValue));
	IconFieldInlineEditPreDisableCallback(fieldId, fieldName,config);
	toastr.success("The new value is successful saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function IconFieldInlineEditInitErrorCallback(response, fieldId, fieldName,config) {
	var selectors = IconFieldInlineEditGenerateSelectors(fieldId, fieldName,config);
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


