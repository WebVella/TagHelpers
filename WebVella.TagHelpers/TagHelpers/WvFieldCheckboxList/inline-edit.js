function CheckboxListInlineEditGenerateSelectors(fieldId, fieldName, config) {
    //Method for generating selector strings of some of the presentation elements
    var selectors = {};
    selectors.viewWrapper = "#view-" + fieldId;
    selectors.editWrapper = "#edit-" + fieldId;
    return selectors;
}

function CheckboxListInlineEditPreEnableCallback(fieldId, fieldName, config) {
    var selectors = CheckboxListInlineEditGenerateSelectors(fieldId, fieldName, config);
    var viewEl = document.querySelector(selectors.viewWrapper);
    var editEl = document.querySelector(selectors.editWrapper);
    if (viewEl) viewEl.style.display = "none";
    if (editEl) editEl.style.display = "block";
}

function CheckboxListInlineEditPreDisableCallback(fieldId, fieldName, config) {
    var selectors = CheckboxListInlineEditGenerateSelectors(fieldId, fieldName, config);
    document.querySelectorAll(selectors.editWrapper + " .invalid-feedback").forEach(function (el) {
        el.remove();
    });
    document.querySelectorAll(selectors.editWrapper + " .form-control").forEach(function (el) {
        el.classList.remove("is-invalid");
    });
    document.querySelectorAll(selectors.editWrapper + " .save .fa").forEach(function (el) {
        el.classList.add("fa-check");
        el.classList.remove("fa-spin", "fa-spinner");
    });
    document.querySelectorAll(selectors.editWrapper + " .save").forEach(function (el) {
        el.disabled = false;
    });
    var viewEl = document.querySelector(selectors.viewWrapper);
    var editEl = document.querySelector(selectors.editWrapper);
    if (viewEl) viewEl.style.display = "block";
    if (editEl) editEl.style.display = "none";
}

function CheckboxListInlineEditInit(fieldId, fieldName, config) {
    config = WebVellaTagHelpers.ProcessConfig(config);
    var selectors = CheckboxListInlineEditGenerateSelectors(fieldId, fieldName, config);
    
    //Init enable action click
    document.querySelectorAll(selectors.viewWrapper + " .action .btn").forEach(function (el) {
        el.addEventListener("click", function (event) {
            event.stopPropagation();
            event.preventDefault();
            CheckboxListInlineEditPreEnableCallback(fieldId, fieldName, config);
        });
    });

    //Init enable action dblclick
    document.querySelectorAll(selectors.viewWrapper + " .form-control").forEach(function (el) {
        el.addEventListener("dblclick", function (event) {
            event.stopPropagation();
            event.preventDefault();
            CheckboxListInlineEditPreEnableCallback(fieldId, fieldName, config);
        });
    });

    //Disable inline edit action
    document.querySelectorAll(selectors.editWrapper + " .cancel").forEach(function (el) {
        el.addEventListener("click", function (event) {
            event.stopPropagation();
            event.preventDefault();
            CheckboxListInlineEditPreDisableCallback(fieldId, fieldName, config);
        });
    });

    //Save inline changes
    document.querySelectorAll(selectors.editWrapper + " .save").forEach(function (el) {
        el.addEventListener("click", function (event) {
            event.stopPropagation();
            event.preventDefault();
            var inputValueArray = [];
            document.querySelectorAll(selectors.editWrapper + " .form-check-input").forEach(function (element) {
                if (element.checked) {
                    inputValueArray.push(element.value);
                }
            });

            var submitObj = {};
            submitObj[fieldName] = _.join(inputValueArray, ',');
            
            document.querySelectorAll(selectors.editWrapper + " .save .fa").forEach(function (el) {
                el.classList.remove("fa-check");
                el.classList.add("fa-spin", "fa-spinner");
            });
            document.querySelectorAll(selectors.editWrapper + " .save").forEach(function (el) {
                el.disabled = true;
            });
            document.querySelectorAll(selectors.editWrapper + " .invalid-feedback").forEach(function (el) {
                el.remove();
            });
            document.querySelectorAll(selectors.editWrapper + " .form-control").forEach(function (el) {
                el.classList.remove("is-invalid");
            });

            var apiUrl = config.api_url;
            fetch(apiUrl, {
                method: 'PATCH',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(submitObj)
            })
            .then(function (response) {
                return response.json().then(function (data) {
                    if (response.ok) {
                        if (data && data.success) {
                            CheckboxListInlineEditInitSuccessCallback(data, fieldId, fieldName, config);
                        } else {
                            CheckboxListInlineEditInitErrorCallback(data, fieldId, fieldName, config);
                        }
                    } else {
                        CheckboxListInlineEditInitErrorCallback(data || {}, fieldId, fieldName, config);
                    }
                }).catch(function () {
                    CheckboxListInlineEditInitErrorCallback({ message: "" }, fieldId, fieldName, config);
                });
            })
            .catch(function (error) {
                CheckboxListInlineEditInitErrorCallback({ message: "" }, fieldId, fieldName, config);
            });
        });
    });
}

