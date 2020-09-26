<template>
  <v-row>
        <v-col>
            <v-btn dark color="secondary" @click="goBack()"><v-icon left>mdi-arrow-left</v-icon>Go back</v-btn>
        </v-col>
    </v-row>
</template>

<script>
export default {
    name: 'BackButton',
    methods: {
        goBack() {
            this.$router.go(-1)
        }
    }
}
</script>

<style>

</style>