import { mount } from "cypress/vue";
import App from "../../src/App.vue";
import { appRouter } from "../../src/routing/router";
import { createTestingPinia } from "@pinia/testing";
import { AuthStore, useAuth } from "../../src/auth/stores/auth.store";

// helper methods, probably will be moved to own file if and when they are needed from other tests

/**
 * Helper method to write tests that need to await stuff
 * @param test
 */
const asyncTestExecutor = (test: () => Promise<void>) => {
  return () =>
    cy.wrap(async () => {
      await test();
    });
};

/**
 * Helper method to authenticate the `AuthStore`
 */
const fakeAuthenticate = async () => {
  const auth = await new Promise<AuthStore>((resolve) => {
    cy.get<AuthStore>("@auth").then(resolve);
  });
  auth.setUser({});
};

describe("Auth routing", () => {
  beforeEach(() => {
    mount(App, {
      global: {
        plugins: [
          createTestingPinia({
            createSpy: cy.spy,
          }),
          appRouter,
        ],
      },
    }).then(() => {
      cy.wrap(useAuth()).as("auth");
    });
  });

  it(
    "should show login if user is not authenticated",
    asyncTestExecutor(async () => {
      await appRouter.push("/dashboard");
      cy.get("#dashboard").should("not.exist");
      cy.get("#log-in").should("exist");
    })
  );

  it(
    "should allow dashboard (or any other page other than login) if user is authenticated",
    asyncTestExecutor(async () => {
      await fakeAuthenticate();
      cy.get("#dashboard").should("exist");
      await appRouter.push("/dashboard");
      cy.get("#dashboard").should("exist");
    })
  );

  it(
    "should prevent login route to authenticated users",
    asyncTestExecutor(async () => {
      await fakeAuthenticate();

      // try forcing a visit to login component
      await appRouter.push("/login");
      cy.get("#login").should("not.exist");
      cy.get("#dashboard").should("exist");
    })
  );
});
