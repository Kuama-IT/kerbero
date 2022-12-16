/// <reference types="vitest" />

import { defineConfig } from "vite";
import Vue from "@vitejs/plugin-vue";
import { fileURLToPath, URL } from "node:url";

export default defineConfig({
  plugins: [Vue()],
  test: {
    globals: true,
    environment: "jsdom",
    setupFiles: ["./test/setup/fetch-mock.ts"],
    coverage: {
      reporter: ["lcov", "html"],
    },
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
