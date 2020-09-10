var FieldFileFormGlobalPasteActiveFieldId = null;

function FileFormGenerateSelectors(fieldId) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	selectors.inputEl = "#input-" + fieldId;
	selectors.fileUploadEl = "#file-" + fieldId;
	selectors.fakeInputEl = "#fake-" + fieldId;
	selectors.fakeInputLinkEl = "#fake-" + fieldId + " a";
	selectors.fakeInputProgressEl = "#fake-" + fieldId + " .form-control-progress";
	selectors.fakeInputTrigger = "#fake-" + fieldId + ".erp-file-trigger";
	selectors.removeValueEl = "#remove-" + fieldId;
	selectors.pasteValueEl = "#paste-" + fieldId;
	return selectors;
}

function FileFormSubmit(fieldId, files){
		var selectors = FileFormGenerateSelectors(fieldId);
		$(selectors.fakeInputLinkEl).hide();
		$(selectors.fakeInputProgressEl).first().text("Sending to server ...");
		var uploadApiUrl = $(selectors.fakeInputEl).attr("data-file-upload-api");

		if (files.length > 0) {
			if (window.FormData !== undefined) {
				var data = new FormData();
				//support only single file upload
				data.append("file", files[0]);
				$(selectors.pasteValueEl).first().trigger("click");
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
						xhr.upload.onprogress = function (evt) {
							var progressPercent = evt.loaded / evt.total * 100;
							$(selectors.fakeInputProgressEl).first().attr("style", "display:block;width:" + progressPercent + "%")
						};
						// set the onload event handler
						xhr.upload.onload = function () {
							$(selectors.fakeInputProgressEl).first().html("<i class='fa fa-spin fa-spinner go-blue'></i> Sent, processing ...");
						};
						// return the customized object
						return xhr;
					},
					success: function (result) {
						$(selectors.fakeInputLinkEl).first().text(result.object.filename).attr("href", "/fs" + result.object.url).attr("title", "/fs" + result.object.url);
						$(selectors.fakeInputProgressEl).first().attr("style", "display:none;width:0%")
						$(selectors.fakeInputLinkEl).show();
						var typeIconClass = WebVellaTagHelpers.GetPathTypeIcon(result.object.filename);
						$(selectors.fakeInputEl).closest(".input-group").find(".type-icon").first().attr("class", "fa fa-fw type-icon " + typeIconClass);
						//Show the input-group-addon if needed
						$(selectors.fakeInputEl).closest(".input-group").find(".icon-addon").removeClass("d-none");
						$(selectors.fakeInputEl).closest(".input-group").removeClass("left-border");
						$(selectors.fakeInputEl).closest(".input-group").find(".input-group-append .remove").removeClass("d-none");
						$(selectors.fakeInputEl).removeClass("erp-file-trigger");
						$(selectors.inputEl).val(result.object.url);
					},
					error: function (xhr, status, p3, p4) {
						var err = "Error " + " " + status + " " + p3 + " " + p4;
						if (xhr.responseText && xhr.responseText[0] === "{")
							err = JSON.parse(xhr.responseText).Message;
						$(selectors.fakeInputEl).addClass("is-invalid");
						$(selectors.fakeInputEl).closest(".input-group").after("<div class='invalid-feedback'>" + response.message + "</div>");
						$(selectors.fakeInputEl).closest(".wv-field").find(".invalid-feedback").first().show();
						toastr.error("An error occurred", 'Error!', { closeButton: true, tapToDismiss: true });
						console.log(err);
					}
				});
			} else {
				alert("This browser doesn't support HTML5 file uploads!");
			}
		}
}

function FieldFileFormAttachEventListener(event){
	if (FieldFileFormGlobalPasteActiveFieldId) {
		event.preventDefault();
		event.stopPropagation();
		var selectors = FileFormGenerateSelectors(FieldFileFormGlobalPasteActiveFieldId, null);
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
						FileFormSubmit(FieldFileFormGlobalPasteActiveFieldId, files);

						return;
					}; // data url!
					reader.readAsDataURL(blob);
				}
			}
		}
	}
}


function FileFormInit(fieldId, fieldName, config) {
	config = WebVellaTagHelpers.ProcessConfig(config);
	var selectors = FileFormGenerateSelectors(fieldId, fieldName, config);

	//Remove value
	$(selectors.removeValueEl).first().on('click', function (e) {
		$(selectors.fakeInputLinkEl).attr("href", "").attr("title", "").text();
		$(selectors.fakeInputLinkEl).hide();
		$(selectors.fakeInputEl).closest(".input-group").find(".icon-addon").addClass("d-none");
		$(selectors.fakeInputEl).closest(".input-group").addClass("left-border");
		$(selectors.fakeInputEl).closest(".input-group").find(".input-group-append .remove").addClass("d-none");
		$(selectors.inputEl).first().val("");
		$(selectors.fileUploadEl).first().val("");
	});

	//Paste
	$(selectors.pasteValueEl).first().on('click', function (event) {
		if (FieldFileFormGlobalPasteActiveFieldId === fieldId) {
			$(selectors.pasteValueEl).attr("title","Activate 'Paste Image' from clipboard");
			$(selectors.pasteValueEl + " .icon").removeClass("go-teal").addClass("go-gray");
			FieldFileFormGlobalPasteActiveFieldId = null;
			document.removeEventListener("paste",FieldFileFormAttachEventListener);
		}
		else {
			$(selectors.pasteValueEl).attr("title","listening for image paste...");
			$(selectors.pasteValueEl + " .icon").addClass("go-teal").removeClass("go-gray");
			FieldFileFormGlobalPasteActiveFieldId = fieldId;
			document.addEventListener("paste",FieldFileFormAttachEventListener);
		}
	});

	if ($(selectors.fakeInputTrigger)) {
		$(selectors.fakeInputTrigger).click(function (e) {
			e.preventDefault();
			e.stopPropagation();
			$(selectors.fileUploadEl).click();
		});
	}

	$(selectors.fileUploadEl).first().on('change', function (e) {
		var files = e.target.files;
		FileFormSubmit(fieldId,files);

	});
}
