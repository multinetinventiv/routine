angular
	.module("testapp")
	.service("indexService", ["$http", "configuration", function ($http, configuration) {
		$http.defaults.headers.post["Content-Type"] = "application/json";

		var service = {};
		service.getConfiguration = function () {
			return $http.get(configuration.urlbase + "/Configuration").then(function (response) {
				return response.data;
			});
		};
		service.getApplicationModel = function () {
			return $http.get(configuration.urlbase + "/ApplicationModel").then(function (response) {
				return response.data;
			});
		};

		service.do = function (url, headers, data) {
			return $http({
				method: "POST",
				url: url,
				headers: headers,
				data: data
			}).then(function (response) {
				return response;
			});
		}
		return service;
	}]);