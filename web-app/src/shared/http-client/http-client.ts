const call = async (endpoint: string, request?: RequestInit) => {
  const raw = await fetch(
    import.meta.env.VITE_WEB_API_ENDPOINT + endpoint,
    request
  );

  return await raw.json();
};

export type HttpClientGetRequest = {
  endpoint: string; // TODO this can be typed
  params: undefined | string | string[][] | Record<string, string>;
};

export type HttpClientPostRequest = {
  endpoint: string; // TODO this can be typed
  request: unknown;
};

export type HttpClientPutRequest = HttpClientPostRequest;
export type HttpClientDeleteRequest = Pick<HttpClientPostRequest, "endpoint">;

export const httpClient = {
  get: async ({ endpoint, params }: HttpClientGetRequest) => {
    return await call(endpoint + "?" + new URLSearchParams(params));
  },
  post: async ({ endpoint, request }: HttpClientPostRequest) => {
    return await call(endpoint, {
      method: "POST",
      body: JSON.stringify(request),
    });
  },
  put: async ({ endpoint, request }: HttpClientPutRequest) => {
    return await call(endpoint, {
      method: "PUT",
      body: JSON.stringify(request),
    });
  },
  delete: async ({ endpoint }: HttpClientDeleteRequest) => {
    return await call(endpoint, {
      method: "DELETE",
    });
  },
};
