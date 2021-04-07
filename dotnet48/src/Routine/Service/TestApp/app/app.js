angular.module("testapp", ["angular.filter", "ui.router", "ui.bootstrap"]);

String.prototype.splitCamelCase = function () {
	return this
		.replace(/([.])/g, ' /')
		.replace(/([A-Z])/g, ' $1')
		.replace(/^./, function (str) { return str.toUpperCase(); });
};