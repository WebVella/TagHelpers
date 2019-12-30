/**
 * /General scripts
 */

function ProcessConfig(config) {
	if (config !== null && typeof config === 'object') {
		return config;
	}
	else if (config) {
		return JSON.parse(config);
	}
	else {
		return {};
	}
}

function ProcessNewValue(response, fieldName) {
	var newValue = null;
	if (response.object.data && Array.isArray(response.object.data)) {
		newValue = response.object.data[0][fieldName];
	}
	else if (response.object.data) {
		newValue = response.object.data[fieldName];
	}
	else if (response.object) {
		newValue = response.object[fieldName];
	}
	return newValue;
}

//Fix for modal in modal scroll problem
function WvFixModalInModalClose() {
	var bodyEl = document.querySelector("body");
	var openModalsCount = $('.modal:visible').length;
	if (!bodyEl.classList.contains("modal-open") && openModalsCount > 0) {
		bodyEl.classList.add("modal-open");
	}
}

function isStringNullOrEmptyOrWhiteSpace(str) {
    return (!str || str.length === 0 || /^\s*$/.test(str));
}

function isEmpty(obj) {
    for (var key in obj) {
        if (obj.hasOwnProperty(key))
            return false;
    }
    return true;
}

function checkInt(data) {
    var response = {
        success: true,
        message: "It is integer"
    }
    if (!data) {
        response.message = "Empty value is OK";
        return response;
    }
    if (!isNumeric(data)) {
        response.success = false;
        response.message = "Only integer is accepted";
        return response;
    }

    if (data.toString().indexOf(",") > -1 || data.toString().indexOf(".") > -1) {
        response.success = false;
        response.message = "Only integer is accepted";
        return response;
    }

    if (data === parseInt(data, 10)) {
        return response;
    }
    else {
        response.success = false;
        response.message = "Only integer is accepted";
        return response;
    }

}

function checkDecimal(data) {
    var response = {
        success: true,
        message: "It is decimal"
    }
    if (!data) {
        response.message = "Empty value is OK";
        return response;
    }
    if (data.toString().indexOf(",") > -1) {
        response.success = false;
        response.message = "Comma is not allowed. Use '.' for decimal separator";
        return response;
    }

    if (!isNumeric(data)) {
        response.success = false;
        response.message = "Only decimal is accepted";
        return response;
    }

    return response;
}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function decimalPlaces(num) {
    var match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
    if (!match) { return 0; }
    return Math.max(
        0,
        // Number of digits right of decimal point.
        (match[1] ? match[1].length : 0)
        // Adjust for scientific notation.
        - (match[2] ? +match[2] : 0));
}

function checkPercent(data) {
    var response = {
        success: true,
        message: "It is decimal"
    }
    if (!data) {
        response.message = "Empty value is OK";
        return response;
    }
    if (data.toString().indexOf(",") > -1) {
        response.success = false;
        response.message = "Comma is not allowed. Use '.' for decimal separator";
        return response;
    }
    if (!isNumeric(data)) {
        response.success = false;
        response.message = "Only decimal is accepted";
        return response;
    }

    if (data > 1) {
        response.success = false;
        response.message = "Only decimal values between 0 and 1 are accepted";
        return response;
    }

    return response;
}

function checkPhone(data) {
    var response = {
        success: true,
        message: "It is decimal"
    }
    if (!phoneUtils.isValidNumber(data)) {
        response.success = false,
            response.message = "Not a valid phone. Should start with + followed by the country code digits";
        return response;
    }


    return response;
}

