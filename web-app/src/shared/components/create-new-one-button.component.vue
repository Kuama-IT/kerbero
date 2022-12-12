<script setup lang="ts">
import { PlusIcon } from "@heroicons/vue/24/outline";

const emit = defineEmits<{
  (e: "create"): void;
}>();

const onClick = () => {
  emit("create");
};
</script>

<template>
  <div
    @click="onClick"
    class="bg-white rounded-large w-64 m-4 p-5 cursor-pointer"
  >
    <div class="flex justify-between items-center">
      <p>create new one</p>
      <div class="rounded w-1/5 p-3 bg-purple-300 aspect-square">
        <PlusIcon class="stroke-purple-500 stroke-2"></PlusIcon>
      </div>
    </div>
  </div>
</template>
