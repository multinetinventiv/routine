angular
	.module("testapp")
	.directive('jsonCell', function ($compile) {
		return {
			scope: {
				data: '='
			},
			templateUrl: '$urlbase$/File?path=app/application/jsonCell.html',
			link: function (scope, element) {
				scope.isObject = function (val) {
					return angular.isObject(val);
				}

				if (angular.isObject(scope.data)) {
					element.append('<div json-table ' +
										'data="data"></div>');

					$compile(element.contents())(scope);
				}
			}
		};
	});