function checkEmail(data) {
    var response = {
        success: true,
        message: "It is email"
    }
    if (!data) {
        response.message = "Empty value is OK";
        return response;
    }
    var regex = new RegExp("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
    if (!regex.test(_.toLower(data.toString()))) {
        response.success = false;
        response.message = "Invalid email format";
        return response;
    }


    return response;
}

function GetPathTypeIcon(filePath) {
    var fontAwesomeIconName = "fa-file";
    if (filePath.endsWith(".txt")) {
        fontAwesomeIconName = "fa-file-alt";
    }
    else if (filePath.endsWith(".pdf")) {
        fontAwesomeIconName = "fa-file-pdf";
    }
    else if (filePath.endsWith(".doc") || filePath.endsWith(".docx")) {
        fontAwesomeIconName = "fa-file-word";
    }
    else if (filePath.endsWith(".xls") || filePath.endsWith(".xlsx")) {
        fontAwesomeIconName = "fa-file-excel";
    }
    else if (filePath.endsWith(".ppt") || filePath.endsWith(".pptx")) {
        fontAwesomeIconName = "fa-file-powerpoint";
    }
    else if (filePath.endsWith(".gif") || filePath.endsWith(".jpg")
        || filePath.endsWith(".jpeg") || filePath.endsWith(".png")
        || filePath.endsWith(".bmp") || filePath.endsWith(".tif")) {
        fontAwesomeIconName = "fa-file-image";
    }
    else if (filePath.endsWith(".zip") || filePath.endsWith(".zipx")
        || filePath.endsWith(".rar") || filePath.endsWith(".tar")
        || filePath.endsWith(".gz") || filePath.endsWith(".dmg")
        || filePath.endsWith(".iso")) {
        fontAwesomeIconName = "fa-file-archive";
    }
    else if (filePath.endsWith(".wav") || filePath.endsWith(".mp3")
        || filePath.endsWith(".fla") || filePath.endsWith(".flac")
        || filePath.endsWith(".ra") || filePath.endsWith(".rma")
        || filePath.endsWith(".aif") || filePath.endsWith(".aiff")
        || filePath.endsWith(".aa") || filePath.endsWith(".aac")
        || filePath.endsWith(".aax") || filePath.endsWith(".ac3")
        || filePath.endsWith(".au") || filePath.endsWith(".ogg")
        || filePath.endsWith(".avr") || filePath.endsWith(".3ga")
        || filePath.endsWith(".mid") || filePath.endsWith(".midi")
        || filePath.endsWith(".m4a") || filePath.endsWith(".mp4a")
        || filePath.endsWith(".amz") || filePath.endsWith(".mka")
        || filePath.endsWith(".asx") || filePath.endsWith(".pcm")
        || filePath.endsWith(".m3u") || filePath.endsWith(".wma")
        || filePath.endsWith(".xwma")) {
        fontAwesomeIconName = "fa-file-audio";
    }
    else if (filePath.endsWith(".avi") || filePath.endsWith(".mpg")
        || filePath.endsWith(".mp4") || filePath.endsWith(".mkv")
        || filePath.endsWith(".mov") || filePath.endsWith(".wmv")
        || filePath.endsWith(".vp6") || filePath.endsWith(".264")
        || filePath.endsWith(".vid") || filePath.endsWith(".rv")
        || filePath.endsWith(".webm") || filePath.endsWith(".swf")
        || filePath.endsWith(".h264") || filePath.endsWith(".flv")
        || filePath.endsWith(".mk3d") || filePath.endsWith(".gifv")
        || filePath.endsWith(".oggv") || filePath.endsWith(".3gp")
        || filePath.endsWith(".m4v") || filePath.endsWith(".movie")
        || filePath.endsWith(".divx")) {
        fontAwesomeIconName = "fa-file-video";
    }
    else if (filePath.endsWith(".c") || filePath.endsWith(".cpp")
        || filePath.endsWith(".css") || filePath.endsWith(".js")
        || filePath.endsWith(".py") || filePath.endsWith(".git")
        || filePath.endsWith(".cs") || filePath.endsWith(".cshtml")
        || filePath.endsWith(".xml") || filePath.endsWith(".html")
        || filePath.endsWith(".ini") || filePath.endsWith(".config")
        || filePath.endsWith(".json") || filePath.endsWith(".h")) {
        fontAwesomeIconName = "fa-file-code";
    }
    else if (filePath.endsWith(".exe") || filePath.endsWith(".jar")
        || filePath.endsWith(".dll") || filePath.endsWith(".bat")
        || filePath.endsWith(".pl") || filePath.endsWith(".scr")
        || filePath.endsWith(".msi") || filePath.endsWith(".app")
        || filePath.endsWith(".deb") || filePath.endsWith(".apk")
        || filePath.endsWith(".jar") || filePath.endsWith(".vb")
        || filePath.endsWith(".prg") || filePath.endsWith(".sh")) {
        fontAwesomeIconName = "fa-cogs";
    }
    else if (filePath.endsWith(".com") || filePath.endsWith(".net")
        || filePath.endsWith(".org") || filePath.endsWith(".edu")
        || filePath.endsWith(".gov") || filePath.endsWith(".mil")
        || filePath.endsWith("/") || filePath.endsWith(".html")
        || filePath.endsWith(".htm") || filePath.endsWith(".xhtml")
        || filePath.endsWith(".jhtml") || filePath.endsWith(".php")
        || filePath.endsWith(".php3") || filePath.endsWith(".php4")
        || filePath.endsWith(".php5") || filePath.endsWith(".phtml")
        || filePath.endsWith(".asp") || filePath.endsWith(".aspx")
        || filePath.endsWith(".aspx") || filePath.endsWith("?")
        || filePath.endsWith("#")) {
        fontAwesomeIconName = "fa-globe";
    }
    return fontAwesomeIconName;
}

function newGuid() {
	var d = new Date().getTime();
	var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
		var r = (d + Math.random() * 16) % 16 | 0;
		d = Math.floor(d / 16);
		return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
	});
	return uuid;
};

