/// <reference types="cypress" />
import { mount } from "cypress/vue";

declare global {
  namespace Cypress {
    interface Chainable {
      mount;
    }
  }
}
