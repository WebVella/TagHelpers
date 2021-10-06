
function SelectInlineEditGenerateSelectors(fieldId, fieldName, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.viewWrapper = "#view-" + fieldId;
	selectors.editWrapper = "#edit-" + fieldId;
	selectors.inputEl = "#input-" + fieldId;
	selectors.viewOptionsListUl = selectors.viewWrapper + " .select2-selection__rendered";
	selectors.editWrapper = "#edit-" + fieldId;
	return selectors;
}

function SelectInlineEditFormat(record) {
	var originalOption = record.element;
	//Non API
	if (originalOption) {
		var iconClass = $(originalOption).data('icon');
		var color = $(originalOption).data('color');
		if (!color) {
			color = "#999";
		}
		if (!iconClass) {
			return record.text;
		}
		return '<i class="fa fa-fw ' + iconClass + '" style="color:' + color + '"></i> ' + record.text;
	}
	//API
	else if (record.icon_class) {
		return '<i class="fa fa-fw ' + record.icon_class + '" style="color:' + record.color + '"></i> ' + record.text;
	}
	else {
		return record.text;
	}
}

function SelectInlineEditMatchStartsWith(params, data) {
	// If there are no search terms, return all of the data
	if ($.trim(params.term) === '') {
		return data;
	}

	// Do not display the item if there is no 'text' property
	if (typeof data.text === 'undefined') {
		return null;
	}

	// `params.term` should be the term that is used for searching
	// `data.text` is the text that is displayed for the data object
	if (data.text.toLowerCase().startsWith(params.term.toLowerCase())) {
		var modifiedData = $.extend({}, data, true);
		//		modifiedData.text += ' (matched)';

		// You can return modified objects from here
		// This includes matching the `children` how you want in nested data sets
		return modifiedData;
	}

	// Return `null` if the term should not be displayed
	return null;
}

function SelectInlineEditPreEnableCallback(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = SelectInlineEditGenerateSelectors(fieldId, fieldName, config);

	var placeholder = 'not selected';
	if (config.placeholder) {
		placeholder = config.placeholder;
	}

	var selectInitObject = {
		theme: 'bootstrap4',
		closeOnSelect: true,
		language: "en",
		minimumResultsForSearch: 10,
		placeholder: placeholder,
		allowClear: !$(selectors.inputEl).prop('required'),
		width: 'element',
		escapeMarkup: function (markup) {
			return markup;
		},
		templateResult: SelectInlineEditFormat,
		templateSelection: SelectInlineEditFormat
	};

	if (config.select_match_type === 1) {
		selectInitObject.matcher = SelectInlineEditMatchStartsWith;
	}

	if (config.ajax_datasource) {
		var currentPage = 1;
		selectInitObject.ajax = {
			type: 'POST',
			headers: {
				'Accept': 'application/json',
				'Content-Type': 'application/json'
			},
			url: config.ajax_datasource_api,
			data: function (params) {
				var query = {
					name: config.ajax_datasource.ds,
					parameters: [
						{
							name: "term",
							value: params.term || ""
						},
						{
							name: "page",
							value: params.page || 1
						}
					]
				};
				currentPage = params.page || 1;
				return JSON.stringify(query);
			},
			processResults: function (data) {
				var results = [];
				var hasMore = false;
				if (data && data.object && data.object.list) {
					var totalRecords = data.object.total_count;
					var displayedCount = data.object.list.length + (currentPage - 1) * config.ajax_datasource.page_size;
					if (displayedCount < totalRecords) {
						hasMore = true;
					}
					_.forEach(data.object.list, function (record) {
						var result = {};
						if (record[config.ajax_datasource.value]) {
							result.id = record[config.ajax_datasource.value];
						}
						else {
							result.id = null;
						}
						if (config.ajax_datasource.label.length > 0) {
							if (config.ajax_datasource.label.includes("{{") && config.ajax_datasource.label.includes("}}")) {
								result.text = WebVellaTagHelpers.ProcessStringTemplateWithObject(config.ajax_datasource.label, record);
							}
							else if (record.hasOwnProperty(config.ajax_datasource.label)) {
								result.text = record[config.ajax_datasource.label];
							}
							else {
								result.text = "!undefined!";
							}
						}
						else {
							result.text = "!undefined!";
						}
						if (record["icon_class"]) {
							result.icon_class = record["icon_class"];
						}
						else {
							result.icon_class = "";
						}
						if (record["color"]) {
							result.color = record["color"];
						}
						else {
							result.color = "";
						}
						results.push(result);
					});
					return {
						results: results, //id,text
						pagination: {
							more: hasMore
						}
					};
				}
				else if (data && data.object) {
					_.forEach(data.object, function (record) {
						var result = {};
						if (record[config.ajax_datasource.value]) {
							result.id = record[config.ajax_datasource.value];
						}
						else {
							result.id = null;
						}
						if (config.ajax_datasource.label.length > 0) {
							if (config.ajax_datasource.label.includes("{{") && config.ajax_datasource.label.includes("}}")) {
								result.text = WebVellaTagHelpers.ProcessStringTemplateWithObject(config.ajax_datasource.label, record);
							}
							else if (record.hasOwnProperty(config.ajax_datasource.label)) {
								result.text = record[config.ajax_datasource.label];
							}
							else {
								result.text = "!undefined!";
							}
						}
						else {
							result.text = "!undefined!";
						}
						if (record["icon_class"]) {
							result.icon_class = record["icon_class"];
						}
						else {
							result.icon_class = "";
						}
						if (record["color"]) {
							result.color = record["color"];
						}
						else {
							result.color = "";
						}
						results.push(result);
					});
					return {
						results: results, //id,text
						pagination: {
							more: hasMore
						}
					};
				}

				return data;
			}
		};
	}
	else if (config.ajax_datasource_api) {
		selectInitObject.ajax = {
			type: 'POST',
			headers: {
				'Accept': 'application/json',
				'Content-Type': 'application/json'
			},
			url: config.ajax_datasource_api,
			data: function (params) {
				var query = {
					term: params.term || "",
					page: params.page || 1
				};
				return JSON.stringify(query);
			}
		};
	}

	$(selectors.inputEl).select2(selectInitObject);
	if (config.is_invalid) {
		$(selectors.inputEl).closest(".input-group").find(".select2-selection").addClass("is-invalid");
	}
	$(selectors.viewWrapper).hide();
	$(selectors.editWrapper).show();
	$(selectors.inputEl).focus();
}

