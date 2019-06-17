angular
	.module("testapp")
	.directive('parameterValue', function ($compile) {
		return {
			scope: {
				model: '=',
				data: '=',
				i: '=',
				modelOf: '&',
				setValue: '&',
				unsetValue: '&'
			},
			templateUrl: '$urlbase$/File?path=app/application/parameterValue.html',
			link: function (scope, element) {
				scope.modelOfW = function (obj) {
					return scope.modelOf({ obj: obj });
				}

				scope.$watch('model', function () {
					if (scope.model == null) {
						return;
					}

					if (scope.data != null) {
						if (scope.model.StaticInstances.length === 1) {
							scope.data.Id = scope.model.StaticInstances[0].Id;
						} else {
							delete scope.data.Id;
						}
					}

					if (scope.model.StaticInstances.length <= 0 && scope.model.Initializer.Parameters.length > 0) {
						if (scope.data != null) {
							scope.data.Data = {};
							delete scope.data.Id;
						}

						if (angular.isUndefined(scope.recursiveDirectiveAdded)) {
							element.append('<div parameter ' +
												'pmodel="parameter" ' +
												'viewmodel="modelOf({obj: parameter.ViewModelId})" ' +
												'data="data.Data" ' +
												'model-of="modelOfW(obj)" ' +
												'ng-repeat="parameter in model.Initializer.Parameters"></div>');

							$compile(element.contents())(scope);
							scope.recursiveDirectiveAdded = true;
						}
					} else if (scope.model.Initializer.Parameters.length <= 0) {
						if (scope.data != null) {
							delete scope.data.Data;
						}
					}
				});
			}
		};
	});
