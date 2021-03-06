import Vue from 'vue'
import App from './App.vue'
import VueRouter from 'vue-router'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import Checkin from './components/Checkin.vue'
import Active from './components/Active.vue'
import Utilization from './components/Utilization.vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.use(BootstrapVue)
Vue.use(IconsPlugin)
Vue.config.productionTip = false
Vue.use(VueRouter)

const routes = [
  { path: '/', name: 'checkin', component: Checkin },
  { path: '/active/:id', name: 'active', component: Active },
  { path: '/utilization', component: Utilization },
]

const router = new VueRouter({
  routes, // short for `routes: routes`
})

new Vue({
  router,
  render: h => h(App),
}).$mount('#app')
