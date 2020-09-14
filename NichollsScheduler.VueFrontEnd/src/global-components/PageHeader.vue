<template>
    <div>
    <v-app-bar color='primary'>
    <v-container>
      <v-toolbar-title class="d-flex text-h5"><v-icon :color="'white'" :left="true" >book_online</v-icon><a class="white--text" @click="logoClick($router)">Nicholls Scheduler</a></v-toolbar-title>
    </v-container>
    </v-app-bar>
  </div>
</template>

<script>
  export default {
    methods: {
      logoClick: function($router) {
        if(window.location.pathname != '/') {
          $router.push('/')
        }
      }
    }
  }
</script>

<style scoped>

</style>