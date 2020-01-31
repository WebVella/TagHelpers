function RadioListFormGenerateSelectors(fieldId) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.inputControlWrapper = "#input-" + fieldId;
	return selectors;
}

function RadioListFormInit(fieldId,isButtons,uncheckedClass,checkedClass) {
	if(!isButtons)
		return false;

	var selectors = RadioListFormGenerateSelectors(fieldId);
	
	$(selectors.inputControlWrapper + " .btn").on("click",function(e){
		e.preventDefault();
		if($(this).hasClass(checkedClass)){
			// already activated -> deactivate
			$(this).removeClass(checkedClass);
			$(this).addClass(uncheckedClass);
			$(this).find("input").prop("checked", false);
			$(this).find("input").trigger("change");
		}
		else{
			var buttons = document.querySelectorAll(selectors.inputControlWrapper + " .btn");

			for (var i = 0; i < buttons.length; ++i) {
				var $btn = buttons[i];
				if($btn === this){
					$($btn).addClass(checkedClass);
					$($btn).removeClass(uncheckedClass);
					$($btn).find("input").prop("checked", true);
					$($btn).find("input").trigger("change");
				}
				else
				{
					$($btn).removeClass(checkedClass);
					$($btn).addClass(uncheckedClass);
					$($btn).find("input").prop("checked", false);
					$($btn).find("input").trigger("change");
				}
			}
		}
	});

}

