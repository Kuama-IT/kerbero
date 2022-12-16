<template>
  <div class="m-2">
    <p class="my-4">keys</p>
    <div class="grid grid-cols-3 gap-3 grid-rows-min">
      <SmartLockKeysItem v-for="item in items" :item="item" :key="item.id">
      </SmartLockKeysItem>
    </div>
  </div>
</template>

<script setup lang="ts">
import { SmartLockKeyResponseDto } from "@/smart-lock-keys/api/smart-lock-key.schemas";
import SmartLockKeysItem from "@/smart-lock-keys/components/smart-lock-keys-item.component.vue";

defineProps<{
  items: SmartLockKeyResponseDto[];
}>();
</script>
