import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    termId: 0,
    selectedCourses: [],
    selectedResults: {},
    availableTerms: []
  },
  getters: {
    termId: state => {
      return state.termId
    },
    selectedCourses: state => {
      return state.selectedCourses
    },
    selectedResults: state => {
      return state.selectedResults
    },
    availableTerms: state => {
      return state.availableTerms;
    }
  },
  mutations: {
    setTermId(state, termId) {
      state.termId = termId;
    },
    setSelectedCourses(state, selectedCourses) {
      state.selectedCourses = selectedCourses;
    },
    resetSelectedResults(state) {
      state.selectedResults = {};
    },
    setAvailableTerms(state, terms) {
      state.availableTerms = terms;
    }
  },
  actions: {
  },
  modules: {
  }
})
