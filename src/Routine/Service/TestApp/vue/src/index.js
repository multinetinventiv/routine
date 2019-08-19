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
		},
		filteredModules() {
			const search = this.search;

			const modules = _.groupBy(_.sortBy(this.models, ['Module']), 'Module');
			let clonedModules = _.cloneDeep(modules, function () {
				this.$store.commit('CHANGE_MODELS_LOAIDING', true);
			});

			let newModules = {};
			Object.keys(clonedModules).forEach(moduleName => {
				var modulesByModuleName = clonedModules[moduleName];
				modulesByModuleName.forEach(module => {
					if (search && search.length >= 3) {
						module.Operations = _.filter(module.Operations, function (operation) {
							return _.includes(operation.Name.toLowerCase(), search.toLowerCase());
						});
					}
					if (module.Operations && module.Operations.length > 0) {
						if (!newModules[moduleName]) {
							newModules[moduleName] = modulesByModuleName;
						}
					}
				});
			});
			return newModules;
		}
	},
	data() {
		return {
			headersState: true,
			moduleMenuState: {},
			modelMenuState: {},
			modules: [],
			search: ""
		}
	},
	components: {
		'parameter': httpVueLoader('$urlbase$/File?path=vue/src/components/parameter.vue'),
		'json-table': httpVueLoader('$urlbase$/File?path=vue/src/components/json-table.vue')
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
					axios.post(this.getUrl(), this.data, { headers: app.headers })
						.then(response => {
							self.loading = false;
							self.response = {};
							self.response.data = response.data;
							self.response.headers = {};
							app.responseHeaders.forEach(function (responseHeader) {
								self.response.headers[responseHeader] = response.headers[responseHeader];
							});
						});
				}
			};
			this.$store.dispatch('addRequest', request);
			this.activate(request);
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
			return modelOf(obj);
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