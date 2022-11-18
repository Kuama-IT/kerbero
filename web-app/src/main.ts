import { createApp } from "vue";
import { createPinia } from "pinia";
import { VueQueryPlugin } from "@tanstack/vue-query";
import App from "./App.vue";
import "./styles.scss";
import { appRouter } from "./routing/router";

createApp(App)
  .use(VueQueryPlugin)
  .use(appRouter)
  .use(createPinia())
  .mount("#app");
