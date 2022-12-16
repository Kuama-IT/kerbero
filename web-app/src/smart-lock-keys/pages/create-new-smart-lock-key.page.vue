<template>
  <div
    class="flex flex-col w-full h-screen bg-slate-100 justify-center items-center"
  >
    <div
      v-if="isFetchingList"
      class="flex justify-center items-center w-full h-full"
    >
      <SyncLoader></SyncLoader>
    </div>
    <div class="flex flex-col gap-12 justify-center items-center" v-else>
      <SmartLockSelector :items="smartLocks"></SmartLockSelector>
      <div
        class="flex justify-center items-center w-full rounded bg-white p-2 shadow"
      >
        <DateTimeManager
          :end-date="new Date()"
          :start-date="new Date()"
        ></DateTimeManager>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import SmartLockSelector from "@/smart-lock-keys/components/smart-lock-selector.component.vue";
import { useQuery } from "@tanstack/vue-query";
import { SmartLockListResponseDto } from "@/smart-locks/api/smart-lock.schemas";
import { ZodError } from "zod";
import { listSmartLockAction } from "@/smart-locks/api/smart-lock-actions";
import SyncLoader from "@/shared/components/sync-loader.component.vue";
import DateTimeManager from "@/smart-lock-keys/components/date-time-manager.component.vue";

const { isFetching: isFetchingList, data: smartLocks } = useQuery<
  SmartLockListResponseDto,
  ZodError
>({
  queryFn: listSmartLockAction,
  queryKey: ["smartLocksQuery"],
});
</script>
