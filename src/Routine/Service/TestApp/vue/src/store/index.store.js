const store = new Vuex.Store({
	state: {
		responseHeaders: [],
		models: [],
		headers: {},
		modelsloading: true,
		headersloading: true,
		requests:[]
	},
	actions: {

		loadConfiguration({ commit, state }) {

			axios.get("http://localhost:32805/Service/Configuration").then(response => {
				let data = response.data;

				commit('SET_HEADERS_LOADING', false);

				data.requestHeaders.forEach(function (requestHeader) {
					state.headers[requestHeader] = "";
				});
			});
		},

		loadApplicationModel({ commit }) {

			axios.get("http://localhost:32805/Service/ApplicationModel").then(response => {
				let data = response.data;
				let models = data.Models;

				commit('SET_MODELS_LOADING', false);

				models.forEach(function (model) {
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
				commit('SET_APPLICATION_MODELS', models);
			});
		},

		addRequest({ commit }, request) {
			commit('ADD_REQUEST', request);
		},


		modelOf(obj) {
			return new Promise((resolve, reject) => {
				var id = obj;

				if (id === undefined) {
					resolve(null);
				}

				if (!(typeof id === 'string')) {
					id = obj.ModelId;
				}
				console.log(state.models);
				resolve(_.find(state.models,
					function (item) {
						return item['Id'] === id;
					}));
			});
		}
	},
	mutations: {
		SET_RESPONSE_HEADERS(state, responseHeaders) {
			state.responseHeaders = responseHeaders;
		},

		SET_APPLICATION_MODELS(state, models) {
			state.models = models;
		},

		SET_MODELS_LOADING(state, modelsloading) {
			state.modelsloading = modelsloading;
		},

		SET_HEADERS_LOADING(state, headersloading) {
			state.headersloading = headersloading;
		},

		ADD_REQUEST(state, request) {
			state.requests.push(request);
		}
	}
});