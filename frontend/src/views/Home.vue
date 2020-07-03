<template>
  <div class="mt-12">
    <v-row>
      <v-col>
        <p class="text-h3 text-center">Student made tool designed to remove the scheduling headaches.</p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <p class="text-h6 text-center">Don't spend hours trying to find classes that find your schedule, just follow the prompts and let the app do it for you!</p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-divider class="mx-4" inset></v-divider>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <p class="text-subtitle-1 text-center">Ready to make your schedule? Click below to start!</p>
      </v-col>
    </v-row>
    <v-row>
      <v-spacer/>
      <v-col>
          <v-btn block x-large color="primary" @click="$router.push('/About')">Start Scheduling</v-btn>
      </v-col>
      <v-spacer/>
    </v-row>
  </div>
</template>

<script>

export default {
  name: 'Home',
  components: {
  }
}
</script>
