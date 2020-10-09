<template>
  <div>
    <v-container v-if="!loadingResults">
      <div v-if="!error">
        <BackButton />
        <v-row class="mb-10">
          <v-col cols="12">
            <h1>Click/tap to select your desired sections.</h1>
          </v-col>
        </v-row>
          <div v-for="courses in courseResults" :key="courses.searchModel.subjectCode + courses.searchModel.courseNumber">
            <div><h2 class="font-weight-regular">{{courses.searchModel.subjectCode}} {{courses.searchModel.courseNumber}} - {{courses.searchModel.courseTitle}}</h2></div>
              <v-container v-if="courses.results.length > 0" class="px-5">
                <v-row>
                  <v-col cols="12" lg="6" xl="4" v-for="course in courses.results" :key="course.courseRegistrationNum" class="d-flex" style="flex-direction:column">
                      <v-card
                      :color="isCourseSelected(course) ? 'primary' : ''"
                      :dark="isCourseSelected(course)"
                      :disabled="courseDisabled(course) || checkForScheduleConflict(course)"
                      @click="selectedCourseUpdate(course, !isCourseSelected(course))"
                      class="mb-1 flex-grow-1"
                      :class="courseDisabled(course) ? 'text-decoration-line-through' : ''"
                      outlined
                      hover
                      >
                      <v-card-title>
                        <h4>{{course.section}}</h4>
                        <template v-if="course.topic != null"> - {{course.topic}}</template>
                      </v-card-title>
                      <v-card-subtitle>
                        <v-chip pill light><v-icon left>mdi-briefcase-account</v-icon><h5>{{course.instructor}}</h5></v-chip></v-card-subtitle>
                      <v-card-text>
                        <div>
                          <h4>Remaining Seats: {{course.remainingSeats}}</h4>
                          <h4>Waitlist Remaining: {{course.remainingWaitlist}}</h4>
                        </div>
                        <div>
                          <v-simple-table dense light>
                            <thead>
                              <tr>
                                <th>Days</th>
                                <th>Times</th>
                                <th>Location</th>
                              </tr>
                            </thead>
                            <tbody>
                              <template v-if="course.meetings !== null">
                                <tr v-for="index in course.meetings.length" :key="course.courseRegistrationNum + course.meetings[index - 1].days">
                                <td>{{course.meetings[index - 1].days.join("")}}</td>
                                <td>{{course.meetings[index - 1].startTime.twelveHourTime}} - {{course.meetings[index - 1].endTime.twelveHourTime}}</td>
                                <td>{{course.meetings[index - 1].location}}</td>
                              </tr>
                              </template>
                              <template v-if="course.meetings === null">
                                <tr>
                                  <td v-if="course.section.includes('WW')">Online</td>
                                  <td v-else>TBA</td>
                                  <td>N/A</td>
                                  <td>N/A</td>
                                </tr>
                              </template>
                            </tbody>
                          </v-simple-table>
                        </div>
                      </v-card-text>
                      </v-card>
                  </v-col>
                </v-row>
              </v-container>
            <v-container v-if="courses.results.length == 0">
              <h4 class="red--text text--accent-4">No matching courses found.</h4>
            </v-container>
            <v-divider inset class="my-3"></v-divider>
          </div>
          <v-row>
            <v-col offset="1" cols="10" md="4" offset-md="4" v-if="canContinue">
              <v-btn block color="primary" x-large @click="continueButtonClick()">Continue</v-btn>
            </v-col>
          </v-row>
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
    <v-container v-if="loadingResults" class="mt-5">
      <v-row justify="center" align-content="center" class="mb-5">
        <v-progress-circular indeterminate size="250" width="5" color="primary"></v-progress-circular>
      </v-row>
      <v-row justify="center" class="mt-5">
        <h2>Searching for matching courses...</h2>
      </v-row>
    </v-container>
  </div>
</template>

<style>
  .theme--light.v-sheet--outlined {
    border-top-color: rgba(0, 0, 0, 0.4);
    border-top-style: solid;
    border-top-width: thin;
    border-right-color: rgba(0, 0, 0, 0.4);
    border-right-style: solid;
    border-right-width: thin;
    border-bottom-color: rgba(0, 0, 0, 0.4);
    border-bottom-style: solid;
    border-bottom-width: thin;
    border-left-color: rgba(0, 0, 0, 0.4);
    border-left-style: solid;
    border-left-width: thin;
    border-image-source: initial;
    border-image-slice: initial;
    border-image-width: initial;
    border-image-outset: initial;
    border-image-repeat: initial;
  }
</style>

<script>
import BackButton from '../global-components/BackButton'
export default {
  name: "CourseResults",
  components: {
    BackButton
  },
  data() {
    return {
      error: false,
      loadingResults: true,
      courseResults: []
    };
  },
  computed: {
    selectedCourses: function () {
      return this.$store.getters.selectedCourses;
    },
    selectedTerm: function () {
      return this.$store.getters.termId;
    },
    selectedResults: function() {
      return this.$store.getters.selectedResults;
    },
    canContinue: function() {
      return Object.keys(this.selectedResults).length > 0
    }
  },
  methods: {
    selectedCourseUpdate(course, active) {
      if(active) {
        const conflict = this.checkForScheduleConflict(course);
        if(conflict) {
          return;
        }
        this.$set(this.selectedResults, `${course.subjectCode + course.courseNumber}`, course);
      }
      else {
        this.$delete(this.selectedResults, `${course.subjectCode + course.courseNumber}`)
      }
    },
    courseDisabled(course) {
      return course.remainingWaitlist <= 0 && course.remainingSeats <= 0
    },
    continueButtonClick() {
      this.$router.push('/confirm-schedule')
    },
    isCourseSelected(course) {
      return Object.values(this.selectedResults).filter(c => c.courseRegistrationNum === course.courseRegistrationNum).length === 1
    },
    //lord forgive me for I have sinned.
    checkForScheduleConflict(course) {
      const currentSchedule = Object.values(this.selectedResults);
      for(let i = 0; i < currentSchedule.length; i++) {
        if(!(course.subjectCode == currentSchedule[i].subjectCode && course.courseNumber == currentSchedule[i].courseNumber) && currentSchedule[i].meetings != null && course.meetings != null) {
          for(let k = 0; k < currentSchedule[i].meetings.length; k++) {
            for(let l = 0; l < course.meetings.length; l++) {
              const existingCourse = currentSchedule[i].meetings[k];
              const courseMeeting = course.meetings[l];
              if(courseMeeting.startTime.value < existingCourse.endTime.value && existingCourse.startTime.value < courseMeeting.endTime.value) {
                for(let a = 0; a < courseMeeting.days.length; a++) {
                  for(let b = 0; b < existingCourse.days.length; b++) {
                    if(courseMeeting.days[a] == existingCourse.days[b]) {
                      return true;
                    }
                  }
                }
              }
            }
          }
        }
      }
    return false;
    }
  },
  created() {
    if(this.selectedTerm == 0) {
      this.$router.push('/')
    } else if(this.selectedCourses.length == 0) {
      this.$router.push('select-courses')
    }
    this.$http
      .post(`/api/search-courses?termId=${this.selectedTerm}`, this.selectedCourses)
      .then((res) => {
        this.courseResults = res.data;
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