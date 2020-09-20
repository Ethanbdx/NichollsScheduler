import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    termId: null,
    currentStep: 1,
    selectedCourses: []
  },
  getters: {
    getTermId: state => {
      return state.termId
    },
    getSelectedCourses: state => {
      return state.selectedCourses
    }
  },
  mutations: {
    selectTermId(state, termId) {
      state.termId = termId;
    },
    setSelectedCourses(state, selectedCourses) {
      state.selectedCourses = selectedCourses;
    }
  },
  actions: {
  },
  modules: {
  }
})
