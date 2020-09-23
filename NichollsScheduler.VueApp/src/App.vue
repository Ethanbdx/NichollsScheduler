<template>
  <v-app>
    <PageHeader />
    <v-main>
        <transition name="component-fade" mode="out-in">
        <router-view />
        </transition>
    </v-main>
    <PageFooter />
  </v-app>
  
</template>

<style scoped>
.component-fade-enter-active, .component-fade-leave-active {
  transition: opacity .3s ease;
}
.component-fade-enter, .component-fade-leave-to {
  opacity: 0;
}
</style>

<script>
import PageHeader from './global-components/PageHeader';
import PageFooter from './global-components/PageFooter';

export default {
  name: 'App',

  components: {
    PageHeader,
    PageFooter
  },
  data: () => ({
    
  }),
};
</script>
