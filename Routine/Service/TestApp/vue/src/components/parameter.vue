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
		<pre>{{getItems()}}</pre>
		<div class="panel-body" v-if="getItems() && getItems().length > 0">
			<div class="row" v-for="(item, index) in getItems()">
				<pre>{{item}}</pre>
				<div class="col-xs-12" v-if="index > 0"><hr /></div>
				<div class="col-xs-12" v-if="viewmodel.IsViewModel && item != null">
					<div class="form-group">
						<select class="form-control"
								v-if="viewmodel.ActualModelIds.length > 1"
								v-model="item.ModelId">
							<option v-bind:value="modelId"
									v-for="modelId in viewmodel.ActualModelIds">
								{{$emit('modelOf', modelId).Name | splitCamelCase}}
							</option>
						</select>
						<p class="form-control" v-if="viewmodel.ActualModelIds.length === 1">
							<span>{{$emit('modelOf', viewmodel.ActualModelIds[0]).Name | splitCamelCase}}</span>
						</p>
					</div>
				</div>
				<div class="col-xs-12">
					<div class="form-group nopad-b">
						<parameter-value :data="item"
										 :i="index"
										 :model="currentModel(item)"
										 @modelOf="modelOf"></parameter-value>
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
		mounted() {
			this.addValue();
		},
		methods: {
			createEmptyItem: function () {
				if (!this.viewmodel.IsViewModel) {
					return {};
				}

				if (this.viewmodel.ActualModelIds.length === 1) {
					return { ModelId: $emit('modelOf', scope.viewmodel.ActualModelIds[0]).Id };
				}

				return {};
			},

			getItems: function () {
				var pmodel = this.pmodel;
				var data = this.data;
				if (pmodel.IsList) {
					return data[pmodel.Name];
				}

				console.log(this.data);
				//console.log(this.childData.hasOwnProperty(pmodel.Name));

				if (!data.hasOwnProperty(pmodel.Name)) {
					return [];
				}

				return [data[pmodel.Name]];
			},

			hasOwnProperty: function (obj, prop) {
				Object.keys(obj).forEach(key => {
					if (key == prop) {
						return true;
					}
				});
				return false;
			},

			addValue: function (i) {
				var pmodel = this.pmodel;
				var data = this.data;

				if (pmodel.IsList) {
					if (data[pmodel.Name] === undefined) {
						this.reactiveSet(data, pmodel.Name, []);
					}

					if (i === undefined) {
						data[pmodel.Name].push(this.createEmptyItem());
					} else {
						data[pmodel.Name][i] = this.createEmptyItem();
					}
				} else {
					if (data[pmodel.Name] === undefined || data[pmodel.Name] == null) {
						this.reactiveSet(data, pmodel.Name, this.createEmptyItem());
					};
				}
			},
			removeValue: function () {
				var pmodel = this.pmodel;
				var data = this.data;

				if (pmodel.IsList) {
					if (data[pmodel.Name] === undefined) {
						return;
					} else if (data[pmodel.Name].length <= 0) {
						delete data[pmodel.Name];

						return;
					}

					if (i === undefined) {
						data[pmodel.Name].splice(data[pmodel.Name].length - 1, 1);
					} else {
						data[pmodel.Name][i] = null;
					}
				} else {
					if (data.hasOwnProperty(pmodel.Name) && i === undefined) {
						delete data[pmodel.Name];

						return;
					} else if (data[pmodel.Name] === undefined) {
						return;
					}

					data[pmodel.Name] = null;
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
					return $emit('modelOf', item.ModelId);
				}
				return viewmodel;
			},
			reactiveSet: function (obj, key, value) {
				this.$set(obj, key, value);
			}

		}
	}
</script>