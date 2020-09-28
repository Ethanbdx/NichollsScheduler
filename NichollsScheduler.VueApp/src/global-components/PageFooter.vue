<template>
    <v-footer color="secondary">
        <v-container>
            <div></div>
             <div class="white--text">Created by Ethan Boudreaux ツ <a href="https://github.com/Ethanbdx/NichollsScheduler">View Source</a></div>
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