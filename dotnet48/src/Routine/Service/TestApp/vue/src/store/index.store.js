window.store = new Vuex.Store({
        state: {
                responseHeaders: [],
                models: [],
                modules: {},
                headers: {},
                modelsloading: true,
                headersloading: true,
                requests: []
        },
        actions: {

                loadConfiguration({ commit }) {
                        axios.get(`${config.URL_BASE}/Configuration`).then(response => {
                                const data = response.data;

                                commit('SET_HEADERS_LOADING', false);

                                const headers = {};
                                data.requestHeaders.forEach(function (requestHeader) {
                                        headers[requestHeader] = "";
                                });

                                commit('SET_REQUEST_HEADERS', headers);
                                commit('SET_RESPONSE_HEADERS', data.responseHeaders);
                        });
                },

                loadApplicationModel({ commit }) {

                        axios.get(`${config.URL_BASE}/ApplicationModel`).then(response => {
                                const data = response.data;
                                const models = data.Models;

                                commit('SET_MODELS_LOADING', false);

                                models.forEach(function (model) {
                                        model.Initializer.ModelId = model.Id;

                                        model.Datas.forEach(function (data) {
                                                data.ModelId = model.Id;
                                        });

                                        model.Operations.forEach(function (operation) {
                                                operation.ModelId = model.Id;
                                                operation.IsShow = true;
                                                operation.Parameters.forEach(function (parameter) {
                                                        parameter.ModelId = model.Id;
                                                        parameter.OperationName = operation.Name;
                                                });
                                        });
                                });

                                const modules = _.groupBy(models, 'Module');
                                const newModules = {};
                                Object.keys(modules).forEach(moduleName => {
                                        const modulesByModuleName = modules[moduleName];
                                        modulesByModuleName.forEach(module => {
                                                module.IsShow = true;
                                                if (module.Operations && module.Operations.length > 0) {
                                                        if (!newModules[moduleName]) {
                                                                newModules[moduleName] = modulesByModuleName;
                                                        }
                                                }
                                        });
                                });
                                commit('SET_APPLICATION_MODELS', models);
                                commit('SET_APPLICATION_MODULES', newModules);
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

                SET_APPLICATION_MODULES(state, modules) {
                        state.modules = modules;
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