function CheckboxFormGenerateSelectors(fieldId) {
    //Method for generating selector strings of some of the presentation elements
    var selectors = {};
    selectors.inputEl = "#input-" + fieldId;
    selectors.submittedInput = "[data-source-id=input-" + fieldId + "]";
    return selectors;
}

function CheckboxFormInit(fieldId) {
    var selectors = CheckboxFormGenerateSelectors(fieldId);
    //Remove value
    document.querySelector(selectors.inputEl).addEventListener('change', function (e) {
        var submittedEl = document.querySelector(selectors.submittedInput);
        if (this.checked) {
            submittedEl.value = "true";
            submittedEl.dispatchEvent(new Event('change'));
        }
        else {
            submittedEl.value = "false";
            submittedEl.dispatchEvent(new Event('change'));
        }
    });
}