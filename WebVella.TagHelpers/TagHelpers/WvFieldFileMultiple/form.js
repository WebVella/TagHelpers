
var FieldMultiFileFormGlobalPasteActiveFieldId = null;

function FieldMultiFileFormGenerateSelectors(fieldId, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.inputEl = "#input-" + fieldId;
	selectors.fileUploadEl = "#file-" + fieldId;
	selectors.fakeInputEl = "#fake-" + fieldId;
	selectors.fakeInputProgressEl = selectors.fakeInputEl + " .form-control-progress";
	selectors.fileListEl = "#fake-list-" + fieldId;
	selectors.removeFileLink = selectors.fileListEl + " .filerow .action.remove .link";
	return selectors;
}

function FieldMultiFileFormAttachEventListener(event){
	if (FieldMultiFileFormGlobalPasteActiveFieldId) {
		event.preventDefault();
		event.stopPropagation();
		var selectors = FieldMultiFileFormGenerateSelectors(FieldMultiFileFormGlobalPasteActiveFieldId, null);
		var items = (event.clipboardData || event.originalEvent.clipboardData).items;
		//console.log(JSON.stringify(items)); // will give you the mime types
		if (items.length > 0) {

			for (index in items) {
				var item = items[index];
				if (item.kind === 'file') {
					event.preventDefault();
					var blob = item.getAsFile();
					var reader = new FileReader();
					reader.onload = function (event) {
						//console.log("dataUrl", event.target.result);
						//console.log("event", event);
						//console.log("file", blob);
						//console.log("fileName", blob.name);
						var nameArray = blob.name.split(".");
						blob.newFilename = "clipboard-" + moment().format("YYYY-MM-DD-HH-MM") + "." + nameArray[nameArray.length - 1];
						var files = [];
						files.push(blob);
						FieldMultiFileFormSubmit(FieldMultiFileFormGlobalPasteActiveFieldId, files);

						return;
					}; // data url!
					reader.readAsDataURL(blob);
				}
			}
		}
	}
}

function FieldMultiFileFormInsertFile(fieldId, file){
	var selectors = FieldMultiFileFormGenerateSelectors(fieldId, null);
	//Add file id to hidden input
	var inputValue = $(selectors.inputEl).val();
	var pathArray = [];
	if (inputValue) {
		pathArray = inputValue.toLowerCase().split(',');
	}
	pathArray.push(file.path);
	$(selectors.inputEl).val(pathArray.join(','));

	var handlerPrefix = $(selectors.fakeInputEl).attr("data-handler-prefix");
	var propPathName = $(selectors.fakeInputEl).attr("data-path-name");
	var propSizeName = $(selectors.fakeInputEl).attr("data-size-name");
	var propNameName = $(selectors.fakeInputEl).attr("data-name-name");
	var propIconName = $(selectors.fakeInputEl).attr("data-icon-name");

	//Path
	var filePath = "";
	if(propPathName && file[propPathName]){
		filePath = handlerPrefix + file[propPathName];
	}
	//Size
	var fileSize = 0;
	var fileSizeString = "";
	if(propSizeName && file[propSizeName]){
		var stringValue = JSON.stringify(file[propSizeName]);
		if(stringValue.length > 0 && !isNaN(stringValue)){
			fileSize = parseInt(stringValue, 10);
		}
	}
	if(fileSize > 0){
		if (fileSize < 1024) {
			fileSizeString = fileSize + " KB";
		}
		else if (fileSize >= 1024 && fileSize < 1024 * 1024) {
			fileSizeString = Math.round(fileSize / 1024) + " MB";
		}
		else {
			fileSizeString = Math.round(fileSize / (1024 * 1024)) + " GB";
		}
	}


	//file Name
	var fileName = "";
	if(propNameName && file[propNameName]){
		fileName = file[propNameName];
	}

	//Icon Class
	var fileIcon = "";
	if(propIconName && file[propIconName]){
		fileIcon = file[propIconName];
	}
	if(fileName && !fileIcon){
		fileIcon = "fa " + WebVellaTagHelpers.GetPathTypeIcon(file.name);
	}

	var fileRowEl = document.createElement("div");
	fileRowEl.className = "filerow";
	fileRowEl.dataset["filePath"] = file.path;
	fileRowEl.dataset["fieldId"] = fieldId;
	fileRowEl.innerHTML = '<div class="icon"><i class="' + fileIcon +'"></i></div><div class="meta"><a class="link" href="' + filePath +'" target="_blank" title="'+ filePath + '">'+ fileName +'<em></em></a><div class="details"><div class="size">' + fileSizeString + '</div></div></div>';
									
	var fileRowAction = document.createElement("div");
	fileRowAction.className = "action remove";
									
	var fileRowActionLink = document.createElement("a");
	fileRowActionLink.className = "link";
	fileRowActionLink.href = "#";
	fileRowActionLink.innerHTML = '<i class="fa fa-times-circle"></i>';
	fileRowActionLink.onclick = FieldMultiFileRemoveFileRow;
	fileRowAction.appendChild(fileRowActionLink);
	fileRowEl.appendChild(fileRowAction);
									
	$(selectors.fileListEl).prepend(fileRowEl);
}

