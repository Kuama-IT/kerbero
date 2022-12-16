<script setup lang="ts">
import { LinkIcon } from "@heroicons/vue/24/outline";
import { EllipsisVerticalIcon } from "@heroicons/vue/24/outline";
import { Popover, PopoverButton, PopoverPanel } from "@headlessui/vue";

defineProps<{
  invalid: boolean;
  id: number;
}>();

const emit = defineEmits<{
  (e: "deleteCredential"): void;
}>();

const onClick = () => {
  emit("deleteCredential");
};
</script>

<template>
  <div class="bg-white rounded-large h-36 w-64 m-4 p-4">
    <div class="flex w-full h-1/3 justify-between items-center mb-1">
      <p class="text-sm text-gray-700">{{ id }}</p>
      <!--      TODO return a human readable identifier-->
      <Popover>
        <PopoverButton>
          <EllipsisVerticalIcon
            class="h-4 cursor-pointer"
          ></EllipsisVerticalIcon>
        </PopoverButton>
        <PopoverPanel class="absolute">
          <button class="text-sm rounded bg-white p-3" @click="onClick">
            delete
          </button>
        </PopoverPanel>
      </Popover>
    </div>
    <div class="flex justify-between items-center mb-2">
      <p v-if="!invalid" class="text-sm text-green-500">valid</p>
      <p v-else class="text-sm text-red-500">outdated</p>
      <div class="rounded w-1/3 p-3 bg-purple-300 aspect-square">
        <LinkIcon class="stroke-purple-500 stroke-1"></LinkIcon>
      </div>
    </div>
  </div>
</template>
