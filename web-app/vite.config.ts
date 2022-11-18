import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import mkcert from "vite-plugin-mkcert";
import vue from "@vitejs/plugin-vue";
import dns from "dns";

// Serve the webapp on localhost to avoid cookie domain conflicts
dns.setDefaultResultOrder("verbatim");

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(), mkcert()],
  server: {
    https: true,
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
