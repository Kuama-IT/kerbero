<script lang="ts" setup>
import { useMutation } from "@tanstack/vue-query";
import { signInAction } from "../api/auth-actions";
import { toFormValidator } from "@vee-validate/zod";
import {
  SignInRequest,
  SignInRequestScheme,
  UserResponse,
} from "../api/auth.schemas";
import { ZodError } from "zod";
import { ErrorMessage, Field, Form } from "vee-validate";
import { navigateToDashboard } from "../../routing/routing-helpers";
import { useRouter } from "vue-router";
import { useAuth } from "../stores/auth.store";

const router = useRouter();

const signInSchema = toFormValidator(SignInRequestScheme);

const { isLoading, mutate } = useMutation<
  UserResponse,
  ZodError,
  SignInRequest
>({
  mutationFn: signInAction,
  onSuccess: (data) => {
    console.log("Login succeded! Check your cookies!");
    const auth = useAuth();
    auth.setUser(data);
    navigateToDashboard({ with: router });
  },
});

const onSubmit = (request: SignInRequest) => {
  mutate(request);
};
</script>
<template>
  <Form
    class="flex flex-col max-w-sm mx-auto gap-4 p-4"
    id="sign-in"
    :validation-schema="signInSchema"
    @submit="onSubmit"
  >
    <div class="flex flex-col gap-1">
      <!-- TODO move to Label component -->
      <label class="block text-gray-700 text-sm font-bold" for="email"
        >Email</label
      >
      <!-- TODO move to our Field component -->
      <Field
        name="email"
        type="email"
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        :disabled="isLoading"
      />
      <ErrorMessage name="email" />
    </div>

    <div class="flex flex-col gap-1">
      <label for="password" class="block text-gray-700 text-sm font-bold"
        >Password</label
      >
      <Field
        name="password"
        type="password"
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        :disabled="isLoading"
      />
      <ErrorMessage name="password" />
    </div>

    <!-- TODO move to button component -->
    <button
      class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
      type="submit"
      :disabled="isLoading"
    >
      Sign in
    </button>
  </Form>
</template>
<style lang="scss"></style>
