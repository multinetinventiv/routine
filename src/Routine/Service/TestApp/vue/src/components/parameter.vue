<template>
	<div class="panel panel-default">
		<div class="panel-heading panel-heading-btn">
			{{pmodel.Name | splitCamelCase}}
			<span class="badge" v-for="mark in marks">{{mark | splitCamelCase}}</span>
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
										 @unset-value="removeValue"
										 @model-of="modelOf"></parameter-value>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	module.exports = {
		name: 'Parameter',
		props: ['pmodel', 'data', 'viewmodel', 'marks'],
		components: {
			'parameter-value': httpVueLoader('$urlbase$/File?path=vue/src/components/parameter-value.vue')
		},
		created() {
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
					if (data[pmodel.Name] === undefined) {
						this.$set(data, pmodel.Name, []);
					}

					if (i === undefined) {
						this.$set(data, pmodel.Name, this.createEmptyItem());
					} else {
						this.$set(data[pmodel.Name], i, this.createEmptyItem());
					}
				} else {
					if (data[pmodel.Name] === undefined || data[pmodel.Name] == null) {
						this.$set(data, pmodel.Name, this.createEmptyItem());
					};
				}

			},
			removeValue: function (i) {
				var pmodel = this.pmodel;
				var data = this.data;
				console.log(data);
				if (pmodel.IsList) {
					if (data[pmodel.Name] === undefined) {
						return;
					} else if (data[pmodel.Name].length <= 0) {
						this.$delete(data, pmodel.Name);
						return;
					}

					if (i === undefined) {
						data[pmodel.Name].splice(data[pmodel.Name].length - 1, 1);

					} else {
						this.reactiveSet(data[pmodel.Name], i, null);
					}
				} else {
					if (data.hasOwnProperty(pmodel.Name) && i === undefined) {
						this.$delete(data, pmodel.Name);

						return;
					} else if (data[pmodel.Name] === undefined) {
						return;
					}

					this.reactiveSet(data, pmodel.Name, null);

				}
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
			},

			reactiveSet: function (obj, key, value) {
				this.$set(obj, key, value);
			},

			modelOf: function (obj) {
				var id = obj;

				if (id === undefined) {
					return null;
				}

				if (!(typeof id === 'string')) {
					id = obj.ModelId;
				}

				return _.find(this.$store.state.models, function (item) {
					return item['Id'] === id;
				});
			}
		}
	}
</script>