<template>
  <div class="my-8" style="height: 60%">
    <v-container v-if="!loadingResults">
      <div v-if="error">
        <v-row>
          <v-col cols="12">
            <h1 class="red--text text--accent-4">:(</h1>
            <h2 class="red--text text--accent-4">There was an error getting the available terms.</h2>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <h2>Please try again later.</h2>
          </v-col>
        </v-row>
      </div>
    </v-container>
    <v-container v-if="loadingResults" style="height: 100%">
      <v-row style="height: 100%" justify="center" align-content="center">
        <v-progress-circular indeterminate size="250" width="5" color="primary"></v-progress-circular>
      </v-row>
      <v-row justify="center">
        <h2>Searching for matching courses...</h2>
      </v-row>
    </v-container>
  </div>
</template>

<script>
export default {
  name: "CourseResults",
  data() {
    return {
      error: false,
      loadingResults: true,
      results: [],
    };
  },
  computed: {
    selectedCourses: function () {
      return this.$store.getters.selectedCourses;
    },
    selectedTerm: function () {
      return this.$store.getters.termId;
    },
  },
  created() {
    this.$http
      .post(
        `/api/search-courses?termId=${this.selectedTerm}`,
        this.selectedCourses
      )
      .then((res) => {
        this.results = res.data;
      })
      .catch((err) => {
        console.log(err);
      })
      .finally(() => {
        this.loadingResults = false;
      });
  },
};
</script>