function CheckboxListInlineEditInitSuccessCallback(response, fieldId, fieldName, config) {
    var selectors = CheckboxListInlineEditGenerateSelectors(fieldId, fieldName, config);
    var newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
    if (newValue) {
        document.querySelectorAll(selectors.viewWrapper + " .input-group-prepend .fa").forEach(function (el) {
            el.classList.remove("fa-check", "fa-question", "fa-times");
            el.classList.add("fa-check");
        });
        document.querySelectorAll(selectors.viewWrapper + " .form-control").forEach(function (el) {
            el.innerHTML = newValue;
        });
        var valueArray = _.split(newValue, ',');
        document.querySelectorAll("#edit-" + fieldId + " .form-check-input").forEach(function (element) {
            var elValue = element.value;
            var valueIndex = _.findIndex(valueArray, function (record) { return record === elValue; });
            element.checked = valueIndex > -1;
        });
    }
    else {
        document.querySelectorAll(selectors.viewWrapper + " .input-group-prepend .fa").forEach(function (el) {
            el.classList.remove("fa-check", "fa-question", "fa-times");
            el.classList.add("fa-times");
        });
        document.querySelectorAll(selectors.viewWrapper + " .form-control").forEach(function (el) {
            el.innerHTML = "";
        });
        document.querySelectorAll("#edit-" + fieldId + " .form-check-input").forEach(function (element) {
            element.checked = false;
        });
    }
    CheckboxListInlineEditPreDisableCallback(fieldId, fieldName, config);
    toastr.success("The new value is successfully saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function CheckboxListInlineEditInitErrorCallback(response, fieldId, fieldName, config) {
    var selectors = CheckboxListInlineEditGenerateSelectors(fieldId, fieldName, config);
    document.querySelectorAll(selectors.editWrapper + " .form-control").forEach(function (el) {
        el.classList.add("is-invalid");
    });
    var errorMessage = response.message;
    if (!errorMessage && response.errors && response.errors.length > 0) {
        errorMessage = response.errors[0].message;
    }
        
    document.querySelectorAll(selectors.editWrapper + " .input-group").forEach(function (el) {
        el.insertAdjacentHTML('afterend', "<div class='invalid-feedback'>" + errorMessage + "</div>");
    });
    document.querySelectorAll(selectors.editWrapper + " .invalid-feedback").forEach(function (el) {
        el.style.display = "block";
    });
    document.querySelectorAll(selectors.editWrapper + " .save .fa").forEach(function (el) {
        el.classList.add("fa-check");
        el.classList.remove("fa-spin", "fa-spinner");
    });
    document.querySelectorAll(selectors.editWrapper + " .save").forEach(function (el) {
        el.disabled = false;
    });
    toastr.error("An error occurred", 'Error!', { closeButton: true, tapToDismiss: true });
    console.log("error", response);
}