import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

  const routes = [
  {
    path: '/',
    name: 'SelectTerm',
    component: () => import('../views/SelectTerm.vue')
  },
  {
    path: '/select-courses',
    name: 'SelectCourses',
    component: () => import('../views/CourseSelection.vue')
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
