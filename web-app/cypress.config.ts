import { defineConfig } from "cypress";
import vitePreprocessor from "cypress-vite";

export default defineConfig({
  video: false,

  component: {
    devServer: {
      framework: "vue",
      bundler: "vite",
    },
    chromeWebSecurity: false,
  },

  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
});
