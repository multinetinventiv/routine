angular
	.module("testapp")
	.controller("indexController", ["$scope", "$filter", "indexService", function ($scope, $filter, indexService) {
		$scope.headers = {};
		$scope.responseHeaders = [];
		$scope.modules = [];
		$scope.headersState = true;
		$scope.requests = [];

		$scope.modelsloading = true;
		$scope.headersloading = true;

		indexService.getConfiguration().then(function (data) {
			$scope.headersloading = false;
			angular.forEach(data.requestHeaders, function (requestHeader) {
				$scope.headers[requestHeader] = "";
			});

			$scope.responseHeaders = data.responseHeaders;
		});

		indexService.getApplicationModel().then(function (data) {
			$scope.modelsloading = false;
			$scope.models = data.Models;
			angular.forEach($scope.models, function (model) {
				model.Initializer.ModelId = model.Id;

				angular.forEach(model.Datas, function (data) {
					data.ModelId = model.Id;
				});

				angular.forEach(model.Operations, function (operation) {
					operation.ModelId = model.Id;

					angular.forEach(operation.Parameters, function (parameter) {
						parameter.ModelId = model.Id;
						parameter.OperationName = operation.Name;
					});
				});
			});
		});

		$scope.showOperation = function (operation) {
			var request = {
				name: $scope.modelOf(operation).Name.splitCamelCase() + " - " + operation.Name.splitCamelCase(),
				operation: operation,
				active: false,
				loading: false,
				target: {
					ViewModelId: operation.ModelId,
					ModelId: !$scope.modelOf(operation).IsViewModel ? operation.ModelId :
						$scope.modelOf(operation).ActualModelIds.length === 1 ? $scope.modelOf(operation.ModelId).ActualModelIds[0] : ""
				},
				data: {},
				response: null,
				getUrl: function () {
					var result = '$urlbase$/' + this.target.ModelId;

					if (!(this.target.Id == undefined) && this.target.Id !== '') {
						result += '/' + this.target.Id;
					}

					if (this.target.ViewModelId !== this.target.ModelId) {
						result += '/' + this.target.ViewModelId;
					}

					result += '/' + this.operation.Name;

					return result;
				},
				invalidateTargetId: function () {
					delete this.target.Id;
				},
				make: function () {
					var self = this;
					this.loading = true;
					indexService.do(this.getUrl(), $scope.headers, this.data).then(function (response, headers) {
						self.loading = false;
						self.response = {};
						self.response.data = response.data;
						self.response.headers = {};
						angular.forEach($scope.responseHeaders, function (responseHeader) {
							self.response.headers[responseHeader] = response.headers(responseHeader);
						});
					});
				}
			};

			$scope.requests.push(request);
			$scope.activate(request);
		};

		$scope.activate = function (current, $event, $index) {
			if ($event != undefined && ($event.which === 2 || ($event.which === 1 && ($event.metaKey || $event.ctrlKey)))) {
				$scope.close($index);
				$event.preventDefault();

				return;
			}

			angular.forEach($scope.requests, function (request) {
				request.active = false;
			});

			current.active = true;
		};

		$scope.close = function (index) {
			var wasActive = $scope.requests[index].active;

			$scope.requests.splice(index, 1);

			if (wasActive && $scope.requests.length > 0) {
				$scope.activate($scope.requests[0]);
			}
		};

		$scope.modelOf = function (obj) {
			var id = obj;

			if (angular.isUndefined(id)) {
				return null;
			}

			if (!angular.isString(id)) {
				id = obj.ModelId;
			}

			return $filter('first')($scope.models, 'Id === \'' + id + '\'')[0];
		}

		$scope.beautify = function (obj) {
			return angular.toJson(obj, true);
		}
	}]);
