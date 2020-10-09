<template>
  <v-container>
    <v-row>
          <v-col cols="12">
            <h1 text-center>Use these CRNs to register on Banner.</h1>
          </v-col>
        </v-row>
    <v-row>
      <v-col cols="12" lg="6" xl="4" v-for="course in desiredCourses" :key="course.courseRegistrationNum" class="d-flex" style="flex-direction:column">
                      <v-card
                      class="mb-1 flex-grow-1"
                      >
                      <v-card-title>
                        <v-container>
                          <v-row>
                            <v-col>
                              <h1 class="text-decoration-underline primary--text">CRN: {{course.courseRegistrationNum}}</h1>
                            </v-col>
                          </v-row>
                          <v-row no-gutters>
                          <v-col cols="12">
                            <h4>{{course.subjectCode}} {{course.courseNumber}} - {{course.section}}</h4>
                            <template v-if="course.topic != null"> - {{course.topic}}</template>
                          </v-col>
                        </v-row>
                        <v-row no-gutters>
                          <v-col>
                            <h4>{{course.courseTitle}}</h4>
                          </v-col>
                        </v-row>
                        </v-container>
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
                                  <td>Online</td>
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
    <v-divider></v-divider>
    <v-row>
      <v-col cols="12" lg="4" offset-lg="4">
        <v-btn block x-large color="primary" link href="https://banner.nicholls.edu/prod/twbkwbis.P_WWWLogin" target="_blank">Open Banner in New Tab</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
    name: "ConfirmSchedule",
    computed: {
      desiredCourses: function() {
        return Object.values(this.$store.getters.selectedResults);
      }
    }
}
</script>

<style>

</style>