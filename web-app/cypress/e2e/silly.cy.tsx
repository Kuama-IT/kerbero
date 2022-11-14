/// <reference types="Cypress" />
describe("Silly", () => {
  it("is a silly test", () => {
    cy.wrap("foo").should("equal", "foo");
  });
});
