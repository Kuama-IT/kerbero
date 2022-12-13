<template>
  <div class="m-4 space-y-3">
    <p class="text-4xl">today</p>
    <p>{{ formatDate(now) }}</p>
  </div>
</template>

<script setup lang="ts">
import formatDate from "@/shared/utils/date-time-formatter";
const now = new Date();
</script>
