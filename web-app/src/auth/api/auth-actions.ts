import {
  SignInRequest,
  SignInRequestScheme,
  SignUpRequest,
  SignUpRequestScheme,
  UserResponse,
  UserResponseScheme,
} from "./auth.schemas";
import { httpClient } from "../../shared/http-client/http-client";

export const signUpAction = async (request: SignUpRequest) => {
  // ensure data is valid
  SignUpRequestScheme.parse(request);

  const json = await httpClient.post({ endpoint: "users", request });

  return UserResponseScheme.parse(json);
};
export const signInAction = async (
  request: SignInRequest
): Promise<UserResponse> => {
  // ensure data is valid
  SignInRequestScheme.parse(request);

  const json = await httpClient.post({
    endpoint: "authentication/login",
    request,
  });

  return UserResponseScheme.parse(json);
};

export const logoutAction = async () => {
  await httpClient.post({
    endpoint: "authentication/logout",
  });
};
