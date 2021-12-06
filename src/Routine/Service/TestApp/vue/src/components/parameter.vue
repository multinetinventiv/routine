<template>
    <div class="panel panel-default">
        <div class="panel-heading panel-heading-btn">
            {{pmodel.Name | splitCamelCase}}
            <span class="badge" v-for="mark in getFilteredMarks(pmodel.Marks)">{{mark | splitCamelCase}}</span>
            <button type="button"
                    class="btn btn-default btn-xs pull-right"
                    v-if="data.hasOwnProperty(pmodel.Name)"
                    @click="removeValue()">
                <i class="glyphicon glyphicon-minus"></i>
            </button>
            <button type="button"
                    class="btn btn-default btn-xs pull-right"
                    v-if="!data.hasOwnProperty(pmodel.Name) || pmodel.IsList"
                    @click="addValue()">
                <i class="glyphicon glyphicon-plus"></i>
            </button>
        </div>
        <div class="panel-body" v-if="getItems() && getItems().length > 0">
            <div class="row" v-for="(item, index) in getItems()">
                <div class="col-xs-12" v-if="index > 0"><hr /></div>
                <div class="col-xs-12" v-if="viewmodel.IsViewModel && item != null">
                    <div class="form-group">
                        <select class="form-control" v-if="viewmodel.ActualModelIds.length > 1" v-model="item.ModelId">
                            <option v-bind:value="modelId" v-for="modelId in viewmodel.ActualModelIds">{{modelOf(modelId).Name | splitCamelCase}}</option>
                        </select>
                        <p class="form-control" v-if="viewmodel.ActualModelIds.length === 1">{{modelOf(viewmodel.ActualModelIds[0]).Name | splitCamelCase}}</p>
                    </div>
                </div>
                <div class="col-xs-12">
                    <div class="form-group nopad-b">
                        <parameter-value :data="item"
                                         :i="index"
                                         :model="currentModel(item)"
                                         @set-value="addValue"
                                         @unset-value="removeValue"></parameter-value>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    module.exports = {
        name: 'Parameter',
        props: ['pmodel', 'data', 'viewmodel'],
        components: {
            'parameter-value': httpVueLoader('$urlbase$/TestApp/File?path=vue/src/components/parameter-value.vue')
        },
        mounted() {
            this.addValue();
        },
        methods: {
            createEmptyItem: function () {
                if (!this.viewmodel.IsViewModel) {
                    return {};
                }

                if (this.viewmodel.ActualModelIds.length === 1) {
                    return { ModelId: this.modelOf(this.viewmodel.ActualModelIds[0]).Id };
                }
                return {};
            },

            getItems: function () {
                var pmodel = this.pmodel;
                var data = this.data;

                if (pmodel.IsList) {
                    return data[pmodel.Name];
                }

                if (!data.hasOwnProperty(pmodel.Name)) {
                    return [];
                }
                return [data[pmodel.Name]];
            },

            addValue: function (i) {
                var pmodel = this.pmodel;
                var data = this.data;

                if (pmodel.IsList) {

                    if (_.isUndefined(data[pmodel.Name])) {
                        this.$set(data, pmodel.Name, []);
                    }

                    if (_.isUndefined(i)) {
                        data[pmodel.Name].push(this.createEmptyItem());
                    } else {
                        this.$set(data[pmodel.Name], i, this.createEmptyItem());
                    }
                } else {
                    if (_.isUndefined(data[pmodel.Name]) || data[pmodel.Name] == null) {
                        this.$set(data, pmodel.Name, this.createEmptyItem());
                    };
                }
                this.$forceUpdate();
            },
            removeValue: function (i) {
                var pmodel = this.pmodel;
                var data = this.data;
                if (pmodel.IsList) {
                    if (_.isUndefined(data[pmodel.Name])) {
                        this.$forceUpdate();
                        return;
                    } else if (data[pmodel.Name].length <= 0) {
                        this.$delete(data, pmodel.Name);
                        this.$forceUpdate();
                        return;
                    }

                    if (_.isUndefined(i)) {
                        data[pmodel.Name].splice(data[pmodel.Name].length - 1, 1);

                    } else {
                        this.$set(data[pmodel.Name], i, null);
                    }
                } else {
                    if (data.hasOwnProperty(pmodel.Name) && i === undefined) {
                        this.$delete(data, pmodel.Name);
                        this.$forceUpdate();
                        return;
                    } else if (data[pmodel.Name] === undefined) {
                        this.$forceUpdate();
                        return;
                    }

                    this.$set(data, pmodel.Name, null);

                }

                this.$forceUpdate();
            },

            currentModel: function (item) {
                var viewmodel = this.viewmodel;
                if (viewmodel.IsViewModel) {
                    if (item == null) {
                        return null;
                    }
                    if (!item.hasOwnProperty("ModelId")) {
                        return null;
                    }
                    return this.modelOf(item.ModelId);
                }
                return viewmodel;
            }
        }
    }
</script>