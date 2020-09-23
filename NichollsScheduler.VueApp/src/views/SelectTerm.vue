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
          <v-col cols="12" md="8" lg="6">
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
        <v-row no-gutters>
          <v-col>
            <v-btn
              color="primary"
              x-large
              @click="continueClicked()"
              :disabled="!termSelected"
            >Continue</v-btn>
          </v-col>
        </v-row>
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
  name: "SelectTerm",
  data() {
    return {
      terms: [],
      error: false,
      doneLoading: false,
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
      },
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
    continueClicked: function () {
      this.$router.push("/select-courses");
    },
  },
  created() {
    this.getTerms();
  },
};
</script>