<template>
	<div>
		<div class="input-group" v-if="data == null">
			<p class="form-control"><em>null</em></p>
			<span class="input-group-btn" @click="setValue({i:i})">
				<button class="btn btn-default" type="button"><i class="glyphicon glyphicon-plus"></i></button>
			</span>
		</div>
		<div class="input-group" v-if="data != null && model && model.StaticInstances.length === 1 && model.Initializer.Parameters.length <= 0">
			<p class="form-control">
				<span>{{model.StaticInstances[0].Display}}</span>
			</p>
			<span class="input-group-btn" @click="unsetValue({i:i})">
				<button class="btn btn-default" type="button"><i class="glyphicon glyphicon-remove"></i></button>
			</span>
		</div>
		<div class="input-group" v-if="data != null && model && model.StaticInstances.length > 1 && model.Initializer.Parameters.length <= 0">
			<select class="form-control"
					v-model="data.Id">
				<option v-for="instance in model.StaticInstances"
						value="{{instance.Id}}">
					{{instance.Display}}
				</option>
			</select>
			<span class="input-group-btn" @click="unsetValue({i:i})">
				<button class="btn btn-default" type="button"><i class="glyphicon glyphicon-remove"></i></button>
			</span>
		</div>
		<div class="input-group" v-if="data != null && model && model.StaticInstances.length <= 0 && model.Initializer.Parameters.length <= 0">
			<input type="text" class="form-control"
				   v-bind:placeholder="model.IsValueModel?'Value':'Id'"
				   v-model="data.Id" />
			<span class="input-group-btn" @click="unsetValue({i:i})">
				<button class="btn btn-default" type="button"><i class="glyphicon glyphicon-remove"></i></button>
			</span>
		</div>

	</div>
</template>

<script>
	module.exports = {
		name: 'ParameterValue',
		props: ['model', 'data', 'i'],
		mounted() {
			this.addValue();
			console.log(this.model);
			console.log(this.data);
			console.log(this.i);
		},
		data: function () {
			return {
				who: 'world'
			}
		},
		methods: {
			modelOfW: function (obj) {
				return $emit('modelOf', obj);
			}
		}

	}
</script>