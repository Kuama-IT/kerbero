import { setActivePinia, createPinia } from "pinia";
import { useAuth } from "../../../src/auth/stores/auth.store";
import { beforeEach, describe, expect, it } from "vitest";

describe("Auth store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("Holds information about the current auth status of the user", () => {
    const auth = useAuth();

    expect(auth.authenticated).toBe(false);
    expect(auth.user).toBeUndefined();
  });

  it("Allows to set the current user", () => {
    const auth = useAuth();

    expect(auth.authenticated).toBe(false);
    auth.setUser({});
    expect(auth.user).toEqual({});
    expect(auth.authenticated).toBe(true);
  });
});
