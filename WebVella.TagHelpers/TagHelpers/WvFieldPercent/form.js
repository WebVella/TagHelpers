function PercentFormGenerateSelectors(fieldId, fieldName, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.inputEl = "#input-" + fieldId;
	selectors.fakeEl = "#fake-" + fieldId;
	return selectors;
}

function PercentFormSetPercent(fieldId, fieldName, config) {
	var selectors = PercentFormGenerateSelectors(fieldId, fieldName, config);
	var decimalDigits = 4;
	if (config.decimal_digits) {
		decimalDigits = config.decimal_digits + 2;
	}
	var value = $(selectors.fakeEl).val();
	if (!value || value === null) {
		$(selectors.inputEl).val(null);
	}
	else {
		var valDec = new Decimal(value);
		var hundDec = new Decimal(100);
		var percentDec = valDec.dividedBy(hundDec);
		var roundedDec = percentDec.toDecimalPlaces(decimalDigits);
		$(selectors.inputEl).val(roundedDec.toString());
	}
}

function PercentFormInit(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = PercentFormGenerateSelectors(fieldId, fieldName, config);
	$(selectors.fakeEl).on("change paste keyup", function () {
		PercentFormSetPercent(fieldId, fieldName, config);
	});
}