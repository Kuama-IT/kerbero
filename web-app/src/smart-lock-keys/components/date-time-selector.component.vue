<template>
  <div
    class="flex justify-center items-center w-full rounded bg-white p-2 shadow"
  >
    <DateTimeManager
      :end-date="new Date()"
      :start-date="new Date()"
      @dates-updated="onUpdate"
    ></DateTimeManager>
  </div>
</template>

<script setup lang="ts">
import DateTimeManager from "@/smart-lock-keys/components/date-time-manager.component.vue";
defineProps<{ dateRange: { from: Date; to: Date } }>();
const emit = defineEmits<{
  (e: "update:dateRange", date: { from: Date; to: Date }): {
    from: Date;
    to: Date;
  };
}>();

const onUpdate = (dates: { from: Date; to: Date }) => {
  emit("update:dateRange", dates);
};
</script>

<style scoped></style>
