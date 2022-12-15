<template>
  <div class="flex flex-col w-full h-screen bg-slate-100">
    <DeleteDialog
      :id="selectionId"
      :open="openDeleteDialog"
      ref=""
      class="h-screen w-screen absolute z-10"
      @on-close="isDeletionConfirmed"
    ></DeleteDialog>
    <UserStats :user="user"></UserStats>
    <div
      class="flex flex-col w-full h-full border-t-2 border-slate-200 p-4 my-4"
    >
      <p class="text-2xl">linked accounts</p>
      <div class="flex justify-end w-full">
        <CreateNewOneButton @create="toNewCredentialPage"></CreateNewOneButton>
      </div>
      <p class="text-lg">your nuki linked accounts</p>
      <div
        v-if="isFetching || isLoadingDelete"
        class="flex justify-center items-center w-full h-full"
      >
        <SyncLoader></SyncLoader>
      </div>
      <CredentialList
        v-else
        :items="credentials"
        @delete-credential="deleteCredentialById"
      ></CredentialList>
    </div>
  </div>
</template>

<script setup lang="ts">
import CredentialList from "@/user-credentials/components/credential-list.component.vue";
import {
  deleteNukiCredentialsAction,
  listNukiCredentialsAction,
} from "@/user-credentials/api/nuki-credential-actions.js";
import { useMutation, useQuery } from "@tanstack/vue-query";
import {
  NukiCredentialListResponseDto,
  NukiCredentialResponseDto,
} from "@/user-credentials/api/nuki-credential.schemas";
import { ZodError } from "zod";
import SyncLoader from "@/shared/components/sync-loader.component.vue";
import CreateNewOneButton from "@/shared/components/create-new-one-button.component.vue";
import { navigateToCreateNewProvider } from "@/routing/routing-helpers";
import { useRouter } from "vue-router";
import UserStats from "@/user-credentials/components/user-stats.component.vue";
import { useAuth } from "@/auth/stores/auth.store";
import DeleteDialog from "@/user-credentials/components/delete-dialog.component.vue";
import { ref } from "vue";

const {
  isFetching,
  data: credentials,
  refetch,
} = useQuery<NukiCredentialListResponseDto, ZodError>({
  queryFn: listNukiCredentialsAction,
  queryKey: ["getCredentials"],
});

const { isLoading: isLoadingDelete, mutate: mutateDelete } = useMutation<
  NukiCredentialResponseDto,
  ZodError,
  number
>({
  mutationFn: deleteNukiCredentialsAction,
  onSuccess: () => {
    refetch();
  },
});

const openDeleteDialog = ref(false);
const selectionId = ref(0);

const deleteCredentialById = (id: number) => {
  selectionId.value = id;
  openDeleteDialog.value = true;
};

const isDeletionConfirmed = (isConfirmed: boolean, id: number) => {
  if (isConfirmed) {
    mutateDelete(id);
  }
  openDeleteDialog.value = false;
};

const router = useRouter();

const toNewCredentialPage = () => {
  navigateToCreateNewProvider({ with: router });
};

const auth = useAuth();
if (auth.user == undefined) {
  throw new Error("TODO error popup no user found");
}
const user = auth.user;
</script>
