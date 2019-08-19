function modelOf(obj) {
	var id = obj;

	if (_.isUndefined(id)) {
		return null;
	}

	if (!_.isString(id)) {
		id = obj.ModelId;
	}

	return _.find(this.store.state.models, function (item) {
		return item['Id'] === id;
	});
}
