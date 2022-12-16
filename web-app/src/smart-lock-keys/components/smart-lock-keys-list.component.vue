<template>
  <div class="m-2 w-full h-full">
    <p class="my-4">keys</p>
    <div
      v-if="items.smartLockKeys.length > 0"
      class="grid grid-cols-3 gap-3 grid-rows-min"
    >
      <SmartLockKeysItem
        v-for="item in items.smartLockKeys"
        :item="item"
        :key="item.id"
        @key-updated="keyUpdated(item)"
        @delete="onDelete(item)"
      >
      </SmartLockKeysItem>
    </div>
    <div class="flex w-full h-full items-center justify-center">
      <p>No elements to show here.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  SmartLockKeyListResponseDto,
  SmartLockKeyResponseDto,
} from "@/smart-lock-keys/api/smart-lock-key.schemas";
import SmartLockKeysItem from "@/smart-lock-keys/components/smart-lock-keys-item.component.vue";

defineProps<{
  items: SmartLockKeyListResponseDto;
}>();

const emit = defineEmits<{
  (e: "keyUpdate", key: SmartLockKeyResponseDto): void;
  (e: "keyDelete", key: SmartLockKeyResponseDto): void;
}>();

const keyUpdated = (key: SmartLockKeyResponseDto) => {
  emit("keyUpdate", key);
};

const onDelete = (key: SmartLockKeyResponseDto) => {
  emit("keyDelete", key);
};
</script>
