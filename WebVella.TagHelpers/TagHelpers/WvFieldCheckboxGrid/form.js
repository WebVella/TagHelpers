function CheckboxGridFormGenerateSelectors(fieldId) {
    //Method for generating selector strings of some of the presentation elements
    var selectors = {};
    selectors.inputEl = "#input-" + fieldId;
    selectors.gridCheckbox = ".gchk-" + fieldId;
    return selectors;
}

function CheckboxGridFormInit(fieldId) {
    var selectors = CheckboxGridFormGenerateSelectors(fieldId);
    //Remove value
    document.querySelectorAll(selectors.gridCheckbox).forEach(function (el) {
        el.addEventListener('change', function (e) {
            var rowId = this.getAttribute("data-row-key");
            var currentValue = JSON.parse(document.querySelector(selectors.inputEl).value);
            var chkValue = this.value;
            var keyIndex = _.findIndex(currentValue, function (record) {return record.key === rowId;});
            
            if (this.checked) {
                if (keyIndex === -1) {
                    var newObj = { key: rowId, values: [] };
                    newObj.values.push(chkValue);
                    currentValue.push(newObj);
                }
                else {
                    currentValue[keyIndex].values = _.concat(currentValue[keyIndex].values, chkValue);
                }
            }
            else {
                if (keyIndex > -1) {
                    _.remove(currentValue[keyIndex].values, function (record) {return record === chkValue;});
                }
            }
            document.querySelector(selectors.inputEl).value = JSON.stringify(currentValue);
        });
    });
}