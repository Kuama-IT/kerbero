<template>
  <div class="h-full w-full">
    <p class="p-3">Doors</p>
    <div
      v-if="items != null && items.smartLocks.length !== 0"
      class="grid grid-cols-3"
    >
      <SmartLockListItem
        v-for="item in items.smartLocks"
        :item="item"
        :key="item.id"
        @open-smart-lock="emit('openSmartLock', item)"
        @close-smart-lock="emit('closeSmartLock', item)"
        @create-key="emit('createKey', item)"
      >
      </SmartLockListItem>
    </div>
    <div v-else class="flex h-full text-center items-center">
      <p class="w-full">No element to show here</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import SmartLockListItem from "./smart-lock-list-item.component.vue";
import {
  SmartLockListResponseDto,
  SmartLockResponseDto,
} from "../api/smart-lock.schemas";

defineProps<{
  items: SmartLockListResponseDto;
}>();

const emit = defineEmits<{
  (e: "openSmartLock", smartLockResponseDto: SmartLockResponseDto): void;
  (e: "closeSmartLock", smartLockResponseDto: SmartLockResponseDto): void;
  (e: "createKey", smartLockResponseDto: SmartLockResponseDto): void;
}>();
</script>
