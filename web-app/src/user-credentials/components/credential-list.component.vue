<template>
  <div class="grid grid-cols-3">
    <CredentialItem
      v-for="item in mappedCredentials"
      :id="item.id"
      :key="item.id"
      :invalid="item.invalid"
      @delete-credential="deleteCredential(item.id)"
    ></CredentialItem>
  </div>
</template>

<script setup lang="ts">
import { NukiCredentialListResponseDto } from "@/user-credentials/api/nuki-credential.schemas";
import CredentialItem from "@/user-credentials/components/credential-item.component.vue";

const props = defineProps<{
  items: NukiCredentialListResponseDto;
}>();

const emit = defineEmits<{
  (e: "deleteCredential", id: number): void;
}>();

const mappedCredentials = [
  ...props.items.credentials,
  ...props.items.outdatedCredentials,
].map((item) => {
  return {
    id: item.id,
    invalid: Object.prototype.hasOwnProperty.call(item, "errors"),
  };
});

const deleteCredential = (id: number) => {
  emit("deleteCredential", id);
};
</script>
