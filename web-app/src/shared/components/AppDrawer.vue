<template>
  <div class="flex flex-row h-24 bg-white w-full items-stretch">
    <div class="flex justify-center items-center w-56 p-4">
      <h2 class="flex text-2xl tracking-widest font-normal text-center">
        KERBERO
      </h2>
    </div>
    <div class="flex grow justify-end items-center">
      <div class="m-4 w-8 h-8">
        <NotificationIcon class="flex grow"></NotificationIcon>
      </div>
      <div class="flex flex-col justify-center text-right h-full">
        <p class="font-normal text-sm break-after-right">
          {{ user.name }} {{ user.surname }}
        </p>
        <p class="font-normal text-xs">{{ user.role }}</p>
      </div>
      <div class="rounded-full secondary-color w-12 m-4">
        <img class="rounded-full" :src="user.icon" alt="User icon" />
      </div>
    </div>
  </div>
  <div class="flex flex-col h-screen p-4 bg-white w-56">
    <div class="space-y-6 mt-4">
      <div class="flex justify-center">
        <ul class="pt-2 pb-4 space-y-4">
          <li
            class="flex justify-center"
            v-for="(item, index) in sections"
            :key="index"
          >
            <div class="flex w-6 h-6 justify-center items-center m-4">
              <a
                :href="item.href"
                class="flex rounded-md justify-center focus:outline-none rounded-full"
              >
                <component :is="item.icon"></component>
              </a>
            </div>
          </li>
          <li class="flex justify-center pt-24">
            <a
              href="#"
              class="flex p-2 space-x-3 rounded-md justify-center w-2/3"
            >
              <SetupIcon />
            </a>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import MenuIcon from "./../assets/MenuIcon.vue";
import DoorsIcon from "./../assets/DoorsIcon.vue";
import KeyIcon from "./../assets/KeyIcon.vue";
import UserIcon from "./../assets/UsersIcon.vue";
import SetupIcon from "./../assets/SetupIcon.vue";
import NotificationIcon from "./../assets/NotificationIcon.vue";
import avatarIcon from "./../assets/avatar.png";
import { reactive } from "vue";
const user = reactive({
  name: "nicola",
  surname: "salvadore",
  role: "Admin",
  icon: avatarIcon,
});
const sections = [
  { icon: MenuIcon, href: "#" },
  { icon: DoorsIcon, href: "#" },
  { icon: KeyIcon, href: "#" },
  { icon: UserIcon, href: "#" },
];

</script>

<style scoped>
.tracking-widest {
  letter-spacing: 0.35em;
}
</style>
