import { RouteLocationNormalized, RouteLocationRaw, Router } from "vue-router";

/**
 * The aim of this file is to group actions related to navigation, until we get a better handle on how to strong-type routes
 */

export type AppRoute = "Authenticate" | "Dashboard";

export const isLoginRoute = (route: RouteLocationNormalized) => {
  return route.name === "Authenticate";
};

/**
 * By default, clears browser history and brigs the user to the login page
 * @param {Router} with vue router
 * @param {boolean} replace whether to clear browser history
 */
export const navigateToLogin = async ({
  with: router,
  replace = true,
}: {
  with: Router;
  replace?: boolean;
}) => {
  return await navigate({ to: "Authenticate", with: router, replace });
};

/**
 * By default, clears browser history and brigs the user to the dashboard page
 * @param {Router} with vue router
 * @param {boolean} replace whether to clear browser history
 */
export const navigateToDashboard = async ({
  with: router,
  replace = true,
}: {
  with: Router;
  replace?: boolean;
}) => {
  return await navigate({ to: "Dashboard", with: router, replace });
};

/**
 * Helper to navigate to a @{link AppRoute}
 * @param {Router} with vue router
 * @param {AppRoute} to location you want to reach
 * @param {boolean} replace whether to clear browser history
 */
export const navigate = async ({
  with: router,
  to: appRoute,
  replace = false,
}: {
  with: Router;
  to: AppRoute;
  replace: boolean;
}) => {
  if (replace) {
    return await router.replace({ name: appRoute });
  }

  return await router.push({ name: appRoute });
};

export const getRouteLocation = ({
  forAppRoute: routeName,
}: {
  forAppRoute: AppRoute;
}): RouteLocationRaw => {
  switch (routeName) {
    case "Dashboard":
    case "Authenticate":
      return { name: routeName };
    default:
      throw new Error(`[DEV] unknown route name ${routeName}`);
  }
};
