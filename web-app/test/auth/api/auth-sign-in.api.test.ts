import { signInAction } from "../../../src/auth/api/auth-actions";
import { ZodError } from "zod";
import { fetchMock } from "../../setup/fetch-mock";

describe("Auth Sign-in API", () => {
  beforeEach(() => {
    fetchMock.resetMocks();
  });

  it("Validates payload before trying to call the sign-in api", async () => {
    // email validation
    try {
      await signInAction({
        email: "an email",
        password: "Password123$",
      });
    } catch (error) {
      expect(error).toBeInstanceOf(ZodError);
      const zodError = error as ZodError;
      expect(zodError.errors[0].path).toStrictEqual(["email"]);
    }

    // password
    try {
      await signInAction({
        email: "email@email.email",
        password: "a password",
      });
    } catch (error) {
      expect(error).toBeInstanceOf(ZodError);
      const zodError = error as ZodError;
      expect(zodError.errors[0].path).toStrictEqual(["password"]);
    }
  });

  it("Returns a SignInResponse", async () => {
    fetchMock.mockResponseOnce(
      JSON.stringify({
        id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        userName: "username",
        email: "email@email.email",
        emailConfirmed: false,
      })
    );
    // Just testing happy paths for now
    const response = await signInAction({
      email: "email@email.email",
      password: "Password123$",
    });

    expect(response.email).toStrictEqual("email@email.email");
    expect(response.userName).toStrictEqual("username");
    expect(response.emailConfirmed).toStrictEqual(false);

    expect(fetch).toHaveBeenCalledWith(
      "https://api.test/authentication/login",
      {
        body: JSON.stringify({
          email: "email@email.email",
          password: "Password123$",
        }),
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
  });
});
