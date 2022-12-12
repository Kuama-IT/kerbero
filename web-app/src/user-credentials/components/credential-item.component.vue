<script setup lang="ts">
import { LinkIcon } from "@heroicons/vue/24/outline";
import { NukiCredentialResponseDto } from "../api/nuki-credential.schemas";

defineProps<{
  invalid: boolean;
  item: NukiCredentialResponseDto;
}>();

const emit = defineEmits<{
  (e: "manage"): void;
}>();

const onClick = () => {
  emit("manage");
};
</script>

<template>
  <div
    class="bg-white rounded-large h-36 w-64 m-4 p-5 cursor-pointer"
    @click="onClick"
  >
    <div class="flex w-full h-1/3 justify-between">
      <p class="text-sm text-gray-700">{{ item.id }}</p>
      <!--      TODO return a human readable identifier-->
    </div>
    <div class="flex justify-between items-center">
      <p v-if="!invalid" class="text-sm text-green-500">valid</p>
      <p v-else class="text-sm text-red-500">outdated</p>
      <div class="rounded w-1/3 p-3 bg-purple-300 aspect-square">
        <LinkIcon class="stroke-purple-500 stroke-1"></LinkIcon>
      </div>
    </div>
  </div>
</template>
