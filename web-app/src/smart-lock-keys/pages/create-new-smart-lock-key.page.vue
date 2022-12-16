<template>
  <div class="flex flex-col w-full h-screen bg-slate-100">
    <div class="flex flex-row h-14 p-6 justify-end">
      <router-link to="/dashboard">
        <XMarkIcon class="w-10 stroke-slate-400"></XMarkIcon>
      </router-link>
    </div>
    <div class="flex flex-col w-full h-full justify-center items-center">
      <div
        v-if="isFetchingList"
        class="flex justify-center items-center w-full h-full"
      >
        <SyncLoader></SyncLoader>
      </div>
      <div class="flex flex-col justify-center items-center" v-else>
        <CreateSmartLockKeyForm
          :smart-locks="smartLocks"
          :is-loading="isLoading"
          @submit-form="callCreateSmartLockKey"
        ></CreateSmartLockKeyForm>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useMutation, useQuery } from "@tanstack/vue-query";
import { SmartLockListResponseDto } from "@/smart-locks/api/smart-lock.schemas";
import { ZodError } from "zod";
import { listSmartLockAction } from "@/smart-locks/api/smart-lock-actions";
import SyncLoader from "@/shared/components/sync-loader.component.vue";
import CreateSmartLockKeyForm from "@/smart-lock-keys/components/create-smart-lock-key-form.component.vue";
import { createSmartLockKeyAction } from "@/smart-lock-keys/api/smart-lock-key-actions";
import {
  CreateSmartLockKeyRequestDto,
  SmartLockKeyResponseDto,
} from "@/smart-lock-keys/api/smart-lock-key.schemas";
import { useRouter } from "vue-router";
import { navigateToDashboard } from "@/routing/routing-helpers";
import { XMarkIcon } from "@heroicons/vue/24/outline";

const router = useRouter();

const { isFetching: isFetchingList, data: smartLocks } = useQuery<
  SmartLockListResponseDto,
  ZodError
>({
  queryFn: listSmartLockAction,
  queryKey: ["smartLocksQuery"],
});

const { isLoading, mutate } = useMutation<
  SmartLockKeyResponseDto,
  ZodError,
  CreateSmartLockKeyRequestDto
>({
  mutationFn: createSmartLockKeyAction,
  onSuccess: () => {
    navigateToDashboard({ with: router });
  },
});

const callCreateSmartLockKey = (submission: CreateSmartLockKeyRequestDto) => {
  mutate(submission);
};
</script>
