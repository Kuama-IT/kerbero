import { Router } from "vue-router";
import { AuthStore } from "../auth/auth.store";
import {
  getRouteLocation,
  isLoginRoute,
  navigateToDashboard,
  navigateToLogin,
} from "./routing-helpers";

export const redirectIfUnauthorized = ({
  router,
  auth,
}: {
  router: Router;
  auth: AuthStore;
}) => {
  auth.$subscribe(async () => {
    // redirect to log-in route whenever the current state looses the authentication
    if (!auth.authenticated) {
      await navigateToLogin({ with: router });
    }

    // redirect to dashboard whenever the current state gains the authentication
    if (isLoginRoute(router.currentRoute.value)) {
      await navigateToDashboard({ with: router });
    }
  });

  router.beforeEach((to, from, next) => {
    // redirect to log-in route when trying to access any route (except log-in) without authentication
    if (!isLoginRoute(to) && !auth.authenticated) {
      next(getRouteLocation({ forAppRoute: "Login" }));
    } else if (isLoginRoute(to) && auth.authenticated) {
      // do not show login to authenticated users
      next(getRouteLocation({ forAppRoute: "Dashboard" }));
    } else next();
  });
};
