<template>
  <v-container></v-container>
</template>

<script>
export default {
    name: 'ConfirmSchedule',
    created() {
        this.$store.commit('setCurrentStep', 4)
    }
}
</script>

<style>

</style>