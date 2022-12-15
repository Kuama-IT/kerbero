/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx,vue}"],
  theme: {
    borderRadius: {
      'none': '0',
      DEFAULT: '0.75em',
      'full': '9999px',
      'large': '1.25em'
    },
    extend: {},
  },
  plugins: [],
};
