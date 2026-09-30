function CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config) {
    //Method for generating selector strings of some of the presentation elements
    var selectors = {};
    selectors.viewWrapper = "#view-" + fieldId;
    selectors.editWrapper = "#edit-" + fieldId;
    return selectors;
}

function CheckboxInlineEditPreEnableCallback(fieldId, fieldName, config) {
    var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
    var viewEl = document.querySelector(selectors.viewWrapper);
    var editEl = document.querySelector(selectors.editWrapper);

    if (viewEl) {
        viewEl.style.display = "none";
    }
    if (editEl) {
        editEl.style.display = "";
    }
}

function CheckboxInlineEditPreDisableCallback(fieldId, fieldName, config) {
    var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
    var editEl = document.querySelector(selectors.editWrapper);
    var viewEl = document.querySelector(selectors.viewWrapper);

    if (editEl) {
        editEl.querySelectorAll(".invalid-feedback").forEach(function (el) {
            el.remove();
        });
        editEl.querySelectorAll(".form-control").forEach(function (el) {
            el.classList.remove("is-invalid");
        });
        editEl.querySelectorAll(".save .fa").forEach(function (el) {
            el.classList.add("fa-check");
            el.classList.remove("fa-spin", "fa-spinner");
        });
        editEl.querySelectorAll(".save").forEach(function (el) {
            el.disabled = false;
        });
        editEl.style.display = "none";
    }

    if (viewEl) {
        viewEl.style.display = "";
    }
}

function CheckboxInlineEditInit(fieldId, fieldName, config) {
    config = WebVellaTagHelpers.ProcessConfig(config);
    var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);

    //Init enable action click
    var viewBtn = document.querySelector(selectors.viewWrapper + " .action .btn");
    if (viewBtn) {
        viewBtn.addEventListener("click", function (event) {
            event.stopPropagation();
            event.preventDefault();
            CheckboxInlineEditPreEnableCallback(fieldId, fieldName, config);
        });
    }

    //Init enable action dblclick
    var viewFormControl = document.querySelector(selectors.viewWrapper + " .form-control");
    if (viewFormControl) {
        viewFormControl.addEventListener("dblclick", function (event) {
            event.stopPropagation();
            event.preventDefault();
            CheckboxInlineEditPreEnableCallback(fieldId, fieldName, config);
        });
    }

    //Disable inline edit action
    var editCancelBtn = document.querySelector(selectors.editWrapper + " .cancel");
    if (editCancelBtn) {
        editCancelBtn.addEventListener("click", function (event) {
            event.stopPropagation();
            event.preventDefault();
            CheckboxInlineEditPreDisableCallback(fieldId, fieldName, config);
        });
    }

    //Save inline changes
    var editSaveBtn = document.querySelector(selectors.editWrapper + " .save");
    if (editSaveBtn) {
        editSaveBtn.addEventListener("click", function (event) {
            event.stopPropagation();
            event.preventDefault();

            var editEl = document.querySelector(selectors.editWrapper);
            var checkInput = editEl ? editEl.querySelector(".form-check-input") : null;
            var inputValue = checkInput ? checkInput.checked : false;

            var submitObj = {};
            submitObj[fieldName] = inputValue;

            if (editEl) {
                editEl.querySelectorAll(".save .fa").forEach(function (el) {
                    el.classList.remove("fa-check");
                    el.classList.add("fa-spin", "fa-spinner");
                });
                editEl.querySelectorAll(".save").forEach(function (el) {
                    el.disabled = true;
                });
                editEl.querySelectorAll(".invalid-feedback").forEach(function (el) {
                    el.remove();
                });
                editEl.querySelectorAll(".form-control").forEach(function (el) {
                    el.classList.remove("is-invalid");
                });
            }

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
                            CheckboxInlineEditInitSuccessCallback(data, fieldId, fieldName, config);
                        } else {
                            CheckboxInlineEditInitErrorCallback(data, fieldId, fieldName, config);
                        }
                    } else {
                        CheckboxInlineEditInitErrorCallback(data || {}, fieldId, fieldName, config);
                    }
                }).catch(function () {
                    CheckboxInlineEditInitErrorCallback({ message: "" }, fieldId, fieldName, config);
                });
            })
            .catch(function (error) {
                CheckboxInlineEditInitErrorCallback({ message: "" }, fieldId, fieldName, config);
            });
        });
    }
}

function CheckboxInlineEditInitSuccessCallback(response, fieldId, fieldName, config) {
    var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
    var newValue = WebVellaTagHelpers.ProcessNewValue(response, fieldName);
    var viewEl = document.querySelector(selectors.viewWrapper);
    var editEl = document.querySelector(selectors.editWrapper);

    var viewIcon = viewEl ? viewEl.querySelector(".input-group-prepend .fa") : null;
    var viewFormControl = viewEl ? viewEl.querySelector(".form-control") : null;
    var editCheckInput = editEl ? editEl.querySelector(".form-check-input") : null;

    if (newValue === null) {
        if (viewIcon) {
            viewIcon.classList.remove("fa-check", "fa-question", "fa-times");
            viewIcon.classList.add("fa-question");
        }
        if (viewFormControl) {
            viewFormControl.innerHTML = "";
        }
        if (editCheckInput) {
            editCheckInput.checked = false;
        }
    }
    else if (newValue) {
        if (viewIcon) {
            viewIcon.classList.remove("fa-check", "fa-question", "fa-times");
            viewIcon.classList.add("fa-check");
        }
        if (viewFormControl) {
            viewFormControl.innerHTML = config.true_label;
        }
        if (editCheckInput) {
            editCheckInput.checked = true;
        }
    }
    else {
        if (viewIcon) {
            viewIcon.classList.remove("fa-check", "fa-question", "fa-times");
            viewIcon.classList.add("fa-times");
        }
        if (viewFormControl) {
            viewFormControl.innerHTML = config.false_label;
        }
        if (editCheckInput) {
            editCheckInput.checked = false;
        }
    }

    CheckboxInlineEditPreDisableCallback(fieldId, fieldName, config);
    toastr.success("The new value is successfully saved", 'Success!', { closeButton: true, tapToDismiss: true });
}

function CheckboxInlineEditInitErrorCallback(response, fieldId, fieldName, config) {
    var selectors = CheckboxInlineEditGenerateSelectors(fieldId, fieldName, config);
    var editEl = document.querySelector(selectors.editWrapper);

    if (editEl) {
        editEl.querySelectorAll(".form-control").forEach(function (el) {
            el.classList.add("is-invalid");
        });

        var errorMessage = response ? response.message : "";
        if (!errorMessage && response && response.errors && response.errors.length > 0) {
            errorMessage = response.errors[0].message;
        }

        var inputGroup = editEl.querySelector(".input-group");
        if (inputGroup) {
            inputGroup.insertAdjacentHTML('afterend', "<div class='invalid-feedback' style='display: block;'>" + (errorMessage || "") + "</div>");
        }

        editEl.querySelectorAll(".save .fa").forEach(function (el) {
            el.classList.add("fa-check");
            el.classList.remove("fa-spin", "fa-spinner");
        });

        editEl.querySelectorAll(".save").forEach(function (el) {
            el.disabled = false;
        });
    }

    toastr.error("An error occurred", 'Error!', { closeButton: true, tapToDismiss: true });
    console.log("error", response);
}