import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    termId: 0,
    currentStep: 1,
    selectedCourses: []
  },
  getters: {
    termId: state => {
      return state.termId
    },
    selectedCourses: state => {
      return state.selectedCourses
    }
  },
  mutations: {
    setTermId(state, termId) {
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
