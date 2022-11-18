import { acceptHMRUpdate, defineStore } from "pinia";
import { UserResponse } from "../api/auth.schemas";

type AuthGetters = {
  authenticated: (auth: AuthState) => boolean;
};
type AuthSetters = {
  setUser: (user: UserResponse) => void;
};

type AuthState = {
  user: undefined | UserResponse;
};

const AuthId = "auth";

export const useAuth = defineStore<
  typeof AuthId,
  AuthState,
  AuthGetters,
  AuthSetters
>("auth", {
  state: () => ({
    user: undefined,
  }),
  getters: {
    authenticated(state: AuthState) {
      return state.user !== undefined;
    },
  },
  actions: {
    setUser(user: UserResponse) {
      this.user = user;
    },
  },
});

export type AuthStore = ReturnType<typeof useAuth>;

// make sure to pass the right store definition, `useAuth` in this case.
if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useAuth, import.meta.hot));
}
