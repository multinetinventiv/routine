angular
	.module('testapp')
	.factory('configuration', function () {
		var config = {};
		config.urlbase = "$urlbase$";
		return config;
	}
);

angular
	.module('testapp')
	.config(['$compileProvider', function ($compileProvider) {
		$compileProvider.debugInfoEnabled(false);
	}]
);