angular
	.module("testapp")
	.directive('jsonTable', function ($compile, $filter) {
		return {
			scope: {
				data: '='
			},
			templateUrl: '$urlbase$/File?path=app/application/jsonTable.html',
			link: function (scope, element) {
				var buildTable = function (data) {
					if (!angular.isArray(data)) {
						data = [data];
					}

					var result = {
						columns: [],
						rows: []
					};

					for (var i = 0; i < data.length; i++) {
						if (!angular.isObject(data[i])) {
							data[i] = { Value: data[i] };
						}
					}
					for (var i = 0; i < data.length; i++) {
						var row = data[i];
						for (var column in row) {
							if (column === "Data") {
								for (var dcolumn in row[column]) {
									if (!$filter("contains")(result.columns, dcolumn)) {
										result.columns.push(dcolumn);
									}
								}
							} else if (!$filter("contains")(result.columns, column)) {
								result.columns.push(column);
							}
						}
					}

					for (var i = 0; i < data.length; i++) {
						var row = data[i];
						var tr = [];
						for (var j = 0; j < result.columns.length; j++) {
							var column = result.columns[j];
							if (row == null) {
								tr.push("null");
							} else if (row.hasOwnProperty(column)) {
								tr.push(row[column]);
							} else if (row.hasOwnProperty("Data") && row.Data.hasOwnProperty(column)) {
								tr.push(row.Data[column]);
							} else {
								tr.push("");
							}
						}
						result.rows.push(tr);
					}

					return result;
				};

				scope.$watch('data', function () {
					scope.table = buildTable(scope.data);
				});
			}
		};
	});
