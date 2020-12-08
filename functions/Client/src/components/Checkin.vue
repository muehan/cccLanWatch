<template>
  <div>
    <h1>Pila Lan Guest Registration</h1>

    <div v-if="!type">
      <div class="checkin-wrapper">
        <button
          type="button"
          class="btn btn-primary btn-lg btn-block btn-checkin"
          v-on:click="personType('member')"
        >
          Pila Mitglied
        </button>
      </div>
      <div class="checkin-wrapper">
        <button
          type="button"
          class="btn btn-primary btn-lg btn-block btn-checkin"
          v-on:click="personType('friend')"
        >
          Pila Friend
        </button>
      </div>
      <div class="checkin-wrapper">
        <button
          type="button"
          class="btn btn-primary btn-lg btn-block btn-checkin"
          v-on:click="personType('guest')"
        >
          Pila Guest
        </button>
      </div>
    </div>

    <div v-if="type">
      <div class="input-group mb-3">
        <div class="input-group-prepend">
          <span class="input-group-text" id="basic-addon1">name</span>
        </div>
        <input
          v-model="name"
          type="text"
          class="form-control"
          placeholder="name"
          aria-label="name"
          aria-describedby="basic-addon1"
        />
      </div>
      <button
        type="button"
        class="btn btn-primary btn-lg btn-back"
        v-on:click="type = ''"
      >
        back
      </button>
      <button
        type="button"
        class="btn btn-primary btn-lg"
        v-on:click="checkin()"
      >
        Checkin
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Checkin',
  props: {},
  data() {
    return {
      type: '',
      name: '',
      fromMember: '',
    }
  },
  methods: {
    personType: function(entryType) {
      this.type = entryType
    },
    checkin: function() {
      console.log('checkin')
      fetch(`${process.env.VUE_APP_API_URL}registration`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          name: this.name,
          type: this.type,
          fromMember: this.fromMember,
        }),
      })
        .then(response => response.json())
        .then(data => {
          console.log('Success:', data)
          this.$router.push({
            name: 'active',
            params: { id: data.id }
          })
        })
        .catch(error => {
          console.error('Error:', error)
        })
    },
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.checkin-wrapper {
  margin-bottom: 10px;
}
.btn-checkin {
  padding-top: 15px;
  padding-bottom: 15px;
}
.btn-back {
  margin-right: 10px;
}
</style>
