var htmlFieldModalOptions = {
	backdrop: "static"
};
if(!window.HtmlFieldEditors){
	window.HtmlFieldEditors = {};
}

function InitHtmlFieldCKEditor(fieldId, fieldConfig) { //modes are -> none, one-repository,user-repository
	fieldConfig = ProcessConfig(fieldConfig);
	//Fix for a case when used in page body node options and the init is called multiple times and results in error: Some CKEditor 5 modules are duplicated
	//Destroy ckeditor
	//if (HtmlFieldEditors[fieldId]) {
	//	HtmlFieldEditors[fieldId].destroy();
	//	delete HtmlFieldEditors[fieldId];
	//};

	if (!HtmlFieldEditors[fieldId]) {
		var config = {};
		config.language = 'en';
		//switch (fieldConfig.toolbar_mode) {
		//	default:
		//		config.toolbar = ['bold','italic','|','numberedList','bulletedList','|','indent','outdent','|','link','pasteTest','pasteFromWord'];
		//}
		//var removePluginsArray = [];
		//switch (fieldConfig.toolbar_mode) {
		//	default: //Basic
		//		extraPluginsArray.push("panel");
		//		extraPluginsArray.push("autogrow");
		//		config.toolbar = 'full';
		//		config.toolbar_full = [
		//			{ name: 'basicstyles', items: ['Bold', 'Italic'] },
		//			{ name: 'paragraph', items: ['NumberedList', 'BulletedList'] },
		//			{ name: 'indent', items: ['Indent', 'Outdent'] },
		//			{ name: 'links', items: ['Link', 'Unlink'] },
		//			{ name: 'pasting', items: ['PasteText', 'PasteFromWord'] },
		//		]
		//		break;
		//	case 2: //Standard
		//		extraPluginsArray.push("colorbutton");
		//		extraPluginsArray.push("colordialog");
		//		extraPluginsArray.push("panel");
		//		extraPluginsArray.push("font");
		//		extraPluginsArray.push("autogrow");
		//		config.colorButton_colors = '333333,FFFFFF,F44336,E91E63,9C27B0,673AB7,3F51B5,2196F3,03A9F4,00BCD4,009688,4CAF50,8BC34A,CDDC39,FFEB3B,FFC107,FF9800,FF5722,795548,607D8B,999999';
		//		config.colorButton_enableAutomatic = false;
		//		config.colorButton_enableMore = false;
		//		config.toolbar = 'full';
		//		config.toolbar_full = [
		//			{ name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', 'Underline'] },
		//			{ name: 'colors', items: ['TextColor', 'BGColor'] },
		//			{ name: 'styles', items: ['FontSize', 'RemoveFormat'] },
		//			{ name: 'editing', items: ['Format'] },
		//			{ name: 'links', items: ['Link', 'Unlink'] },
		//			{ name: 'pasting', items: ['PasteText', 'PasteFromWord'] },
		//			{ name: 'paragraph', items: ['BulletedList', 'NumberedList', 'Blockquote'] },
		//			{ name: 'insert', items: ['Image', 'Table'] },
		//		]
		//		break;
		//	case 3: //Full
		//		extraPluginsArray.push("sourcedialog");
		//		extraPluginsArray.push("colorbutton");
		//		extraPluginsArray.push("colordialog");
		//		extraPluginsArray.push("panel");
		//		extraPluginsArray.push("font");
		//		extraPluginsArray.push("autogrow");
		//		config.colorButton_colors = '333333,FFFFFF,F44336,E91E63,9C27B0,673AB7,3F51B5,2196F3,03A9F4,00BCD4,009688,4CAF50,8BC34A,CDDC39,FFEB3B,FFC107,FF9800,FF5722,795548,607D8B,999999';
		//		config.colorButton_enableAutomatic = false;
		//		config.colorButton_enableMore = false;
		//		config.toolbar = 'full';
		//		config.toolbar_full = [
		//			{ name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', 'Underline'] },
		//			{ name: 'colors', items: ['TextColor', 'BGColor'] },
		//			{ name: 'styles', items: ['FontSize', 'RemoveFormat'] },
		//			{ name: 'editing', items: ['Format'] },
		//			{ name: 'links', items: ['Link', 'Unlink'] },
		//			{ name: 'pasting', items: ['PasteText', 'PasteFromWord'] },
		//			{ name: 'paragraph', items: ['BulletedList', 'NumberedList', 'Blockquote'] },
		//			{ name: 'insert', items: ['Image', 'Table', 'SpecialChar'] },
		//			{ name: 'tools', items: ['Sourcedialog'] }
		//		]
		//		break;
		//}
		//switch (fieldConfig.upload_mode) {
		//	default: //None
		//		removePluginsArray.push("uploadimage");
		//		removePluginsArray.push("uploadfile");
		//		break;
		//	case 2: //SiteRepository
		//		config.filebrowserImageBrowseUrl = '/ckeditor/ImageFinder';
		//		config.filebrowserImageUploadUrl = '/ckeditor/image-upload-url';
		//		config.uploadUrl = '/ckeditor/drop-upload-url';
		//		extraPluginsArray.push("uploadimage");
		//		break;
		//}

		//if (extraPluginsArray.length > 0) {
		//	config.extraPlugins = _.join(extraPluginsArray, ",");
		//}

		//if (removePluginsArray.length > 0) {
		//	config.removePlugins = _.join(removePluginsArray, ",");
		//}


		//var editor = CKEDITOR.replace('input-' + fieldId, config);

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
