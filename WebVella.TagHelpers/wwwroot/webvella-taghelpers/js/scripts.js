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

