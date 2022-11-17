<script setup lang="ts">
import { redirectIfUnauthorized } from "./routing/routing-guards";
import { useRouter } from "vue-router";
import { useAuth } from "./auth/stores/auth.store";

redirectIfUnauthorized({ router: useRouter(), auth: useAuth() });
</script>

<template>
  <router-view></router-view>
</template>

<style lang="scss"></style>
