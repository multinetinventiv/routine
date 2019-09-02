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
                mounted() {
                        this.initComponent();
                        isShow = false
                },
                methods: {
                        initComponent() {
                                if (_.isObject(this.data)) {
                                        console.log('parameter:bla');
                                        this.isShow = true;
                                }
                        }
                }
        }
</script>