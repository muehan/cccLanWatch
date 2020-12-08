<template>
  <div>
    <p>{{ registration.name }}</p>
    <p>{{ registration.start | formatDate }}</p>
  </div>
</template>

<script>
import moment from 'moment'

export default {
  name: 'active',
  props: {},
  data() {
    return {
      registration: {},
    }
  },
  methods: {},
  computed: {
    registrationid() {
      return this.$route.params.id
    },
  },
  mounted() {
    fetch(`${process.env.VUE_APP_API_URL}registration/${this.registrationid}`)
      .then(response => response.json())
      .then(data => {
        this.registration = data
      })
      .catch(error => {
        console.error('Error:', error)
      })
    return true
  },
  filters: {
    formatDate: function(value) {
      if (value) {
        return moment(String(value)).format('DD-MMM-yyyy hh:mm')
      }
    },
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
