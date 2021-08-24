/**
 * /General scripts
 */
window.WebVellaTagHelpers = {
    ProcessNewValue: function (response, fieldName) {
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
        if (Array.isArray(newValue)) {
            var processedValue = [];
            _.forEach(newValue, function (value) {
                if (typeof value === 'object' && value !== null && value.id) {
                    processedValue.push(value.id);
                }
                else {
                    processedValue.push(value);
                }
            })
            newValue = processedValue;
        }
        else {
            if (typeof newValue === 'object' && newValue !== null && newValue.id) {
                newValue = newValue.id;
            }
        }
        return newValue;
    },
    ProcessConfig: function (config) {
        if (config !== null && typeof config === 'object') {
            return config;
        }
        else if (config) {
            return JSON.parse(config);
        }
        else {
            return {};
        }
    },
    WvFixModalInModalClose: function () {
        //Fix for modal in modal scroll problem
        var bodyEl = document.querySelector("body");
        var openModalsCount = $('.modal:visible').length;
        if (!bodyEl.classList.contains("modal-open") && openModalsCount > 0) {
            bodyEl.classList.add("modal-open");
        }
    },
    isStringNullOrEmptyOrWhiteSpace: function (str) {
        return (!str || str.length === 0 || /^\s*$/.test(str));
    },
    isEmpty: function (obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key))
                return false;
        }
        return true;
    },
    checkInt: function (data) {
        var response = {
            success: true,
            message: "It is integer"
        }
        if (!data) {
            response.message = "Empty value is OK";
            return response;
        }
        if (!WebVellaTagHelpers.isNumeric(data)) {
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

    },
    checkDecimal: function (data) {
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

        if (!WebVellaTagHelpers.isNumeric(data)) {
            response.success = false;
            response.message = "Only decimal is accepted";
            return response;
        }

        return response;
    },
    isNumeric: function (n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    },
    decimalPlaces: function (num) {
        var match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
        if (!match) { return 0; }
        return Math.max(
            0,
            // Number of digits right of decimal point.
            (match[1] ? match[1].length : 0)
            // Adjust for scientific notation.
            - (match[2] ? +match[2] : 0));
    },
    checkPercent: function (data) {
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
        if (!WebVellaTagHelpers.isNumeric(data)) {
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
    },
    checkPhone: function (data) {
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
    },
    checkEmail: function (data) {
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
    },
    GetPathTypeIcon: function (filePath) {
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
    },
    newGuid: function () {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    },
    clearSelection: function () {
        //Double click causes text to be selected. This clears this text
        if (document.selection && document.selection.empty) {
            document.selection.empty();
        } else if (window.getSelection) {
            var sel = window.getSelection();
            sel.removeAllRanges();
        }
    },
    GetFilenameFromUrl: function (url) {
        if (url && url !== "") {
            return url.split('/').pop().split('#')[0].split('?')[0];
        }
        return "";
    },
    ExtractTagsFromTemplate: function (str) {
        var results = [], re = /{{([^}]+)}}/g, text;

        while (text = re.exec(str)) {
            results.push(text[1]);
        }
        return results;
    },
    ProcessStringTemplateWithObject: function (str, record) {
        if (typeof str !== "string")
            throw "first parameter should be a string";
        if (typeof record !== "object")
            throw "second parameter should be an object";
        var processedString = str;
        var templateTags = WebVellaTagHelpers.ExtractTagsFromTemplate(str);
        _.forEach(templateTags, function (tag) {
            var replaceValue = "!invalid!";
            if (record.hasOwnProperty(tag)) {
                replaceValue = record[tag];
            }
            processedString = processedString.replaceAll("{{" + tag + "}}", replaceValue);
        });

        return processedString;
    }
}

