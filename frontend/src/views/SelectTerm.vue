<template>
  <v-container>
    <v-row :justify="'center'" :align-content="'center'">
      <v-col :cols=12 :align-self="'center'">
        <h1>Select a Term</h1>
      </v-col>
    </v-row>
  </v-container>
  
</template>

<style scoped>

</style>

<script>
export default {
  name: 'SelectTerm',
  data() {
    return {
      align: 'center'
    }
  },
}
</script>