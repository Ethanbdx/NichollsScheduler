<template>
  <v-container>
    <div v-if="doneLoading">
      <v-row :justify="'center'" :align-content="'center'">
      <v-col :cols=12 :align-self="'center'">
        <div v-if="!error">
        <h1>What term are you scheduling for?</h1>
        <v-select :items="terms" label="Select a Term" solo></v-select>
        </div>
        <h1 v-if="error">There was an error getting available terms :(</h1>
      </v-col>
    </v-row>
    </div>
    <div v-if="!doneLoading">
      <v-progress-circular
      indeterminate
      size="500"
      width="5"
      color="primary"
    ></v-progress-circular>
    </div>
  </v-container>
</template>

<style scoped>

</style>

<script>
export default {
  name: 'SelectTerm',
  data() {
    return {
      terms: [],
      error: false,
      doneLoading: false
    }
  },
  methods: {
    getTerms: function() {
      this.$http.get('https://localhost:5001/api/get-available-terms')
      .then(res => {
        for(let i = 0; i < res.data.length; i++) {
          res.data[i] = {text: res.data[i].termName, value: res.data[i].termId}
        }
        this.terms = res.data;
      })
      .catch(err => {
        console.log(err)
        this.error = true;
      })
      .finally(() => {
        this.doneLoading = true
      })
    } 
  },
  created() {
    this.getTerms();
  }
}
</script>