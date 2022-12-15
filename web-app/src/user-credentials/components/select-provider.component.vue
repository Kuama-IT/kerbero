<template>
  <div class="bg-white rounded-large m-4 w-96">
    <p class="p-5">select a provider to link</p>
    <div
      class="cursor-pointer"
      v-for="provider in providers"
      :key="provider.name"
      @click="onClick(provider)"
    >
      <div
        class="flex justify-between flex-row border-slate-300 border-t-2 p-5 items-center"
      >
        <div class="w-12 rounded-b-large border-1 border-slate-400">
          <img
            src="https://nuki.io/wp-content/uploads/2016/04/nuki-logo-white.png"
            alt="nuki logo"
          />
        </div>
        <p class="">{{ provider.name }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  providers: { name: string; icon: string }[];
}>();
const emit = defineEmits<{
  (e: "onSelect", provider: { name: string; icon: string }): void;
}>();

const onClick = (provider: { name: string; icon: string }) => {
  emit("onSelect", provider);
};
</script>

<style scoped></style>
