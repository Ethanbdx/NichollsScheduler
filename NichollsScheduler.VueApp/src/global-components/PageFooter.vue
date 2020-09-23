<template>
    <v-footer>
        <v-container>
            <v-stepper v-model="currentStep">
    <v-stepper-header>
      <v-stepper-step :complete="currentStep > 1" step="1">Select a Term</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="currentStep > 2" step="2">Search Courses</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="currentStep > 3" step="3">Select Sections</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step step="4">Confirm Schedule</v-stepper-step>
    </v-stepper-header>
        </v-stepper>
        </v-container>
        
        <v-container>
            <div></div>
             <div>Created by Ethan Boudreaux ツ - <a href="https://github.com/Ethanbdx/NichollsScheduler">View Source</a></div>
        </v-container>
    </v-footer>
</template>

<script>
export default {
    name: 'PageFooter',
    computed: {
        currentStep: function() {
            return this.$store.getters.currentStep
        }
    }
}
</script>