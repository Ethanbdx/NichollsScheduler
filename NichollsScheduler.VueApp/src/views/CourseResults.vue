<template>
  <div class="my-8" style="height: 60%">
    <v-container v-if="!loadingResults">
      <div v-if="!error">
        <v-expansion-panels>
          <v-expansion-panel v-for="courses in results" :key="courses[0].searchModel.subjectCode + courses[0].searchModel.courseNumber">
            <v-expansion-panel-header><h1>{{courses[0].searchModel.subjectCode}} {{courses[0].searchModel.courseNumber}} - {{courses[0].searchModel.courseTitle}}</h1></v-expansion-panel-header>
              <v-expansion-panel-content>
                <v-row>
                  <v-col cols="4" v-for="course in courses" :key="`${course.registrationNumber}.${course.subjectCode + course.courseNumber}`">
                    <v-card color="secondary">
                      <v-card-title class="headline">{{course}}</v-card-title>
                    </v-card>
                  </v-col>
                </v-row>
              </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </div>
      <div v-if="error">
        <v-row>
          <v-col cols="12">
            <h1 class="red--text text--accent-4">:(</h1>
            <h2 class="red--text text--accent-4">There was an error finding matches.</h2>
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
    if(this.$store.getters.termId == 0) {
      this.$router.push('/')
    } else if(this.$store.getters.selectedCourses.length == 0) {
      this.$router.push('select-courses')
    }
    this.$store.commit('setCurrentStep', 3)
    this.$http
      .post(`/api/search-courses?termId=${this.selectedTerm}`, this.selectedCourses)
      .then((res) => {
        this.results = res.data;
      })
      .catch((err) => {
        console.log(err);
        this.error = true;
      })
      .finally(() => {
        this.loadingResults = false;
      });
  },
};
</script>