<template>
  <v-container>
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
      >
        <v-card color="#696868" dark>
          <v-card-title class="headline">{{course.subjectCode + " " + course.courseNumber}}</v-card-title>
          <v-card-subtitle>{{course.courseTitle}}</v-card-subtitle>
          <v-card-actions>
            <v-btn text color="#630a0a" @click="removeCourse(course)">Remove</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-row v-if="canContinue">
      <v-col>
        <v-btn color="primary" x-large @click="continueButtonClicked()">Continue</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: "CourseSelection",
  data() {
    return {
      subjects: [],
      courses: [],
      selectedSubject: "",
      selectedCourse: {},
      courseInfoLoading: false,
    };
  },
  computed: {
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
    },
  },
  created() {
    if(this.$store.getters.termId === 0) {
      this.$router.push('/')
    }
    this.getSubjects();
  },
};
</script>