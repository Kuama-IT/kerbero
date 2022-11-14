describe("main App", () => {
  test("It exists", async () => {
    const cmp = await import("../src/App.vue");
    expect(cmp).toBeDefined();
  });
});
