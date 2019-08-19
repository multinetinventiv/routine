window.store = new Vuex.Store({
	state: {
		responseHeaders: [],
		models: [],
		headers: {},
		modelsloading: true,
		headersloading: true,
		requests: []
	},
	actions: {

		loadConfiguration({ commit }) {

			axios.get("$urlbase$/Configuration").then(response => {
				let data = response.data;

				commit('SET_HEADERS_LOADING', false);

				let headers = {};
				data.requestHeaders.forEach(function (requestHeader) {
					headers[requestHeader] = "";
				});

				commit('SET_REQUEST_HEADERS', headers);
				commit('SET_RESPONSE_HEADERS', data.responseHeaders);
			});
		},

		loadApplicationModel({ commit }) {

			axios.get("$urlbase$/ApplicationModel").then(response => {
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
		}
	},
	mutations: {
		SET_REQUEST_HEADERS(state, headers) {
			state.headers = headers;
		},

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
		},

		CHANGE_MODELS_LOAIDING(state, modelsloading) {
			state.modelsloading = modelsloading;
		}
	}
});