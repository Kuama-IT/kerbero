import { fetchMock } from "../../setup/fetch-mock";
import { expect } from "vitest";
import { listSmartLockKeysAction } from "../../../src/smart-lock-keys/api/smart-lock-key-actions";
describe("Get SmartLockKeys list", () => {
  beforeEach(() => {
    fetchMock.resetMocks();
  });

  it("Returns a SmartLockKeyListResponse", async () => {
    fetchMock.mockResponseOnce(
      JSON.stringify({
        smartLockKeys: [
          {
            id: "fa9f51be-a5d2-417d-9766-12476e5d1b29",
            validFromDate: "2022-12-17T13:11:16.688Z",
            validUntilDate: "2022-12-20T13:11:16.688Z",
            password: "board",
          },
        ],
        outdatedCredentials: [],
      })
    );
    const response = await listSmartLockKeysAction();

    expect(response.smartLockKeys.length).toBeGreaterThan(0);
    expect(response.smartLockKeys).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          id: "fa9f51be-a5d2-417d-9766-12476e5d1b29",
          validFromDate: new Date("2022-12-17T13:11:16.688Z"),
          validUntilDate: new Date("2022-12-20T13:11:16.688Z"),
          password: "board",
        }),
      ])
    );
  });
});
