import { createApp } from "vue";
import { createPinia } from "pinia";
import App from "./App.vue";
import "./styles.scss";
import { appRouter } from "./routing/router";

createApp(App).use(appRouter).use(createPinia()).mount("#app");
