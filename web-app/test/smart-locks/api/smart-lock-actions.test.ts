import { fetchMock } from "../../setup/fetch-mock";
import { listSmartLockAction } from "../../../src/smart-locks/api/smart-lock-actions";
import { expect } from "vitest";
import { ZodError } from "zod";
describe("Get SmartLock list", () => {
  beforeEach(() => {
    fetchMock.resetMocks();
  });

  it("Returns a SmartLockListResponse", async () => {
    fetchMock.mockResponseOnce(
      JSON.stringify({
        smartLocks: [
          {
            id: "648862917",
            name: "k-headquarter",
            smartLockProvider: "nuki",
            credentialId: 1,
            state: {
              description: "Closed",
              value: 0,
            },
          },
        ],
        outdatedCredentials: [],
      })
    );
    const response = await listSmartLockAction();

    expect(response.smartLocks.length).toBeGreaterThan(0);
    expect(response.smartLocks).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          id: "648862917",
          name: "k-headquarter",
          smartLockProvider: "nuki",
          credentialId: 1,
          state: {
            description: "Closed",
            value: 0,
          },
        }),
      ])
    );
  });

  it("Returns an unknown provider", async () => {
    fetchMock.mockResponseOnce(
      JSON.stringify({
        smartLocks: [
          {
            id: "648862917",
            name: "k-headquarter",
            smartLockProvider: "kisi",
            credentialId: 1,
            state: {
              description: "Closed",
              value: 0,
            },
          },
        ],
        outdatedCredentials: [],
      })
    );
    try {
      await listSmartLockAction();
    } catch (error) {
      expect(error).toBeInstanceOf(ZodError);
      const zodError = error as ZodError;
      expect(zodError.errors[0].path).toStrictEqual([
        "smartLocks",
        0,
        "smartLockProvider",
      ]);
    }
  });
});
