<template>
	<span v-if="!(_.isObject(data))">{{data}}</span>
</template>

<script>
	module.exports = {
		name: 'JsonCell',
		props: ['data']
	}
</script>