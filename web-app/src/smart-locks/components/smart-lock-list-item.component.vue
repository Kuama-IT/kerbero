<script setup lang="ts">
import DoorsIcon from "@/shared/icons/doors-icon.component.vue";
import { SmartLockResponseDto } from "../api/smart-lock.schemas";
import { Popover, PopoverButton, PopoverPanel } from "@headlessui/vue";
import { EllipsisVerticalIcon } from "@heroicons/vue/24/outline";

defineProps<{
  item: SmartLockResponseDto;
}>();
const emit = defineEmits<{
  (e: "openSmartLock"): void;
  (e: "closeSmartLock"): void;
  (e: "createKey"): void;
}>();
</script>

<template>
  <div
    role="button"
    class="bg-white rounded-large h-36 w-64 m-4 p-5 cursor-pointer"
  >
    <div class="flex w-full h-1/3 justify-between">
      <p class="text-sm text-gray-700">{{ item.name }}</p>
      <Popover>
        <PopoverButton>
          <EllipsisVerticalIcon
            class="h-4 cursor-pointer"
          ></EllipsisVerticalIcon>
        </PopoverButton>
        <PopoverPanel class="absolute">
          <div class="flex flex-col text-sm rounded bg-white shadow">
            <button class="p-2" @click="emit('openSmartLock')">open</button>
            <button class="p-2 border-y" @click="emit('closeSmartLock')">
              close
            </button>
            <button class="p-2" @click="emit('createKey')">create a key</button>
          </div>
        </PopoverPanel>
      </Popover>
    </div>
    <div class="flex justify-between items-center">
      <p class="text-sm text-gray-500">{{ item.smartLockProvider }}</p>
      <div class="rounded w-1/3 p-3 bg-purple-300 aspect-square">
        <DoorsIcon class="stroke-purple-500 stroke-1"></DoorsIcon>
      </div>
    </div>
  </div>
</template>
