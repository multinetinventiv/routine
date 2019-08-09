var app = new Vue({
	el: '#app',
	store,
	computed: {
		responseHeaders() {
			return store.state.responseHeaders;
		},
		models() {
			return store.state.models;
		},
		headers() {
			return store.state.headers;
		},
		modelsloading() {
			return store.state.modelsloading;
		},
		headersloading() {
			return store.state.headersloading;
		},
		requests() {
			return store.state.requests;
		}
	},
	data() {
		return {
			headersState: true,
			modules: {},
			moduleMenuState: {},
			modelMenuState: {}
		}
	},
	components: {
		'parameter': httpVueLoader('$urlbase$/File?path=vue/src/components/parameter.vue')
	},
	mounted() {
		this.$store.dispatch('loadConfiguration');
		this.$store.dispatch('loadApplicationModel');
	},
	methods: {

		showOperation: function (operation) {
			const request = {
				name: this.$options.filters.splitCamelCase(this.modelOf(operation).Name) + " - " + this.$options.filters.splitCamelCase(operation.Name),
				operation: operation,
				active: false,
				loading: false,
				target: {
					ViewModelId: operation.ModelId,
					ModelId: !this.modelOf(operation).IsViewModel ? operation.ModelId :
						this.modelOf(operation).ActualModelIds.length === 1 ? this.modelOf(operation.ModelId).ActualModelIds[0] : ""
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

					axios.post(this.getUrl(),
						this.data,
						{
							headers: this.headers
						}).then(response => {
							//console.log(response);
							self.loading = false;
							self.response = {};
							self.response.data = response.data;
							self.response.headers = {};
							app.responseHeaders.forEach(function (responseHeader) {
								self.response.headers[responseHeader] = response.headers(responseHeader);
							});
						});

					//indexService.do(this.getUrl(), $scope.headers, this.data).then(function (response, headers) {
					//	self.loading = false;
					//	self.response = {};
					//	self.response.data = response.data;
					//	self.response.headers = {};
					//	app.responseHeaders.forEach(function (responseHeader) {
					//		self.response.headers[responseHeader] = response.headers(responseHeader);
					//	});
					//});
				}
			};
			//this.$set(this.requests, i, request);
			this.$store.dispatch('addRequest', request);
			this.activate(request);
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


		activate: function (current, $event, $index) {
			if ($event != undefined && ($event.which === 2 || ($event.which === 1 && ($event.metaKey || $event.ctrlKey)))) {
				this.close($index);
				$event.preventDefault();
				return;
			}
			this.requests.forEach(function (request) {
				request.active = false;
			});

			current.active = true;
		},

		close: function (index) {
			const wasActive = this.requests[index].active;
			this.requests.splice(index, 1);

			if (wasActive && this.requests.length > 0) {
				this.activate(this.requests[0]);
			}
		},

		modelOf: function (obj) {
			var id = obj;

			if (id === undefined) {
				return null;
			}

			if (!(typeof id === "string")) {
				id = obj.ModelId;
			}

			return _.find(store.state.models, function (item) {
				return item["Id"] === id;
			});
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