//Double click causes text to be selected. This clears this text
function clearSelection() {
    if (document.selection && document.selection.empty) {
        document.selection.empty();
    } else if (window.getSelection) {
        var sel = window.getSelection();
        sel.removeAllRanges();
    }
}

var BulgarianDateTimeLocale = {
    weekdays: {
        shorthand: ["Нд", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"],
        longhand: [
            "Неделя",
            "Понеделник",
            "Вторник",
            "Сряда",
            "Четвъртък",
            "Петък",
            "Събота",
        ],
    },

    months: {
        shorthand: [
            "яну",
            "фев",
            "март",
            "апр",
            "май",
            "юни",
            "юли",
            "авг",
            "сеп",
            "окт",
            "ное",
            "дек",
        ],
        longhand: [
            "Януари",
            "Февруари",
            "Март",
            "Април",
            "Май",
            "Юни",
            "Юли",
            "Август",
            "Септември",
            "Октомври",
            "Ноември",
            "Декември",
        ],
    },
};

function GetFilenameFromUrl(url)
{
   if (url && url !== "")
   {
      return url.split('/').pop().split('#')[0].split('?')[0];
   }
   return "";
}

//////////////////////////////////////////////////////
/// Textarea autogrow => Author: https://github.com/evyros/textarea-autogrow
//////////////////////////////////////////////////////

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define([], factory);
    } else if (typeof module === 'object' && module.exports) {
        module.exports = factory();
    } else {
        root.Autogrow = factory();
  }
})(this, function(){
    return function(textarea, maxLines){
        var self = this;

        if(maxLines === undefined){
            maxLines = 999;
        }

         // Calculates the vertical padding of the element
         // @param textarea
         // @returns {number}
        self.getOffset = function(textarea){
            var style = window.getComputedStyle(textarea, null),
                props = ['paddingTop', 'paddingBottom'],
                offset = 0;

            for(var i=0; i<props.length; i++){
                offset += parseInt(style[props[i]]);
            }
            return offset;
        };

         // Sets textarea height as exact height of content
         // @returns {boolean}
        self.autogrowFn = function(){
            var newHeight = 0, hasGrown = false;
            if((textarea.scrollHeight - offset) > self.maxAllowedHeight){
                textarea.style.overflowY = 'scroll';
                newHeight = self.maxAllowedHeight;
            }
            else {
                textarea.style.overflowY = 'hidden';
                textarea.style.height = 'auto';
                newHeight = textarea.scrollHeight - offset;
                hasGrown = true;
            }
            textarea.style.height = newHeight + 'px';
            return hasGrown;
        };

        var offset = self.getOffset(textarea);
        self.rows = textarea.rows || 1;
        self.lineHeight = (textarea.scrollHeight / self.rows) - (offset / self.rows);
        self.maxAllowedHeight = (self.lineHeight * maxLines) - offset;

        // Call autogrowFn() when textarea's value is changed
        textarea.addEventListener('input', self.autogrowFn);
    };
});