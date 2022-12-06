import { signUp } from "../../../src/auth/api/auth-actions";
import { ZodError } from "zod";
import { fetchMock } from "../../setup/fetch-mock";

describe("Auth Sign-up API", () => {
  beforeEach(() => {
    fetchMock.resetMocks();
  });

  it("Validates payload before trying to call the sign-up api", async () => {
    // email validation
    try {
      await signUp({
        email: "an email",
        password: "Password123$",
        userName: "username",
      });
    } catch (error) {
      expect(error).toBeInstanceOf(ZodError);
      const zodError = error as ZodError;
      expect(zodError.errors[0].path).toStrictEqual(["email"]);
    }

    // password
    try {
      await signUp({
        email: "email@email.email",
        password: "a password",
        userName: "username",
      });
    } catch (error) {
      expect(error).toBeInstanceOf(ZodError);
      const zodError = error as ZodError;
      expect(zodError.errors[0].path).toStrictEqual(["password"]);
    }
    // username
    try {
      await signUp({
        email: "email@email.email",
        password: "Password123$",
        userName: "sh",
      });
    } catch (error) {
      expect(error).toBeInstanceOf(ZodError);
      const zodError = error as ZodError;
      expect(zodError.errors[0].path).toStrictEqual(["userName"]);
    }
  });

  it("Returns a CreateUserResponse", async () => {
    fetchMock.mockResponseOnce(
      JSON.stringify({
        id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        userName: "username",
        email: "email@email.email",
        emailConfirmed: false,
      })
    );
    // Just testing happy paths for now
    const response = await signUp({
      email: "email@email.email",
      password: "Password123$",
      userName: "username",
    });

    expect(response.email).toStrictEqual("email@email.email");
    expect(response.userName).toStrictEqual("username");
    expect(response.emailConfirmed).toStrictEqual(false);

    expect(fetch).toHaveBeenCalledWith("https://api.test/users", {
      body: JSON.stringify({
        email: "email@email.email",
        password: "Password123$",
        userName: "username",
      }),
      method: "POST",
    });
  });
});
