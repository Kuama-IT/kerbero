import {
  SignInRequest,
  signInRequestParseOrThrow,
  SignUpRequest,
  signUpRequestParseOrThrow,
  userResponseParseOrThrow,
} from "./auth.schemas";
import { httpClient } from "../../shared/http-client/http-client";

export const signUpAction = async (request: SignUpRequest) => {
  // ensure data is valid
  signUpRequestParseOrThrow(request);

  const json = await httpClient.post({ endpoint: "users", request });

  return userResponseParseOrThrow(json);
};
export const signInAction = async (request: SignInRequest) => {
  // ensure data is valid
  signInRequestParseOrThrow(request);

  const json = await httpClient.post({
    endpoint: "authentication/login",
    request,
  });

  return userResponseParseOrThrow(json);
};
