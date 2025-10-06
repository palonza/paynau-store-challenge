import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
// import Playground from "./Playground";
import AppRoutes from "./routes/AppRoutes";
import { ThemeProvider, CssBaseline, createTheme } from "@mui/material";

// import './index.css'
// import App from './App.tsx'

const theme = createTheme({
  palette: {
    mode: "light", // o "dark"
    background: {
      default: "#f5f5f5",
    },
  },
});

createRoot(document.getElementById('root')!).render(
  <ThemeProvider theme={theme}>
    <StrictMode>
      <CssBaseline />
      {/* <App /> */}
      {/* <Playground /> */}
      <AppRoutes />
    </StrictMode>
  </ThemeProvider>,
)
