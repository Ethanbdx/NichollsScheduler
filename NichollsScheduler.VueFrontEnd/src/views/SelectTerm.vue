<template>
  <v-container>
    <v-row :justify="'center'" :align-content="'center'">
      <v-col :cols=12 :align-self="'center'">
        <h1>Select a Term</h1>
        <v-select :items="data" label="Term" solo></v-select>
        <button @click="getTerms()">Click me!</button>
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
      data: [{text: 'Test', value: 2}, {text: 'Test 2', value: 1}],
      terms: null
    }
  },
  methods: {
    getTerms: function() {
      this.$http.get('https://localhost:5001/api/get-available-terms').then(res => (this.terms = res.data))
    } 
  }
}
</script>