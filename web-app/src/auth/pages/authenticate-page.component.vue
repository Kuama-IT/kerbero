<template>
  <p>TODO we plan on showing sign up &amp; sign in the same page</p>
  <div class="flex flex-row gap-x-12">
    <SignInComponent />
    <SignUpComponent />
  </div>
</template>

<script lang="ts" setup>
import SignUpComponent from "../components/sign-up.component.vue";
import SignInComponent from "../components/sign-in.component.vue";
</script>
