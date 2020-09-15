import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    termId: null,
    currentStep: 1
  },
  getters: {
    getTermId: state => {
      return state.termId
    }
  },
  mutations: {
    selectTermId(state, termId) {
      state.termId = termId;
    }
  },
  actions: {
  },
  modules: {
  }
})
