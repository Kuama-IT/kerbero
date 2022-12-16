<template>
  <div class="flex flex-col w-full h-screen bg-slate-100">
    <div
      v-if="isFetchingList || isLoadingOpen || isLoadingClose"
      class="flex justify-center items-center w-full h-full"
    >
      <SyncLoader></SyncLoader>
    </div>
    <SmartLockList
      v-else
      :items="smartLocks"
      @open-smart-lock="openSmartLock"
      @close-smart-lock="closeSmartLock"
      @create-key="createKey"
    ></SmartLockList>
  </div>
</template>

<script setup lang="ts">
import SmartLockList from "@/smart-locks/components/smart-lock-list.component.vue";
import { useMutation, useQuery } from "@tanstack/vue-query";
import { ZodError } from "zod";
import {
  CloseSmartLockRequestDto,
  OpenSmartLockRequestDto,
  SmartLockListResponseDto,
  SmartLockResponseDto,
} from "@/smart-locks/api/smart-lock.schemas";
import {
  closeSmartLockAction,
  listSmartLockAction,
  openSmartLockAction,
} from "@/smart-locks/api/smart-lock-actions";
import SyncLoader from "@/shared/components/sync-loader.component.vue";

const { isFetching: isFetchingList, data: smartLocks } = useQuery<
  SmartLockListResponseDto,
  ZodError
>({
  queryFn: listSmartLockAction,
});

const { isLoading: isLoadingOpen, mutate: executeOpenAction } = useMutation({
  mutationFn: openSmartLockAction,
});

const openSmartLock = (request: SmartLockResponseDto) => {
  const openSmartLockRequestDto: OpenSmartLockRequestDto = {
    smartLockProvider: request.smartLockProvider,
    credentialsId: request.credentialId,
  };
  if (request.id != null) {
    executeOpenAction({
      smartLockId: request.id,
      openSmartLockRequestDto: openSmartLockRequestDto,
    });
  } else {
    throw new Error("Bad Request opening smart-lock");
  }
};

const { isLoading: isLoadingClose, mutate: executeCloseAction } = useMutation({
  mutationFn: closeSmartLockAction,
});

const closeSmartLock = (request: SmartLockResponseDto) => {
  const closeSmartLockRequestDto: CloseSmartLockRequestDto = {
    smartLockProvider: request.smartLockProvider,
    credentialsId: request.credentialId,
  };
  if (request.id != null) {
    executeCloseAction({
      smartLockId: request.id,
      closeSmartLockRequestDto: closeSmartLockRequestDto,
    });
  } else {
    throw new Error("Bad Request opening smart-lock");
  }
};

const createKey = () => {
  // TODO navigate to create key
};
</script>
