<template>
  <div class="flex flex-col w-72 rounded p-4 bg-white my-2">
    <div class="flex justify-between py-2">
      <p class="text-sm my-2">{{ item.id }}</p>
      <button class="w-8 mx-4 shrink-0" @click="emit('delete')">
        <TrashIcon class="stroke-slate-500"></TrashIcon>
      </button>
    </div>
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
      :end-date="item.validUntilDate"
      :start-date="item.validFromDate"
      @dates-updated="keyUpdated"
    ></DateTimeManager>
  </div>
</template>

<script setup lang="ts">
import { SmartLockKeyResponseDto } from "@/smart-lock-keys/api/smart-lock-key.schemas";
import { KeyIcon } from "@heroicons/vue/24/outline";
import DateTimeManager from "@/smart-lock-keys/components/date-time-manager.component.vue";
import { TrashIcon } from "@heroicons/vue/24/outline";

const props = defineProps<{
  item: SmartLockKeyResponseDto;
}>();

const emit = defineEmits<{
  (e: "keyUpdated", key: SmartLockKeyResponseDto): void;
  (e: "delete"): void;
}>();

const keyUpdated = (dates: { from: Date; to: Date }) => {
  let key = {
    id: props.item.id,
    validFromDate: dates.from,
    validUntilDate: dates.to,
    password: props.item.password,
  };
  emit("keyUpdated", key);
};
</script>
