<template>
  <div class="flex flex-row justify-between w-full items-center p-4">
    <div class="flex flex-col gap-4 text-lg">
      <p>
        username: <span class="text-purple-600">{{ user.userName }}</span>
      </p>
      <p>
        email: <span class="text-purple-600">{{ user.email }}</span>
      </p>
      <p v-if="user.emailConfirmed">
        account verified <span class="text-purple-600">&check;</span>
      </p>
      <p class="text-amber-600" v-else>
        Your email has not been verified, please check your inbox
      </p>
    </div>
    <div
      class="flex justify-center items-center rounded-full bg-purple-300 w-24 h-24"
    >
      <img
        :src="UserImg"
        alt="user icon"
        class="object-cover w-24 h-24 rounded-full"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { UserResponse } from "@/auth/api/auth.schemas";
import UserImg from "@/assets/img.png";

defineProps<{
  user: UserResponse;
}>();
</script>
