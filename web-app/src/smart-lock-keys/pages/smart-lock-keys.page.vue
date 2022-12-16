<template>
  <div class="flex flex-col w-full h-screen bg-slate-100 gap-3 p-4">
    <div class="flex flex-row justify-end">
      <CreateNewOneButton @create="createNewSmartLockKey"></CreateNewOneButton>
    </div>
    <div
      v-if="isFetchingList || isLoadingUpdate || isLoadingDelete"
      class="flex justify-center items-center w-full h-full"
    >
      <SyncLoader></SyncLoader>
    </div>
    <div v-else class="flex w-full h-full">
      <SmartLockKeysList
        :items="smartLockKeys"
        @key-update="updateSmartLockKey"
        @key-delete="deleteSmartLockKey"
      ></SmartLockKeysList>
    </div>
    <DeleteKeyDialog
      :id="selectionId"
      :open="openDeleteDialog"
      @on-close="deleteSmartLockKeyConfirmed"
    ></DeleteKeyDialog>
  </div>
</template>

<script setup lang="ts">
import { useMutation, useQuery } from "@tanstack/vue-query";
import { ZodError } from "zod";
import {
  deleteSmartLockKeyAction,
  listSmartLockKeysAction,
  updateSmartLockKeyAction,
} from "@/smart-lock-keys/api/smart-lock-key-actions";
import {
  SmartLockKeyListResponseDto,
  SmartLockKeyResponseDto,
  UpdateSmartLockKeyRequestDto,
} from "@/smart-lock-keys/api/smart-lock-key.schemas";
import SmartLockKeysList from "@/smart-lock-keys/components/smart-lock-keys-list.component.vue";
import SyncLoader from "@/shared/components/sync-loader.component.vue";
import CreateNewOneButton from "@/shared/components/create-new-one-button.component.vue";
import { ref } from "vue";
import DeleteKeyDialog from "@/smart-lock-keys/components/delete-key-dialog.component.vue";

const {
  isFetching: isFetchingList,
  data: smartLockKeys,
  refetch,
} = useQuery<SmartLockKeyListResponseDto, ZodError>({
  queryFn: listSmartLockKeysAction,
  queryKey: ["keyList"],
});

const createNewSmartLockKey = () => {
  // TODO navigate to create page
};

const { isLoading: isLoadingUpdate, mutate: executeUpdating } = useMutation<
  SmartLockKeyResponseDto,
  ZodError,
  {
    smartLockKeyId: string;
    updateSmartLockKeyRequestDto: UpdateSmartLockKeyRequestDto;
  }
>({
  mutationFn: updateSmartLockKeyAction,
});

const updateSmartLockKey = (key: SmartLockKeyResponseDto) => {
  executeUpdating({
    smartLockKeyId: key.id,
    updateSmartLockKeyRequestDto: {
      validFromDate: key.validFromDate,
      validUntilDate: key.validUntilDate,
    },
  });
  refetch();
};

const openDeleteDialog = ref(false);
const selectionId = ref("");

const deleteSmartLockKey = (key: SmartLockKeyResponseDto) => {
  selectionId.value = key.id;
  openDeleteDialog.value = true;
};

const { isLoading: isLoadingDelete, mutate: executeDelete } = useMutation<
  SmartLockKeyResponseDto,
  ZodError,
  string
>({
  mutationFn: deleteSmartLockKeyAction,
  onSuccess: () => refetch(),
});

const deleteSmartLockKeyConfirmed = (isConfirmed: boolean, id: string) => {
  if (isConfirmed) {
    executeDelete(id);
  }
  openDeleteDialog.value = false;
};
</script>