function FieldMultiFileFormSubmit(fieldId, files) {
	var selectors = FieldMultiFileFormGenerateSelectors(fieldId, null);
	if (files.length > 0) {
		if (window.FormData !== undefined) {
			var inputEl = $(selectors.inputEl);
			$(selectors.fakeInputEl).html("<div class='form-control-progress'></div>");
			$(selectors.fakeInputEl).removeClass("is-invalid");

			var data = new FormData();
			//support only single file upload
			var totalSize = 0;
			for (var g = 0; g < files.length; ++g) {
				totalSize += files[g].size;
			}
			if (totalSize >= 10485760) {
				if (files.length === 1) {
					toastr.error(files[0].name + " е с размер по-голям от 10 МВ и няма да бъде обработен", 'Грешка!', { closeButton: true, tapToDismiss: true });
				}
				else {
					toastr.error("Общият размер е по-голям от 10 МВ и няма да бъде обработен", 'Грешка!', { closeButton: true, tapToDismiss: true });
				}
				return;
			}

			for (var i = 0; i < files.length; ++i) {
				var currentFile = files[i];
				if (currentFile.newFilename && currentFile.newFilename !== "") {
					data.append('files', currentFile, currentFile.newFilename);
				}
				else {
					data.append('files', currentFile);
				}
			}
			var uploadApiUrl = $(selectors.fakeInputEl).attr("data-file-upload-api");


			$.ajax({
					type: "POST",
					url: uploadApiUrl,
					contentType: false,
					processData: false,
					data: data,
					xhr: function () {
						// get the native XmlHttpRequest object
						var xhr = $.ajaxSettings.xhr();
						// set the onprogress event handler
						xhr.upload.onprogress = function (evt,e) {
							var progressPercent = parseInt(evt.loaded / evt.total * 100);
							$(selectors.fakeInputProgressEl).first().attr("style", "display:block;width:" + progressPercent + "%");
							$(selectors.fakeInputProgressEl).first().html(progressPercent + "%");
						};
						// set the onload event handler
						xhr.upload.onload = function (e) {
							$(selectors.fakeInputProgressEl).first().html("<i class='fa fa-spin fa-spinner go-blue'></i> Processing ...");
						};
						// return the customized object
						return xhr;
					},
					success: function (result) {
						$(selectors.fakeInputProgressEl).first().attr("style", "display:none;width:0%");

						if (result.success) {
							if (result.object && result.object.length > 0) {
								_.forEach(result.object, function(file) {
									FieldMultiFileFormInsertFile(fieldId,file);
								});			
								
								$(selectors.fileListEl).removeClass("d-none");
							}
						}
						else {
							$(selectors.fakeInputEl).addClass("is-invalid");
							$(selectors.fakeInputProgressEl).first().attr("style", "display:none;width:'0px'");
							$(selectors.fakeInputEl).html("<span class='go-red'><i class='fa fa-exclamation-circle'></i> " + result.message +"</span>");

							toastr.error("An error occurred", 'Error!', { closeButton: true, tapToDismiss: true });
							console.log(result.message);							
						}

					},
					error: function (xhr, status, p3, p4) {
						var err = "Error " + " " + status;
						if (p3) {
							err += " " + p3;
						}
						if (p4) {
							err += " " + p4;
						}
						if (xhr.responseText && xhr.responseText.startsWith("{")) {
							err = JSON.parse(xhr.responseText).message;
						}

						$(selectors.fakeInputEl).addClass("is-invalid");
						$(selectors.fakeInputProgressEl).first().attr("style", "display:none;width:'0px'");
						$(selectors.fakeInputEl).html("<span class='go-red'><i class='fa fa-exclamation-circle'></i> " + err +"</span>");

						toastr.error("An error occurred", 'Error!', { closeButton: true, tapToDismiss: true });
						console.log(err);
					}
				});
		} else {
			alert("This browser doesn't support HTML5 file uploads!");
		}
	}
}


function FieldMultiFileRemoveFileRow(e) {
	e.preventDefault();
    e.stopPropagation();
    var clickedBtn = event.target;
    var fileRow = $(clickedBtn).closest(".filerow");
	var filePath = $(fileRow).attr("data-file-path");
	var fieldId = $(fileRow).attr("data-field-id");
    var selectors = FieldMultiFileFormGenerateSelectors(fieldId, {});
    var inputValue = $(selectors.inputEl).val();
    if (inputValue && inputValue.indexOf(filePath) > -1) {
        var pathArray = [];
        if (inputValue) {
            pathArray = inputValue.toLowerCase().split(',');
        }
        var filteredArray = _.filter(pathArray, function (recordPath) { return recordPath !== filePath; });
        $(selectors.inputEl).val(filteredArray.join(','));
        $(fileRow).remove();
        if (filteredArray.length === 0) {
            $(selectors.fileListEl).addClass("d-none");
        }
    }
    else {
        console.error("File Id: " + fileId + " not found in the hidden input value");
    }
}

function FieldMultiFileFormInit(fieldId, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
    var selectors = FieldMultiFileFormGenerateSelectors(fieldId, config);
	//Remove value
	$(selectors.removeFileLink).on('click', function(e){ FieldMultiFileRemoveFileRow(e);});

    $(selectors.fileUploadEl).first().on('change', function (e) {
        var files = e.target.files;
		FieldMultiFileFormSubmit(fieldId,files);
	});

	$(selectors.fakeInputEl).click(function (event) {
		if (FieldMultiFileFormGlobalPasteActiveFieldId === fieldId) {
			$(selectors.fakeInputEl).text("Activate 'Paste Image' from clipboard");
			$(selectors.fakeInputEl).removeClass("go-teal go-bkg-teal-light").addClass("go-gray");
			FieldMultiFileFormGlobalPasteActiveFieldId = null;
			document.removeEventListener("paste",FieldMultiFileFormAttachEventListener);
		}
		else {
			$(selectors.fakeInputEl).text("listening for image paste...");
			$(selectors.fakeInputEl).addClass("go-teal go-bkg-teal-light").removeClass("go-gray");
			FieldMultiFileFormGlobalPasteActiveFieldId = fieldId;
			document.addEventListener("paste",FieldMultiFileFormAttachEventListener);
		}
	});

}