function SelectInlineEditPreDisableCallback(fieldId, fieldName, config) {
	var selectors = SelectInlineEditGenerateSelectors(fieldId, fieldName, config);
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

function SelectInlineEditInit(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = SelectInlineEditGenerateSelectors(fieldId, fieldName, config);
	//Init enable action click
	$(selectors.viewWrapper + " .action .btn").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		SelectInlineEditPreEnableCallback(fieldId, fieldName, config);
	});
	//Init enable action dblclick
	$(selectors.viewWrapper + " .form-control").on("dblclick", function (event) {
		event.stopPropagation();
		event.preventDefault();
		SelectInlineEditPreEnableCallback(fieldId, fieldName, config);
		//WebVellaTagHelpers.clearSelection();//double click causes text to be selected.
		setTimeout(function () {
			$(selectors.editWrapper + " .form-control").get(0).focus();
		}, 200);
	});
	//Disable inline edit action
	$(selectors.editWrapper + " .cancel").on("click", function (event) {
		event.stopPropagation();
		event.preventDefault();
		SelectInlineEditPreDisableCallback(fieldId, fieldName, config);
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
					SelectInlineEditInitSuccessCallback(response, fieldId, fieldName, inputValue, config);
				}
				else {
					SelectInlineEditInitErrorCallback(response, fieldId, fieldName, config);
				}
			},
			error: function (jqXHR, textStatus, errorThrown) {
				var response = {};
				response.message = "";
				if (jqXHR && jqXHR.responseJSON) {
					response = jqXHR.responseJSON;
				}
				SelectInlineEditInitErrorCallback(response, fieldId, fieldName, config);
			}
		});
	});
}

function SelectInlineEditInitSuccessCallback(response, fieldId, fieldName, inputValue, config) {
	var selectors = SelectInlineEditGenerateSelectors(fieldId, fieldName, config);
	var newValue = inputValue;

	if (!fieldName.startsWith("$")) {
		newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
	}

	if (newValue) {
		var selectOptions = $(selectors.inputEl + ' option');
		var matchedOption = _.find(selectOptions, function (record) {
			if (!newValue && (!record.attributes["value"] || !record.attributes["value"].value)) {
				return true;
			}
			else if (record.attributes["value"] && record.attributes["value"].value) {
				return newValue === record.attributes["value"].value;
			}
			return false;
		});

		var optionLabel = newValue;
		if(matchedOption && matchedOption.text && matchedOption.text.length > 0){
			optionLabel = matchedOption.text;
		}
		var iconClass = $(matchedOption).data('icon');
		var color = $(matchedOption).data('color');
		if (!color) {
			color = "#999";
		}
		if (!iconClass) {
			$(selectors.viewWrapper + " .form-control").html(optionLabel);
		}
		else {
			$(selectors.viewWrapper + " .form-control").html('<i class="fa ' + iconClass + '" style="color:' + color + '"></i>  ' + optionLabel);
		}
	}
	else {
		$(selectors.viewWrapper + " .form-control").html("");
	}


	$(selectors.inputEl).val(newValue).attr("data-original-value", JSON.stringify(newValue));
	SelectInlineEditPreDisableCallback(fieldId, fieldName, config);
	toastr.success("The new value is successful saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function SelectInlineEditInitErrorCallback(response, fieldId, fieldName, config) {
	var selectors = SelectInlineEditGenerateSelectors(fieldId, fieldName, config);
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


