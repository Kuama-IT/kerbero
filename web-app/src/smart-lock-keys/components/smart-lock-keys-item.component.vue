<template>
  <div class="flex flex-col w-72 rounded p-3 bg-white">
    <p class="text-sm my-2">{{ item.id }}</p>
    <div class="flex flex-row items-center justify-between">
      <p class="text-sm">password: {{ item.password }}</p>
      <div class="rounded w-2/6 p-2 bg-purple-300 aspect-square">
        <KeyIcon class="stroke-purple-500 stroke-1"></KeyIcon>
      </div>
    </div>
    <div class="my-4">
      <p class="text-xs">phone number</p>
      <p class="text-xs">email@placeholder.com</p>
    </div>
    <DateTimeManager
      :end-date="item.validFromDate"
      :start-date="item.validUntilDate"
      @dates-updated="keyUpdated"
    ></DateTimeManager>
  </div>
</template>

<script setup lang="ts">
import { SmartLockKeyResponseDto } from "@/smart-lock-keys/api/smart-lock-key.schemas";
import { KeyIcon } from "@heroicons/vue/24/outline";
import DateTimeManager from "@/smart-lock-keys/components/date-time-manager.component.vue";

const props = defineProps<{
  item: SmartLockKeyResponseDto;
}>();

const emit = defineEmits<{
  (e: "keyUpdated", key: SmartLockKeyResponseDto): void;
}>();

const keyUpdated = (dates: { from: Date; to: Date }) => {
  let key = props.item;
  key.validFromDate = dates.from;
  key.validUntilDate = dates.to;
  emit("keyUpdated", key);
};
</script>
