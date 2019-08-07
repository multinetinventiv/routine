Vue.filter('splitCamelCase', function (value) {
	if (value) {
		return value
			.replace(/([.])/g, ' /')
			.replace(/([A-Z])/g, ' $1')
			.replace(/^./, function (str) {
				return str.toUpperCase();
			});
	}
})