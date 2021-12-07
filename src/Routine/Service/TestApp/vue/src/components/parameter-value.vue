<template>
    <div>
        <div class="input-group" v-if="data == null">
            <p class="form-control"><em>null</em></p>
            <span class="input-group-btn" @click="$emit('set-value',i)">
                <button class="btn btn-default" type="button"><i class="glyphicon glyphicon-plus"></i></button>
            </span>
        </div>
        <div class="input-group" v-if="data != null && model && model.StaticInstances.length === 1 && model.Initializer.Parameters.length <= 0">
            <p class="form-control">
                <span>{{model.StaticInstances[0].Display}}</span>
            </p>
            <span class="input-group-btn" @click="$emit('unset-value',i)">
                <button class="btn btn-default" type="button"><i class="glyphicon glyphicon-remove"></i></button>
            </span>
        </div>
        <div class="input-group" v-if="data != null && model && model.StaticInstances.length > 1 && model.Initializer.Parameters.length <= 0">
            <select class="form-control"
                    v-model="data.Id">
                <option v-for="instance in model.StaticInstances" v-bind:value="instance.Id">
                    {{instance.Display}}
                </option>
            </select>
            <span class="input-group-btn" @click="$emit('unset-value',i)">
                <button class="btn btn-default" type="button"><i class="glyphicon glyphicon-remove"></i></button>
            </span>
        </div>
        <div class="input-group" v-if="data != null && model && model.StaticInstances.length <= 0 && model.Initializer.Parameters.length <= 0">
            <input type="text" class="form-control"
                   v-bind:placeholder="model.IsValueModel?'Value':'Id'"
                   v-model="data.Id" />
            <span class="input-group-btn" @click="$emit('unset-value',i)">
                <button class="btn btn-default" type="button"><i class="glyphicon glyphicon-remove"></i></button>
            </span>
        </div>
        <div v-if="_.has(model,'Initializer') && model.Initializer!=null">
            <parameter v-for="parameter in model.Initializer.Parameters"
                       :pmodel="parameter"
                       :viewmodel="modelOf(parameter.ViewModelId)"
                       :data="data.Data">
            </parameter>
        </div>
    </div>

</template>

<script>
    module.exports = {
        name: 'ParameterValue',
        props: ['model', 'data', 'i'],
        data() {
            return {
                recursiveDirectiveAdded: undefined,
            }
        },
        components: {
            parameter: httpVueLoader('$urlbase$/$testapp$/File?path=vue/src/components/parameter.vue')
        },

        watch: {
            model: {
                immediate: true,
                handler(newModel, oldModel) {
                    if (newModel == null) {
                        return;
                    }

                    if (this.data != null) {
                        if (newModel.StaticInstances.length === 1) {
                            this.data.Id = newModel.StaticInstances[0].Id;
                        } else {
                            delete this.data.Id;
                        }
                    }

                    if (newModel.StaticInstances.length <= 0 && newModel.Initializer.Parameters.length > 0) {
                        if (this.data != null) {
                            this.data.Data = {};
                            delete this.data.Id;
                        }
                        if (_.isUndefined(this.recursiveDirectiveAdded)) {
                            this.recursiveDirectiveAdded = true;
                        }
                    } else if (newModel.Initializer.Parameters.length <= 0) {
                        if (this.data != null) {
                            delete this.data.Data;
                        }
                    }
                }
            }
        }
    }
</script>