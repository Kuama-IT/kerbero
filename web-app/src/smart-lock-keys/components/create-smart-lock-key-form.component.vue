<template>
  <div
    id="create-key"
    :validation-schema="CreateSmartLockKeyRequestDtoSchema"
    @submit="onSubmit"
    class="flex grow flex-col gap-y-12 p-4"
  >
    <SmartLockSelector
      v-model:selection="smartLockSelected"
      :items="smartLocks"
      @update="onSmartLockSelected"
    ></SmartLockSelector>
    <DateTimeSelector v-model:date-range="dateRange"></DateTimeSelector>
    <!-- TODO move to button component -->
    <button
      class="bg-purple-600 hover:bg-purple-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
      type="submit"
      @click="onSubmit"
      :disabled="isLoading"
    >
      Create key
    </button>
  </div>
</template>

<script setup lang="ts">
import SmartLockSelector from "@/smart-lock-keys/components/smart-lock-selector.component.vue";
import {
  SmartLockListResponseDto,
  SmartLockResponseDto,
} from "@/smart-locks/api/smart-lock.schemas";
import { ref } from "vue";
import DateTimeSelector from "@/smart-lock-keys/components/date-time-selector.component.vue";
import {
  CreateSmartLockKeyRequestDto,
  CreateSmartLockKeyRequestDtoSchema,
} from "@/smart-lock-keys/api/smart-lock-key.schemas";

const props = defineProps<{
  smartLocks: SmartLockListResponseDto;
  isLoading: boolean;
}>();
const emit = defineEmits<{
  (
    e: "submitForm",
    createSmartLockKeyRequestDto: CreateSmartLockKeyRequestDto
  ): CreateSmartLockKeyRequestDto;
}>();

let dateRange = { from: new Date(), to: new Date() };
let smartLockSelected = ref(props.smartLocks.smartLocks[0]);

const onSmartLockSelected = (selection: SmartLockResponseDto) => {
  smartLockSelected.value = selection;
};

const onSubmit = () => {
  // TODO: validate form
  const createSmartLockKeyRequest = {
    smartLockId: smartLockSelected.value.id || "",
    validFromDate: dateRange.from,
    validUntilDate: dateRange.to,
    credentialId: smartLockSelected.value.credentialId,
    smartLockProvider: smartLockSelected.value.smartLockProvider,
  };
  emit("submitForm", createSmartLockKeyRequest);
};
</script>

<style scoped></style>
