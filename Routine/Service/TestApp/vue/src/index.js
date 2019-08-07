var app = new Vue({
	el: '#app',
	data() {
		return {
			message: 'Hello Vue!',
			headersState: true,
			headers: {},
			responseHeaders: [],
			modules: {},
			models: [],
			headersloading: true,
			modelsloading: true,
			moduleMenuState: {},
			modelMenuState: {}
		}
	},
	mounted() {
		this.getConfiguration();
		this.getApplicationModel();
	},
	methods: {
		getConfiguration: function () {

			axios.get("http://localhost:32805/Service/Configuration").then(response => {
				var data = response.data;
				app.headersloading = false;

				data.requestHeaders.forEach(function (requestHeader) {
					app.headers[requestHeader] = "";
				});
				app.responseHeaders = data.responseHeaders;
			});
		},

		getApplicationModel: function () {

			axios.get("http://localhost:32805/Service/ApplicationModel").then(response => {
				var data = response.data;
				app.modelsloading = false;
				app.models = data.Models;

				app.models.forEach(function (model) {
					model.Initializer.ModelId = model.Id;

					model.Datas.forEach(function (data) {
						data.ModelId = model.Id;
					});

					model.Operations.forEach(function (operation) {
						operation.ModelId = model.Id;

						operation.Parameters.forEach(function (parameter) {
							parameter.ModelId = model.Id;
							parameter.OperationName = operation.Name;
						});
					});
				});
			});

		},

		getParentModules: function (modules) {
			var newModules = {};
			Object.keys(modules).forEach(moduleName => {
				var modulesByModuleName = modules[moduleName];
				modulesByModuleName.forEach(module => {
					if (module.Operations && module.Operations.length > 0) {
						if (!newModules[moduleName]) {
							newModules[moduleName] = modulesByModuleName;
						}
					}
				});
			});
			return newModules;
		},

		toggleModuleMenuState: function (module) {
			this.$set(this.moduleMenuState, module, !this.moduleMenuState[module]);
		},

		toggleModelMenuState: function (model) {
			this.$set(this.modelMenuState, model, !this.modelMenuState[model]);
		},

		getRealMarks: function (marks) {
			return marks.filter(mark => {
				return /^(?!__routine).+/g.test(mark);
			});
		},

		getModuleModelsByHasOperations: function (moduleModels) {
			return moduleModels.filter(moduleModel => {
				return moduleModel.Operations && moduleModel.Operations.length > 0;
			});
		}
	}
});