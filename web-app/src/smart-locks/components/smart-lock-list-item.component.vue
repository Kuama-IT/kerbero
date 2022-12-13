<script setup lang="ts">
import DoorsIcon from "@/shared/icons/doors-icon.component.vue";
import { SmartLockResponseDto } from "../api/smart-lock.schemas";
defineProps<{
  item: SmartLockResponseDto;
}>();
const emit = defineEmits<{
  (e: "manage"): void;
}>();
</script>

<template>
  <div
    role="button"
    @click="emit('manage')"
    class="bg-white rounded-large h-36 w-64 m-4 p-5 cursor-pointer"
  >
    <div class="flex w-full h-1/3 justify-between">
      <p class="text-sm text-gray-700">{{ item.name }}</p>
    </div>
    <div class="flex justify-between items-center">
      <p class="text-sm text-gray-500">{{ item.smartLockProvider }}</p>
      <div class="rounded w-1/3 p-3 bg-purple-300 aspect-square">
        <DoorsIcon class="stroke-purple-500 stroke-1"></DoorsIcon>
      </div>
    </div>
  </div>
</template>
