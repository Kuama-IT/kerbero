import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

const DashboardPage = () =>
  import("@/dashboard/pages/dashboard-page.component.vue");
const AuthenticatePage = () =>
  import("@/auth/pages/authenticate-page.component.vue");
const CreateNewProviderPage = () =>
  import("@/user-credentials/pages/create-new-provider.page.vue");

const routes: Readonly<RouteRecordRaw[]> = [
  // TODO improve typings
  { path: "/authenticate", component: AuthenticatePage, name: "Authenticate" },
  { path: "/dashboard", component: DashboardPage, name: "Dashboard" },
  {
    path: "/user/create-credential",
    component: CreateNewProviderPage,
    name: "CreateNewProvider",
  },
];

export const appRouter = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to) {
    if (to.hash) {
      return {
        el: to.hash,
        behavior: "smooth",
      };
    }
    // always scroll to top
    return { top: 0 };
  },
});
