<script lang="ts" setup>
import { useMutation } from "@tanstack/vue-query";
import {
  SignUpRequest,
  SignUpRequestScheme,
  UserResponse,
} from "../api/auth.schemas";
import { ZodError } from "zod";
import { signUpAction } from "../api/auth-actions";
import { ErrorMessage, Field, Form } from "vee-validate";
import { toFormValidator } from "@vee-validate/zod";

const signUpSchema = toFormValidator(SignUpRequestScheme);

const { mutate, isLoading, isSuccess } = useMutation<
  UserResponse,
  ZodError,
  SignUpRequest
>({
  mutationFn: signUpAction,
});

const onSubmit = (values: SignUpRequest) => {
  console.log(values);
  mutate(values);
};
</script>
<template>
  <Form
    class="flex flex-col grow gap-4 p-4"
    v-if="!isSuccess"
    id="sign-up"
    :validation-schema="signUpSchema"
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
        id="email"
        type="email"
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        :disabled="isLoading"
      />
      <ErrorMessage name="email" />
    </div>

    <div class="flex flex-col gap-1">
      <label class="block text-gray-700 text-sm font-bold" for="userName"
        >Username</label
      >
      <Field
        name="userName"
        type="text"
        id="userName"
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        :disabled="isLoading"
      />
      <ErrorMessage name="userName" />
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
      Sign up
    </button>
  </Form>
  <div v-else>Check your email!</div>
</template>
<style lang="scss"></style>
