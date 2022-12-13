<template>
  <div class="m-4 space-y-3">
    <p class="text-4xl">today</p>
    <p>{{ formatDate(now) }}</p>
  </div>
</template>

<script setup lang="ts">
const now = new Date();

const formatDate = (dateTime: Date): string => {
  const [weekday, date, time] = new Intl.DateTimeFormat("en-US", {
    weekday: "short",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    year: "2-digit",
    month: "2-digit",
    day: "2-digit",
  })
    .format(dateTime)
    .toLowerCase()
    .split(",");
  return `${weekday} ${date} | ${time}`;
};
</script>
