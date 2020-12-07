<template>
  <div id="app">
    <img alt="Cube logo" src="./assets/cubelogo.png">
    <Checkin/>
  </div>
</template>

<script>
import Checkin from './components/Checkin.vue'

export default {
  name: 'App',
  components: {
    Checkin
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
