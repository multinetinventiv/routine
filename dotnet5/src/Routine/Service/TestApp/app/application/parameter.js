angular
	.module("testapp")
	.directive('parameter', function () {
		return {
			scope: {
				pmodel: '=',
				data: '=',
				viewmodel: '=',
				modelOf: '&'
			},
			templateUrl: '$urlbase$/File?path=app/application/parameter.html',
			link: function (scope, element, attributes) {
				var createEmptyItem = function () {
					if (!scope.viewmodel.IsViewModel) {
						return {};
					}

					if (scope.viewmodel.ActualModelIds.length === 1) {
						return { ModelId: scope.modelOf({ obj: scope.viewmodel.ActualModelIds[0] }).Id };
					}

					return {};
				}

				scope.getItems = function () {
					var pmodel = this.pmodel;
					var data = this.data;
					if (pmodel.IsList) {
						return data[pmodel.Name];
					}

					if (!data.hasOwnProperty(pmodel.Name)) {
						return [];
					}

					return [data[pmodel.Name]];
				}

				scope.addValue = function (i) {
					var pmodel = this.pmodel;
					var data = this.data;

					if (pmodel.IsList) {
						if (angular.isUndefined(data[pmodel.Name])) {
							data[pmodel.Name] = [];
						}

						if (angular.isUndefined(i)) {
							data[pmodel.Name].push(createEmptyItem());
						} else {
							data[pmodel.Name][i] = createEmptyItem();
						}
					} else {
						if (angular.isUndefined(data[pmodel.Name]) || data[pmodel.Name] == null) {
							data[pmodel.Name] = createEmptyItem();
						};
					}
				};

				scope.removeValue = function (i) {
					var pmodel = this.pmodel;
					var data = this.data;

					if (pmodel.IsList) {
						if (angular.isUndefined(data[pmodel.Name])) {
							return;
						} else if (data[pmodel.Name].length <= 0) {
							delete data[pmodel.Name];

							return;
						}

						if (angular.isUndefined(i)) {
							data[pmodel.Name].splice(data[pmodel.Name].length - 1, 1);
						} else {
							data[pmodel.Name][i] = null;
						}
					} else {
						if (data.hasOwnProperty(pmodel.Name) && angular.isUndefined(i)) {
							delete data[pmodel.Name];

							return;
						} else if (angular.isUndefined(data[pmodel.Name])) {
							return;
						}

						data[pmodel.Name] = null;
					}
				}

				scope.currentModel = function (item) {
					var viewmodel = this.viewmodel;
					if (viewmodel.IsViewModel) {
						if (item == null) {
							return null;
						}
						if (!item.hasOwnProperty("ModelId")) {
							return null;
						}
						return scope.modelOf({ obj: item.ModelId });
					}

					return viewmodel;
				}

				scope.modelOfW = function (obj) {
					return scope.modelOf({ obj: obj });
				}

				scope.addValue();
			}
		};
	});
