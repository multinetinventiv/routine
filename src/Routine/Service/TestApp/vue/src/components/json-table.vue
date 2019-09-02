<template>
        <table class="table table-striped table-condensed table-bordered">
                <thead>
                        <tr>
                                <th v-for="column in table.columns">
                                        {{column}}
                                </th>
                        </tr>
                </thead>
                <tbody>
                        <tr v-for="row in table.rows">
                                <td v-for="(cell, index) in row">
                                        <json-cell :data="cell"></json-cell>
                                </td>
                        </tr>
                </tbody>
        </table>
</template>

<script>
        module.exports = {
                name: 'JsonTable',
                props: ['data'],
                data() {
                        return {
                                dataTable: []
                        }
                },
                computed: {
                        table() {
                                return this.buildTable(this.data);
                        }
                },
                components: {
                        'json-cell': httpVueLoader('$urlbase$/File?path=vue/src/components/json-cell.vue')
                },
                methods: {
                        buildTable: function (data) {
                                console.log(data);
                                if (!_.isArray(data)) {
                                        data = [data];
                                }

                                var result = {
                                        columns: [],
                                        rows: []
                                };

                                for (var i = 0; i < data.length; i++) {
                                        if (!_.isObject(data[i])) {
                                                data[i] = { Value: data[i] };
                                        }
                                }
                                for (var i = 0; i < data.length; i++) {
                                        var row = data[i];
                                        for (var column in row) {
                                                if (column === "Data") {
                                                        for (var dcolumn in row[column]) {
                                                                if (!_.includes(result.columns, dcolumn)) {
                                                                        result.columns.push(dcolumn);
                                                                }
                                                        }
                                                } else if (!_.includes(result.columns, column)) {
                                                        result.columns.push(column);
                                                }
                                        }
                                }

                                for (var i = 0; i < data.length; i++) {
                                        var row = data[i];
                                        var tr = [];
                                        for (var j = 0; j < result.columns.length; j++) {
                                                var column = result.columns[j];
                                                if (row == null) {
                                                        tr.push("null");
                                                } else if (row.hasOwnProperty(column)) {
                                                        tr.push(row[column]);
                                                } else if (row.hasOwnProperty("Data") && row.Data.hasOwnProperty(column)) {
                                                        tr.push(row.Data[column]);
                                                } else {
                                                        tr.push("");
                                                }
                                        }
                                        result.rows.push(tr);
                                }
                                return result;
                        }
                },
        }
</script>