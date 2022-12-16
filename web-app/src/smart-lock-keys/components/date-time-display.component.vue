<template>
  <div class="flex flex-row h-14 gap-2 w-full">
    <CalendarIcon
      class="stroke-1 stroke-purple-700 cursor-pointer"
      @click="updateDate"
    ></CalendarIcon>
    <div class="flex flex-col justify-center text-xs">
      <p>{{ formatDate(startDate) }}</p>
      <p>{{ formatDate(endDate) }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { CalendarIcon } from "@heroicons/vue/24/outline";
import formatDate from "@/shared/utils/date-time-formatter";

const props = defineProps<{
  startDate: Date;
  endDate: Date;
}>();

const emit = defineEmits<{
  (e: "updateDate", range: { from: Date; to: Date }): void;
}>();

const updateDate = () => {
  emit("updateDate", { from: props.startDate, to: props.endDate });
};
</script>

<style scoped></style>
