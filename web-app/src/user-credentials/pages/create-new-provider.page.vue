<template>
  <div class="flex flex-col w-full h-screen bg-slate-100">
    <div class="flex w-full h-10 justify-end items-start p-4">
      <router-link to="/user">
        <XMarkIcon class="w-10 stroke-slate-400"></XMarkIcon>
      </router-link>
    </div>
    <div class="flex w-full h-full justify-center items-center">
      <SelectProvider
        :providers="[{ name: 'nuki', icon: '' }]"
        @on-select="providerSelected"
      ></SelectProvider>
    </div>
  </div>
</template>

<script setup lang="ts">
import SelectProvider from "@/user-credentials/components/select-provider.component.vue";
import { startNukiAuthenticationFlowAction } from "@/user-credentials/api/nuki-credential-actions";
import { XMarkIcon } from "@heroicons/vue/24/outline";

const providerSelected = async (provider: { name: string; icon: string }) => {
  if (provider.name == "nuki") {
    const redirectRef = await startNukiAuthenticationFlowAction();
    window.open(redirectRef.redirectUrl, "_blank");
  }
};
</script>

<style scoped></style>
