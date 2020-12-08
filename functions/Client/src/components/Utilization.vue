<template>
  <div class="list-wrapper">
    <ul class="list-group">
      <li class="list-group-item" v-for="registration in registrations" :key="registration.id">
        {{ registration.name }}
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: 'utilization',
  props: {},
  data() {
    return {
      registrations: [],
    }
  },
  methods: {},
  computed: {},
  mounted() {
    fetch(`${process.env.VUE_APP_API_URL}registration`)
      .then(response => response.json())
      .then(data => {
        this.registrations = data
      })
      .catch(error => {
        console.error('Error:', error)
      })
    return true
  },
  filters: {},
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.list-wrapper {
  margin-top: 25px;
}
</style>
