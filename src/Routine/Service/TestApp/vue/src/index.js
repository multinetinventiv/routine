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
        modules() {
            return store.state.modules;
        },
        filteredData() {
            return this.filterOperations();
        }

    },
    data() {
        return {
            headersState: true,
            moduleMenuState: {},
            modelMenuState: {},
            search: ""
        }
    },
    components: {
        'parameter': httpVueLoader('$urlbase$/TestApp/File?path=vue/src/components/parameter.vue'),
        'json-table': httpVueLoader('$urlbase$/TestApp/File?path=vue/src/components/json-table.vue')
    },
    mounted() {
        this.$store.dispatch('loadConfiguration');
        this.$store.dispatch('loadApplicationModel');
    },
    methods: {
        filterOperations() {
            Object.keys(this.modules).forEach(moduleName => {
                const models = this.modules[moduleName];
                models.map(model => {
                    let operations = model.Operations.filter(operation => {
                        operation.IsShow = true;
                        model.IsShow = true;
                        if (this.search !== "") {
                            const searchStartCase = _.startCase(operation.Name);

                            let pattern = '^', arr = this.search.split('').join(' ').trim().split(' ');
                            arr.forEach(function (chars, i) {
                                pattern += chars + '\\w*' + (arr.length - 1 > i ? '\\s+' : '');
                            });

                            const result = searchStartCase.match(new RegExp(pattern, 'i'));

                            if (result && result.length > 0) {
                                operation.IsShow = true;
                                return true;
                            } else {
                                if (this.search.length > 2) {
                                    if (_.includes(operation.Name.toLowerCase(), this.search.toLowerCase())) {
                                        operation.IsShow = true;
                                        return true;
                                    }
                                }
                                operation.IsShow = false;
                                return false;
                            }
                        }
                        return true;
                    });
                    if (operations && operations.length > 0) {
                        model.IsShow = true;
                    }
                    else {
                        model.IsShow = false;
                    }

                });
            });
            return this.modules;
        },
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
                    let result = `${config.URL_BASE}/${this.target.ModelId}`;

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
                    const self = this;
                    this.loading = true;
                    axios.post(this.getUrl(), this.data, { headers: app.headers })
                        .then(response => {
                            self.loading = false;
                            self.response = {};
                            self.response.data = response.data;
                            self.response.headers = {};
                            app.responseHeaders.forEach(function (responseHeader) {
                                self.response.headers[responseHeader] = response.headers[responseHeader.toLowerCase()];
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

        toggleModuleMenuState: function (moduleName) {
            this.$set(this.moduleMenuState, moduleName, !this.moduleMenuState[moduleName]);
        },

        toggleModelMenuState: function (modelName) {
            this.$set(this.modelMenuState, modelName, !this.modelMenuState[modelName]);
        },

        getModelsWithOperation: function (moduleModels) {
            return moduleModels.filter(moduleModel => {
                return moduleModel.Operations && moduleModel.Operations.length > 0;
            });
        },

        hasOperation(model) {
            return _.some(model.Operations, function (operation) {
                return operation.IsShow;
            });
        }

    }
});