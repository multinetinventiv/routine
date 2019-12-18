<template>
        <div>
                <span v-if="!(_.isObject(data))">{{data}}</span>
                <json-table v-if="isShow" :data="data"></json-table>
        </div>
</template>

<script>
        module.exports = {
                name: 'JsonCell',
                props: ['data'],
                data: function () {
                        return {
                                isShow: false
                        }
                },
                components: {
                        'json-table': httpVueLoader('$urlbase$/File?path=vue/src/components/json-table.vue')
                },
                mounted() {
                        this.setChildTable();
                },
                methods: {
                        setChildTable() {
                                if (_.isObject(this.data)) {
                                        this.isShow = true;
                                }
                        }
                }
        }
</script>