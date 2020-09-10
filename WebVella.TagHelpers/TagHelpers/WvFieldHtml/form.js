var htmlFieldModalOptions = {
	backdrop: "static"
};
if(!window.HtmlFieldEditors){
	window.HtmlFieldEditors = {};
}

function InitHtmlFieldCKEditor(fieldId, fieldConfig) { //modes are -> none, one-repository,user-repository
	fieldConfig = WebVellaTagHelpers.ProcessConfig(fieldConfig);
	//Fix for a case when used in page body node options and the init is called multiple times and results in error: Some CKEditor 5 modules are duplicated
	//Destroy ckeditor
	//if (HtmlFieldEditors[fieldId]) {
	//	HtmlFieldEditors[fieldId].destroy();
	//	delete HtmlFieldEditors[fieldId];
	//};

	if (!HtmlFieldEditors[fieldId]) {
		var config = {};
		config.language = 'en';

		ClassicEditor
			.create(document.querySelector('#input-' + fieldId), config)
			.then(function (editor) {
				if(!HtmlFieldEditors){
					HtmlFieldEditors = {};
				}
				HtmlFieldEditors[fieldId] = editor;
				editor.model.document.on('change:data', function () {
					editor.updateSourceElement();
					var customEvent = new Event('WvFieldHtml_Change');
					var inputElement = document.getElementById('input-' + fieldId);
					//If the element still exists
					if (inputElement) {
						customEvent.payload = {
							value: editor.getData(),
							fieldId: fieldId,
							fieldName: inputElement.name
						};
						document.dispatchEvent(customEvent);
					}
				});
			})
			.catch(function (error) {
				console.error(error);
			});
	}

}
