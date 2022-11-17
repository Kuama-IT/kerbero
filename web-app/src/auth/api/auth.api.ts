import {
  SignUpRequest,
  signUpRequestParseOrThrow,
  signUpResponseParseOrThrow,
} from "./auth.schemas";

export const signUpAction = async (request: SignUpRequest) => {
  // ensure data is valid
  signUpRequestParseOrThrow(request);

  // TODO move to own method as soon as we have at least 2 usages of fetch
  const raw = await fetch(import.meta.env.API_ENDPOINT + "users", {
    method: "POST",
    body: JSON.stringify(request),
  });

  const json = await raw.json();

  return signUpResponseParseOrThrow(json);
};
