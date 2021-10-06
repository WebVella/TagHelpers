
function SelectFormGenerateSelectors(fieldId, fieldName, config) {
    //Method for generating selector strings of some of the presentation elements
    var selectors = {};
    if (!config.prefix || config === "") {
        selectors.inputEl = "#input-" + fieldId;
    }
    else {
        selectors.inputEl = "#input-" + config.prefix + "-" + fieldId;
    }
    selectors.modalEl = "#add-option-modal-" + fieldId;
    selectors.primaryBtnEl = "#add-option-modal-" + fieldId + " .btn-primary";
    selectors.modalFormEl = "#add-option-form-" + fieldId;
    return selectors;
}

function SelectFormFormat(record) {
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

function SelectFormMatchStartsWith(params, data) {
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

function SelectFormInit(fieldId, fieldName, config) {
    config = WebVellaTagHelpers.ProcessConfig(config);
    var selectors = SelectFormGenerateSelectors(fieldId, fieldName, config);

    var placeholder = 'not selected';
    if (config.placeholder) {
        placeholder = config.placeholder;
    }

    var selectInitObject = {
        theme: 'bootstrap4',
        language: "en",
        minimumResultsForSearch: 10,
        closeOnSelect: true,
        placeholder: placeholder,
        allowClear: !$(selectors.inputEl).prop('required'),
        width: 'element',
        escapeMarkup: function (markup) {
            return markup;
        },
        templateResult: SelectFormFormat,
        templateSelection: SelectFormFormat
    };

    if (config.select_match_type === 1) {
        selectInitObject.matcher = SelectFormMatchStartsWith;
    }

    //Datasource is provided
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
    //Only API url is provided
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

    $(selectors.inputEl).select2(selectInitObject).on("select2:unselecting", function (e) {
        $(this).data('state', 'unselected');
    }).on("select2:open", function (e) {
        if ($(this).data('state') === 'unselected') {
            $(this).removeData('state');
            var self = $(this);
            setTimeout(function () {
                self.select2('close');
            }, 1);
        }
    });
    if (config.is_invalid) {
        $(selectors.inputEl).closest(".input-group").find(".select2-selection").addClass("is-invalid");
    }

    //COnfig-a dolu si stoi na posledno otvorenia realno a ne na tozi kum koito purvonachalno e bil initnat tozi event. Stranno ..
    //$(selectors.inputEl).on('select2:open', function () {
    //	var appendLinkString = "<a href=\"javascript:void(0)\" onclick=\"addOptionModal('" + fieldId + "','" + fieldName + "','" + entityName + "','" + config.prefix + "')\" class=\"select2-add-option\"><i class=\"fa fa-plus-circle\"></i> create new record</a>";
    //	if (config && config.can_add_values) {
    //		$(".select2-results:not(:has(a))").append(appendLinkString);
    //	}
    //});

    $(selectors.inputEl).on('change', function (event) {
        var customEvent = new Event('WvFieldSelect_Change');
        var inputElement = document.getElementById('input-' + fieldId);
        var selectedJson = $(selectors.inputEl).select2('data');
        var selectedKey = null;
        if (selectedJson.length > 0) {
            selectedKey = selectedJson[0].id; //this is a single select
        }
        customEvent.payload = {
            value: selectedKey,
            fieldId: fieldId,
            fieldName: inputElement.name
        };
        document.dispatchEvent(customEvent);
    });

    $(selectors.modalEl).appendTo("body");
    var $form = $('<form id="add-option-modal-' + fieldId + '" name="add-option-modal-' + fieldId + '"></form>');
    $(selectors.modalEl + " .modal-dialog").append($form);
    $(selectors.modalEl + " .modal-content").appendTo(selectors.modalEl + " form");
    $(selectors.modalEl + " form").on("submit", function (event) {
        event.preventDefault();
        var $alertMessage = $(selectors.modalEl).find(".alert-danger");
        $($alertMessage).addClass("d-none").html("");
        var inputValue = $(selectors.modalEl).find(".add-option-input").val();
        if (!WebVellaTagHelpers.isStringNullOrEmptyOrWhiteSpace(inputValue)) {
            var submitObj = {};
            submitObj.value = inputValue;
            submitObj.entityName = entityName;
            submitObj.fieldName = fieldName;
            $(selectors.modalEl).find(".btn-primary").attr("disabled", "disabled").find(".fa").removeClass("fa-plus-circle").addClass("fa-spin fa-spinner");
            $.ajax({
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                type: "PUT",
                url: '/api/v3.0/p/core/select-field-add-option',
                data: JSON.stringify(submitObj),
                success: function (response) {
                    if (response.success) {
                        addOptionSuccessCallback(response, fieldId, fieldName, entityName, inputValue, config.prefix);
                    }
                    else {
                        addOptionErrorCallback(response, fieldId, fieldName, entityName, inputValue, config.prefix);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var response = {};
                    response.message = "";
                    if (jqXHR && jqXHR.responseJSON) {
                        response = jqXHR.responseJSON;
                    }
                    addOptionErrorCallback(response, fieldId, fieldName, entityName, inputValue, config.prefix);
                }
            });
        }
        else {
            $($alertMessage).html("Required field").removeClass("d-none");
        }
    });
}

function addOptionSuccessCallback(response, fieldId, fieldName, entityName, inputValue, prefix) {
    var selectorInputEl = "";

    if (!prefix || prefix === "") {
        selectorInputEl = "#input-" + fieldId;
    }
    else {
        selectorInputEl = "#input-" + prefix + "-" + fieldId;
    }
    var selectorModalEl = "#add-option-modal-" + fieldId;
    var newOption = new Option(inputValue, inputValue, false, false);
    $(selectorInputEl).append(newOption);
    $(selectorInputEl).select2().val(inputValue).trigger('change');
    $(selectorModalEl).modal('hide');
}

function addOptionErrorCallback(response, fieldId, fieldName, entityName, inputValue, prefix) {
    var selectorInputEl = "";

    if (!prefix || prefix === "") {
        selectorInputEl = "#input-" + fieldId;
    }
    else {
        selectorInputEl = "#input-" + prefix + "-" + fieldId;
    }
    var selectorModalEl = "#add-option-modal-" + fieldId;
    var $alertMessage = $(selectorModalEl).find(".alert-danger");
    $(selectorModalEl).find(".btn-primary").removeAttr("disabled", "disabled").find(".fa").addClass("fa-plus-circle").removeClass("fa-spin fa-spinner");
    $($alertMessage).html(response.message).removeClass("d-none");
}

function addOptionModal(fieldId, fieldName, entityName, prefix) {
    var selectorInputEl = "";

    if (!prefix || prefix === "") {
        selectorInputEl = "#input-" + fieldId;
    }
    else {
        selectorInputEl = "#input-" + prefix + "-" + fieldId;
    }


    var selectorModalEl = "#add-option-modal-" + fieldId;
    var $alertMessage = $(selectorModalEl).find(".alert-danger");
    $($alertMessage).addClass("d-none").html("");

    $(selectorModalEl).on('shown.bs.modal', function () {
        $(selectorModalEl).find(".add-option-input").val("");
        $(selectorModalEl).find(".btn-primary").removeAttr("disabled", "disabled").find(".fa").addClass("fa-plus-circle").removeClass("fa-spin fa-spinner");
        $(selectorInputEl).select2("close");
        $('.add-option-input').trigger('focus');
    });

    $(selectorModalEl).modal({
        focus: false
    });
}
