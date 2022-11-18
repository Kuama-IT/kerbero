import { z } from "zod";

// See https://bit.ly/3TGB2Vb
const emailRegex =
  // eslint-disable-next-line no-control-regex
  /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)])/;

// TODO we should be able to set localized & custom error messages
// WARN: these rules follow Kerbero.Identity validators, and should be kept in-sync whenever the validations changes on server side
const SignInRequestScheme = z.object({
  email: z.string().trim().regex(emailRegex),
  password: z
    .string()
    .trim()
    .refine((val) => val.length > 5)
    .refine((val) => {
      return (
        /[^a-zA-Z\d]/.test(val) && // RequireNonAlphanumeric
        /\d/.test(val) && // RequireDigit
        /[a-z]/.test(val) && // RequireLowercase
        /[A-Z]/.test(val) // RequireUppercase
      );
    }),
});

const SignUpRequestScheme = SignInRequestScheme.extend({
  userName: z
    .string()
    .trim()
    .refine((val) => val.length > 2),
});

export type SignInRequest = z.infer<typeof SignInRequestScheme>;
export const signInRequestParseOrThrow = (request: unknown): SignInRequest => {
  return SignInRequestScheme.parse(request);
};

export type SignUpRequest = z.infer<typeof SignUpRequestScheme>;
export const signUpRequestParseOrThrow = (request: unknown): SignUpRequest => {
  return SignUpRequestScheme.parse(request);
};

const UserResponseScheme = z.object({
  id: z.string(),
  userName: z.string(),
  email: z.string(),
  emailConfirmed: z.boolean(),
});

export type UserResponse = z.infer<typeof UserResponseScheme>;
export const userResponseParseOrThrow = (response: unknown): UserResponse => {
  return UserResponseScheme.parse(response);
};
