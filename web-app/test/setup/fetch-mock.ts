import createFetchMock from "vitest-fetch-mock";
import { vi } from "vitest";

export const fetchMock = createFetchMock(vi);

// sets globalThis.fetch and globalThis.fetchMock to our mocked version
fetchMock.enableMocks();
