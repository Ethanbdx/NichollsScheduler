<template>
    <v-container>
        <v-row>
            <v-col cols="12">
                <h1 text-center>What courses do you want to search for?</h1>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12" lg="3">
                <v-select :items="subjects" v-model="selectedSubject" item-text="fullSubject" item-value="subjectCode" label="Select a Subject" @change="getCourseNumbers()" outlined></v-select>
            </v-col>
            <v-col cols="12" lg="3">
                <v-select :items="courseNumbers" v-model="selectedCourseNumber" label="Select a Course Number" :loading="false" :disabled="!courseNumberEnabled" outlined></v-select>
            </v-col>
            <v-col cols="12" lg="3">
                <v-btn raised x-large color="primary" :disabled="!canAddCourse" @click="addCourseButtonClick()">Add Course to List</v-btn>
            </v-col>
        </v-row>
        <hr>
        <v-row>
            <v-col>
                <h1 text-center>Selected Courses</h1>
            </v-col>
        </v-row>
        <v-row dense>
            <v-col v-for="course in selectedCourses" :key="course.subject + course.courseNumber" cols="12" sm="6" md="4" lg="3">
                <v-card color="#696868" dark>
                    <v-card-title class="headline">{{course.subject + " " + course.courseNumber}}</v-card-title>
                    <v-card-subtitle>PLACEHOLDER COURSE NAME</v-card-subtitle>
                    <v-card-actions>
                        <v-btn text color="#630a0a" @click="removeCourse(course)">Remove</v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>
        <v-row v-if="canContinue">
            <v-col>
                <v-btn color="primary" x-large>Continue</v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
    name: 'CourseSelection',
    data() {
        return {
            subjects: [],
            courseNumbers: [],
            selectedSubject: "",
            selectedCourseNumber: "",
            selectedCourses: []
        }
    },
    computed: {
        canAddCourse: function() {
            return this.selectedCourseNumber.length != 0
        },
        courseNumberEnabled: function() {
            return this.selectedSubject.length != 0
        },
        canContinue: function() {
            return this.selectedCourses.length != 0;
        }
    },
    methods: {
        getSubjects: function() {
            this.$http.get('https://localhost:5001/api/get-course-subjects')
            .then(res => {
                this.subjects = res.data;
            })
            .catch(err => {
                console.log(err)
            })
        },
        getCourseNumbers: function() {
            this.courseNumbers = []
            this.$http.get(`https://localhost:5001/api/get-course-numbers?subject=${this.selectedSubject}`)
            .then(res => {
                this.courseNumbers = res.data;
            })
            .catch(err => {
                console.log(err)
            })
        },
        addCourseButtonClick: function() {
            let course = { subject: this.selectedSubject, courseNumber: this.selectedCourseNumber }
            this.selectedCourses.push(course)

            //clear selections after adding the course.
            this.selectedSubject = ""
            this.selectedCourseNumber = ""
        },
        removeCourse: function(course)  {
            let index = this.selectedCourses.indexOf(course);
            this.selectedCourses.splice(index, 1)
        }
    },
    created() {
        this.getSubjects();
    }
}
</script>