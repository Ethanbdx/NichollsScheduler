<template>
  <div class="my-8" style="height: 60%">
    <v-container v-if="doneLoading">
      <template v-if="!error">
        <v-row>
          <v-col cols="12">
            <h1 text-center>What term are you scheduling for?</h1>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" lg="6">
            <v-select
              :items="terms"
              item-text="termName"
              item-value="termId"
              label="Select a term"
              v-model="termId"
              type="'number'"
              outlined
            ></v-select>
          </v-col>
        </v-row>
        <template v-if="termSelected">
          <v-row>
      <v-col cols="12">
        <h1 text-center>What courses do you want to search for?</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" lg="3">
        <v-select
          :items="subjects"
          v-model="selectedSubject"
          item-text="fullSubject"
          item-value="subjectCode"
          label="Select a Subject"
          @change="getCoursesInfo()"
          outlined
        ></v-select>
      </v-col>
      <v-col cols="12" lg="3">
        <v-select
          :items="courses"
          v-model="selectedCourse"
          item-text="courseNumber"
          label="Select a Course Number"
          :disabled="!courseNumberEnabled"
          :loading="courseInfoLoading"
          return-object
          outlined
        ></v-select>
      </v-col>
      <v-col cols="12" lg="3">
        <v-btn
          block
          raised
          x-large
          color="primary"
          :disabled="!canAddCourse"
          @click="addCourseButtonClick()"
        >Add Course to List</v-btn>
      </v-col>
    </v-row>
    <hr />
    <v-row>
      <v-col>
        <h1 text-center>Selected Courses</h1>
      </v-col>
    </v-row>
    <v-row dense>
      <v-col
        v-for="course in selectedCourses"
        :key="course.subjectCode + course.courseNumber"
        cols="12"
        sm="6"
        md="4"
        lg="3"
        class="d-flex" style="flex-direction:column"
      >
        <v-card color="#696868" dark class="mb-1 flex-grow-1">
          <v-card-title class="headline">{{course.subjectCode + " " + course.courseNumber}}</v-card-title>
          <v-card-subtitle>{{course.courseTitle}}</v-card-subtitle>
          <v-card-actions>
            <v-btn text color="#630a0a" @click="removeCourse(course)">Remove</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-row v-if="canContinue" class="my-4">
      <v-col cols="12" md="4" offset-md="4">
        <v-btn block color="primary" x-large @click="continueButtonClicked()" >
          Continue</v-btn>
      </v-col>
    </v-row>
        </template>
      </template>
      <template v-if="error">
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
      </template>
    </v-container>
    <v-container v-if="!doneLoading" style="height: 100%">
      <v-row style="height: 100%" justify="center" align-content="center">
        <v-progress-circular indeterminate size="250" width="5" color="primary"></v-progress-circular>
      </v-row>
    </v-container>
  </div>
</template>

<style scoped>
</style>

<script>
export default {
  name: "Select",
  data() {
    return {
      terms: [],
      error: false,
      doneLoading: false,
      subjects: [],
      courses: [],
      selectedSubject: "",
      selectedCourse: {},
      courseInfoLoading: false,
    };
  },
  computed: {
    termSelected: function () {
      return this.termId != 0;
    },
    termId: {
      get: function () {
        return this.$store.getters.termId;
      },
      set: function (value) {
        this.$store.commit("setTermId", value);
      }
    },
    canAddCourse: function () {
      return Object.keys(this.selectedCourse).length != 0 && !this.selectedCourses.some(c => c.subjectCode === this.selectedSubject && c.courseNumber === this.selectedCourse.courseNumber)
    },
    courseNumberEnabled: function () {
      return this.selectedSubject.length != 0;
    },
    canContinue: function () {
      return this.selectedCourses.length != 0;
    },
    selectedCourses: function () {
      return this.$store.getters.selectedCourses;
    },
  },
  methods: {
    getTerms: function () {
      this.$http
        .get("/api/get-available-terms")
        .then((res) => {
          this.terms = res.data;
        })
        .catch((err) => {
          console.log(err);
          this.error = true;
        })
        .finally(() => {
          this.doneLoading = true;
        });
    },
    getSubjects: function () {
      this.$http
        .get("/api/get-course-subjects")
        .then((res) => {
          this.subjects = res.data;
        })
        .catch((err) => {
          console.log(err);
        });
    },
    getCoursesInfo: function () {
      this.courseInfoLoading = true;
      this.courses = [];
      this.$http
        .get(`/api/get-courses-info?subject=${this.selectedSubject}`)
        .then((res) => {
          this.courses = res.data;
        })
        .catch((err) => {
          console.log(err);
        })
        .finally(() => {
          this.courseInfoLoading = false;
        });
    },
    addCourseButtonClick: function () {
      this.selectedCourse.subjectCode = this.selectedSubject;
      this.selectedCourses.push(this.selectedCourse);

      //clear selections after adding the course.
      this.selectedSubject = "";
      this.selectedCourse = {};
    },
    removeCourse: function (course) {
      let index = this.selectedCourses.indexOf(course);
      this.selectedCourses.splice(index, 1);
    },
    continueButtonClicked: function () {
      this.$store.commit("setSelectedCourses", this.selectedCourses);
      this.$router.push("/course-results");
    }
  },
  created() {
    this.getTerms();
    this.getSubjects();
  },
};
</script>